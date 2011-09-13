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

        [Authorize(Roles = "Administrator")]
        public ViewResult Index()
        {
            var customers = db.Customers.Include("Dealer");
            return View(customers.ToList());
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name");
            //ViewBag.PromotionId = new SelectList(db.Promotions, "Id", "Name");
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            // Validates the actual model.
            if (ModelState.IsValid)
            {
                // Create the hashcode for the license driver that will be saved in the database.
                string licenseHashCode = customer.DriverLicenseNumber.GetHashCode().ToString();
                ViewData.Clear();

                Nullable<Guid> promotionId = GetActivePromotionId();

                if (!promotionId.HasValue)
                {
                    ViewData.Add(new KeyValuePair<string, object>("PromotionError", true));
                    ViewData.Add(new KeyValuePair<string, object>("ErrorMessage", "There are no active promotions."));
                    ViewBag.DealerId = null;
                    ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name", customer.DealerId);
                    return View(customer);
                }

                if (IsUserRegistered(licenseHashCode, promotionId.Value))
                {
                    ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name", customer.DealerId);
                    ViewData.Add(new KeyValuePair<string, object>("LicenseError", true));
                    ViewData.Add(new KeyValuePair<string, object>("ErrorMessage", String.Format("{0} {1} has already been included in the current promotion", customer.FirstName, customer.LastName)));
                    return View(customer);
                }
                
                customer.Id = Guid.NewGuid();
                customer.DateCreated = DateTime.UtcNow;
                customer.PromotionId = promotionId.Value;
                customer.DriverLicenseNumber = customer.DriverLicenseNumber.GetHashCode().ToString();
                db.Customers.AddObject(customer);
                db.SaveChanges();
                SendThankYouEmail(customer.Email, customer.FirstName, customer.LastName);

                // Logs off the user and returns them to the Login Page.
                return RedirectToAction("LogOff", "Account");
            }

            ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name", customer.DealerId);
            return View(customer);
        }

        /// <summary>
        /// Sends an email based on the options set in the webconfig file.
        /// </summary>
        /// <param name="toEmailAddress">The customers email address</param>
        private void SendThankYouEmail(string toEmailAddress, string firstName, string lastName)
        {
            // Create the mail message.
            MailMessage mail = new MailMessage();

            // Set the addresses.
            mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailAddressFrom"], ConfigurationManager.AppSettings["FromMask"]);
            mail.To.Add(toEmailAddress);

            // Set the content.
            mail.Subject = ConfigurationManager.AppSettings["EmailSubject"];
            mail.Body = string.Format(ConfigurationManager.AppSettings["EmailBody"], firstName, lastName);

            // Send the message.
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
            smtp.Send(mail);
        }

        /// <summary>
        /// Gets the promotion that is currently available.
        /// </summary>
        /// <returns>The id the promotion</returns>
        private Nullable<Guid> GetActivePromotionId()
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
        /// <param name="promotionId">The id of the promotion</param>
        /// <returns>True if the user is already registered to the actual promotion.</returns>
        private bool IsUserRegistered(string licenseNumber, Guid promotionId)
        {
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