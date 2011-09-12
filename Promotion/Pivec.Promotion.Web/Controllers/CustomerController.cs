using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using Pivec.Promotion.Data;

namespace Pivec.Promotion.Web.Controllers
{ 
    public class CustomerController : Controller
    {
        private PivecPromotionEntities db = new PivecPromotionEntities();

        public ViewResult Index()
        {
            var customers = db.Customers.Include("Dealer");
            return View(customers.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name");
            //ViewBag.PromotionId = new SelectList(db.Promotions, "Id", "Name");
            return View();
        } 

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            // Validates the actual model.
            if (ModelState.IsValid)
            {
                // Create the hashcode for the license driver that will be saved in the database.
                string licenseHashCode = customer.DriverLicenseNumber.GetHashCode().ToString();

                if (IsUserRegistered(licenseHashCode))
                {
                    // If the user is registered for the promotion return an error message.
                    return RedirectToRoute("Error",
                                           new
                                               {
                                                   errorMessage =
                                               "The customer has already been added to the promotion"
                                               });

                }

                customer.Id = Guid.NewGuid();
                customer.DateCreated = DateTime.UtcNow;
                customer.PromotionId = GetActivePromotionId();
                customer.DriverLicenseNumber = customer.DriverLicenseNumber.GetHashCode().ToString();
                db.Customers.AddObject(customer);
                db.SaveChanges();
                SendThankYouEmail(customer.Email);

                // Logs off the user and returns them to the Login Page.
                return RedirectToAction("Index");
            }

            ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name", customer.DealerId);
            return View(customer);
        }

        /// <summary>
        /// Sends an email based on the options set in the webconfig file.
        /// </summary>
        /// <param name="toEmailAddress">The customers email address</param>
        private void SendThankYouEmail(string toEmailAddress)
        {
            // Create the mail message.
            MailMessage mail = new MailMessage();

            // Set the addresses.
            mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailAddressFrom"], ConfigurationManager.AppSettings["FromMask"]);
            mail.To.Add(toEmailAddress);

            // Set the content.
            mail.Subject = ConfigurationManager.AppSettings["EmailSubject"];
            mail.Body = ConfigurationManager.AppSettings["EmailBody"];

            // Send the message.
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
            smtp.Send(mail);
        }

        /// <summary>
        /// Gets the promotion that is currently available.
        /// </summary>
        /// <returns>The id the promotion</returns>
        private Guid GetActivePromotionId()
        {
            var promotionId = (from p in db.Promotions
                               where p.IsActive == true
                               select p.Id).FirstOrDefault();

            return promotionId;
        }

        /// <summary>
        /// Checks if the user is already registered for the current promotion.
        /// </summary>
        /// <param name="licenseNumber">The license number to check.</param>
        /// <returns>True if the user is already registered to the actual promotion.</returns>
        private bool IsUserRegistered(string licenseNumber)
        {
            Guid promotionId = GetActivePromotionId();
            var userRegistered = (from c in db.Customers
                                      where
                                          c.DriverLicenseNumber.Equals(licenseNumber)
                                          && c.PromotionId == promotionId
                                      select c).ToList();


            return userRegistered.Count > 0;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}