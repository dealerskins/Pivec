using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pivec.Promotion.Data;

namespace Pivec.Promotion.Web.Controllers
{ 
    public class CustomerController : Controller
    {
        private PivecPromotionEntities db = new PivecPromotionEntities();

        public ViewResult Index()
        {
            var customers = db.Customers.Include("Dealer").Include("Promotion");
            return View(customers.ToList());
        }

        public ViewResult Details(Guid id)
        {
            Customer customer = db.Customers.Single(c => c.Id == id);
            return View(customer);
        }

        public ActionResult Create()
        {
            ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name");
            ViewBag.PromotionId = new SelectList(db.Promotions, "Id", "Name");
            return View();
        } 

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.Id = Guid.NewGuid();
                db.Customers.AddObject(customer);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name", customer.DealerId);
            ViewBag.PromotionId = new SelectList(db.Promotions, "Id", "Name", customer.PromotionId);
            return View(customer);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}