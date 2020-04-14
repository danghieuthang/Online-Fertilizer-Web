using OnlineFertilizerWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineFertilizerWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly string ROLE_LOGIN = "user";
        private FertilizerDBEntities db = new FertilizerDBEntities();
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Products");

        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account account, string returnURL)
        {

            if (ModelState.IsValid) // Kiểm tra người dùng bấm submit chưa
            {
                db.Configuration.ValidateOnSaveEnabled = false; // Tắt tính năng tự xắc thực của validtation
                var user = db.Accounts.Where(a => a.Email.Equals(account.Email) && a.Password.Equals(account.Password)).ToList();
                if (user.Count() > 0)
                {
                    if (user.FirstOrDefault().Role.Name.Equals(ROLE_LOGIN))
                    {
                        Session["USER"] = user.FirstOrDefault();
                        //   FormsAuthentication.SetAuthCookie(account.Name, true);
                        if (string.IsNullOrWhiteSpace(returnURL))
                        {
                            return RedirectToAction("Index", "Products");
                        }
                        else
                        {
                            // return RedirectToAction(returnURL);
                            return RedirectToAction("Index", "Products");
                        }

                    }
                    else
                    {
                        ViewBag.error = "This account not permit login!";
                    }

                }
                else
                {
                    ViewBag.error = "This account not found!";
                }
            }
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Configuration.ValidateOnSaveEnabled = false;

                var users = db.Accounts.Where(a => a.Email.Equals(account.Email));
                if (users.Count() > 0)
                {
                    ViewBag.error = "Email đã tồn tại trong hệ thống";
                }
                else
                {
                    account.CreateDate = DateTime.Now;
                    account.Status = true;
                    account.RoleID = db.Roles.Where(r => r.Name.Equals("user")).FirstOrDefault().ID;
                    db.Accounts.Add(account);
                    if (db.SaveChanges() > 0)
                    {
                        ViewBag.registerStatus = "Create account was successfull, Please login!";
                        return View("~/Views/Account/Login.cshtml");
                    }


                }
            }
            return View();
        }
        [Authentication]
        public ActionResult Profile()
        {
            Account user = (Account)Session["USER"];
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authentication]
        public ActionResult Profile(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    Session["USER"] = account;
                    ViewBag.updateStatus = "Update was success!";
                }
                else
                {
                    ViewBag.error = "Update was fail!";
                }

            }
            return Profile();
        }
        [Authentication]
        public ActionResult History()
        {
            Account user = (Account)Session["USER"];
            var transactions = db.Transactions.Where(tr => tr.CustomerEmail.Equals(user.Email));
            List<Order> orders = new List<Order>();
            foreach (Transaction tran in transactions)
            {
                var order = db.Orders.Where(o => o.TransactionID == tran.ID);
                orders.AddRange(order.ToList());
            }
            ViewBag.transactions = transactions.ToList();
            return View(orders);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            //   FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Products");
        }
    }
}