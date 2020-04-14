using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineFertilizerWeb.Models;

namespace OnlineFertilizerWeb.Controllers
{
    public class ProductsController : Controller
    {
        private FertilizerDBEntities db = new FertilizerDBEntities();
        private readonly int PAGE_SIZE = 8;
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Where(p => p.Status == true);
            var categories = db.Categories;
            ViewBag.categories = categories;
            ViewBag.NumberPage = Math.Ceiling((products.ToList().Count() * 1.0 / PAGE_SIZE));
            int pageIndex = 1;
            ViewBag.PageIndex = pageIndex;
            return View(products.ToList().GetRange(0, PAGE_SIZE));
        }
        // GET: Products/Search/ các parameters
        public ActionResult Search(Search search)
        {
            if (search.TxtSearch == null)
            {
                search.TxtSearch = "";
            }
            var products = db.Products.Include(p => p.Category).Where(p => p.Status == true && p.Title.ToLower().Contains(search.TxtSearch.ToLower()));
            if (search.CategoryID != 0)
            {
                products = db.Products.Include(p => p.Category).Where(p => (p.Status == true && p.CategoryID == search.CategoryID) && p.Title.ToLower().Contains(search.TxtSearch.ToLower()));
            }
            if (string.IsNullOrEmpty(search.TxtSearch))
            {
                products = db.Products.Include(p => p.Category).Where(p => p.Status == true);
                if (search.CategoryID != 0)
                {
                    products = db.Products.Include(p => p.Category).Where(p => (p.Status == true && p.CategoryID == search.CategoryID));
                }
            }
            var categories = db.Categories;
            ViewBag.categories = categories;
            ViewBag.selectCategory = search.CategoryID;
            ViewBag.txtSearch = search.TxtSearch;
            ViewBag.NumberPage = Math.Ceiling((products.ToList().Count() * 1.0 / PAGE_SIZE));
            if (search.PageIndex <=0)
            {
                search.PageIndex = 1;
            }
            ViewBag.PageIndex = search.PageIndex;
            int minIndex = (search.PageIndex - 1) * PAGE_SIZE;
            int maxIndex = Math.Min(search.PageIndex * PAGE_SIZE, products.ToList().Count);
            
            return View("~/Views/Products/Index.cshtml", products.ToList().GetRange(minIndex,maxIndex-minIndex));
        }

       

        // GET: Products/Details/{int}
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
           
            List<Comment> comments = db.Comments.Include(c=>c.Account).Where(c => c.ProductID == id).ToList();
            ViewBag.Product = product;
            ViewBag.Comments = comments;
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
