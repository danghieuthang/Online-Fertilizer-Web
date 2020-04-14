using OnlineFertilizerWeb.Domain;
using OnlineFertilizerWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OnlineFertilizerWeb.Controllers
{
    public class CartController : Controller
    {
        private FertilizerDBEntities db = new FertilizerDBEntities();
        // GET: Cart
        public ActionResult Index()
        {
            Cart cart = GetCart();
            return View(cart);

        }
        public ActionResult ContinuesShopping()
        {
            return RedirectToAction("Index", "Products");
        }
        public ActionResult AddToCart(int ID, int quantity)
        {

            Product product = db.Products.Where(p => p.ID == ID).FirstOrDefault();
            Cart cart = GetCart();
            cart.AddToCart(product, quantity);
            Session["Cart"] = cart;
            return RedirectToAction("Index", "Cart");
        }
        public ActionResult Remove(int ID)
        {
            Cart cart = GetCart();
            cart.RemoveItem(new Product { ID = ID });
            return RedirectToAction("Index", "Cart");
        }
        public ActionResult Update(int ID, int quantity)
        {
            Cart cart = GetCart();
            cart.Update(new Product { ID = ID }, quantity);
            return RedirectToAction("Index", "Cart");
        }
        [Authentication]
        public ActionResult Checkout()
        {
            Cart cart = GetCart();
            return View(cart);
        }
        [HttpPost]
        [Authentication]
        public ActionResult PlaceAnOrder()
        {

            Cart cart = GetCart();
            Account user = (Account)Session["USER"];
            if (cart.Products.Count() > 0 && user != null)
            {
                Transaction transaction = new Transaction
                {
                    Total = cart.Total(),
                    CreateDate = DateTime.Now,
                    CustomerEmail = user.Email,
                    Status = true
                };
                db.Transactions.Add(transaction);
                if (db.SaveChanges() > 0)
                {
                    int tranID = db.Transactions.Where(tr => tr.CustomerEmail == user.Email).OrderByDescending(tr => tr.ID).FirstOrDefault().ID;
                    foreach (CartItem item in cart.Products)
                    {
                        Order order = new Order
                        {
                            ProductID = item.Product.ID,
                            Quantity = item.Quantity,
                            Total = item.Product.Price * item.Quantity,
                            TransactionID = tranID
                        };
                        db.Orders.Add(order);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        SendEmail(cart);
                        ViewBag.Message = "Buy successfull";
                        Session["Cart"] = null;
                    }

                }
            }
            return View("~/Views/Cart/Index.cshtml", GetCart());
        }
        private void SendEmail(Cart cart)
        {
            try
            {
                Account user = (Account)Session["USER"];
                SmtpClient mainClient = new SmtpClient("smtp.gmail.com", 587);
                mainClient.EnableSsl = true;
                mainClient.Credentials = new NetworkCredential("dhthang1998@gmail.com", "anhthangdepZai123");
                MailMessage message = new MailMessage("dhthang1998@gmail.com", user.Email);
                message.Subject = "Thông tin hóa đơn từ Online Fertilizer";
                string bodyMessage = "Bạn đã mua các sản phẩm: \n";
                foreach (CartItem item in cart.Products)
                {
                    bodyMessage += item.Product.Title + "x" + item.Quantity + "\n";
                }
                bodyMessage += "Total: " + cart.Total();
                message.Body = bodyMessage;
                mainClient.Send(message);
            }
            catch (Exception)
            {

                
            }
        }
        public Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
            }
            return cart;
        }
    }
}