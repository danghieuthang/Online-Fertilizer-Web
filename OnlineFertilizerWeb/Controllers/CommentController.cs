using OnlineFertilizerWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineFertilizerWeb.Controllers
{
    public class CommentController : Controller
    {
        private FertilizerDBEntities db = new FertilizerDBEntities();
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authentication]
        public ActionResult PostComment(Comment comment)
        {
            Account user = (Account)Session["USER"];

            if (ModelState.IsValid)
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                comment.CreateDate = DateTime.Now;
                comment.CustomerEmail = user.Email;
                comment.Status = true;
                db.Comments.Add(comment);
                db.SaveChanges();
            }

            Product product = db.Products.Find(comment.ProductID);
            if (product == null)
            {
                return HttpNotFound();
            }
            //List<Comment> comments = db.Comments.Include(c => c.Account).Where(c => c.ProductID == id).ToList();

            List<Comment> comments = db.Comments.Include(c=>c.Account).Where(c => c.ProductID == comment.ProductID).ToList();
            ViewBag.Product = product;
            ViewBag.Comments = comments;
            return View("~/Views/Products/Details.cshtml"); // Chuyển lại về trang detail
        }
    }
}