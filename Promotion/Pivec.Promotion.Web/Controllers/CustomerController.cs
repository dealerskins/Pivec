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

        /// <summary>
        /// Displays the list of customers that have been registered to the current promotion.
        ///  </summary>
        /// <returns>The view with the customers.</returns>
        [Authorize(Roles = "Administrator")]
        public ViewResult Index()
        {
            var customers = db.Customers.Include("Dealer");
            return View(customers.ToList());
        }

        /// <summary>
        /// The initial method that will show the items on screen when access for the first time.
        /// </summary>
        /// <returns>An action indicating the next step.</returns>
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name");
            ViewBag.StateList = new SelectList(GetStateList(), "Value", "Text");
            return View();
        }

        /// <summary>
        /// Includes a new customer into the system.
        /// </summary>
        /// <param name="customer">The Customer to include.</param>
        /// <returns>An action indicating the next step.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            ViewData.Clear();
            // Validates the actual model.
            if (ModelState.IsValid)
            {
                // Create the hashcode for the license driver that will be saved in the database.
                string licenseHashCode = customer.DriverLicenseNumber.GetHashCode().ToString();
                
                Nullable<Guid> promotionId = GetActivePromotionId();

                if (!promotionId.HasValue)
                {
                    ViewData.Add(new KeyValuePair<string, object>("PromotionError", true));
                    ViewData.Add(new KeyValuePair<string, object>("ErrorMessage", "There are no active promotions."));
                    ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name", customer.DealerId);
                    ViewBag.StateList = new SelectList(GetStateList(), "Value", "Text", customer.State);
                    return View(customer);
                }

                if (IsUserRegistered(licenseHashCode, promotionId.Value))
                {
                    ViewData.Add(new KeyValuePair<string, object>("LicenseError", true));
                    ViewData.Add(new KeyValuePair<string, object>("ErrorMessage", String.Format("{0} {1} has already been included in the current promotion", customer.FirstName, customer.LastName)));
                    ViewBag.DealerId = new SelectList(db.Dealers, "Id", "Name", customer.DealerId);
                    ViewBag.StateList = new SelectList(GetStateList(), "Value", "Text", customer.State);
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
            ViewBag.StateList = new SelectList(GetStateList(), "Value", "Text", customer.State);
            return View(customer);
        }

        /// <summary>
        /// Sends an email based on the options set in the webconfig file.
        /// </summary>
        /// <param name="toEmailAddress">The customers email address.</param>
        /// <param name="firstName">The First Name of the customer.</param>
        /// <param name="lastName">The Last Name of the customer.</param>
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

        /// <summary>
        /// Gets the list of the states that belong to the united states.
        /// </summary>
        /// <returns>The list of states.</returns>
        private IEnumerable<SelectListItem> GetStateList()
        {

            List<SelectListItem> stateList = new List<SelectListItem>();

            stateList.Add(new SelectListItem{Text="AK", Value="AK"});
            stateList.Add(new SelectListItem{Text="AL", Value="AL"});
            stateList.Add(new SelectListItem{Text="AR", Value="AR"});
            stateList.Add(new SelectListItem{Text="AS", Value="AS"});
            stateList.Add(new SelectListItem{Text="AZ", Value="AZ"});
            stateList.Add(new SelectListItem{Text="CA", Value="CA"});
            stateList.Add(new SelectListItem{Text="CO", Value="CO"});
            stateList.Add(new SelectListItem{Text="CT", Value="CT"});
            stateList.Add(new SelectListItem{Text="DC", Value="DC"});
            stateList.Add(new SelectListItem{Text="DE", Value="DE"});
            stateList.Add(new SelectListItem{Text="FL", Value="FL"});
            stateList.Add(new SelectListItem{Text="FM", Value="FM"});
            stateList.Add(new SelectListItem{Text="GA", Value="GA"});
            stateList.Add(new SelectListItem{Text="GU", Value="GU"});
            stateList.Add(new SelectListItem{Text="HI", Value="HI"});
            stateList.Add(new SelectListItem{Text="IA", Value="IA"});
            stateList.Add(new SelectListItem{Text="ID", Value="ID"});
            stateList.Add(new SelectListItem{Text="IL", Value="IL"});
            stateList.Add(new SelectListItem{Text="IN", Value="IN"});
            stateList.Add(new SelectListItem{Text="KS", Value="KS"});
            stateList.Add(new SelectListItem{Text="KY", Value="KY"});
            stateList.Add(new SelectListItem{Text="LA", Value="LA"});
            stateList.Add(new SelectListItem{Text="MA", Value="MA"});
            stateList.Add(new SelectListItem{Text="ME", Value="ME"});
            stateList.Add(new SelectListItem{Text="MD", Value="MD"});
            stateList.Add(new SelectListItem{Text="MH", Value="MH"});
            stateList.Add(new SelectListItem{Text="MI", Value="MI"});
            stateList.Add(new SelectListItem{Text="MN", Value="MN"});
            stateList.Add(new SelectListItem{Text="MO", Value="MO"});
            stateList.Add(new SelectListItem{Text="MP", Value="MP"});
            stateList.Add(new SelectListItem{Text="MS", Value="MS"});
            stateList.Add(new SelectListItem{Text="MT", Value="MT"});
            stateList.Add(new SelectListItem{Text="NC", Value="NC"});
            stateList.Add(new SelectListItem{Text="ND", Value="ND"});
            stateList.Add(new SelectListItem{Text="NE", Value="NE"});
            stateList.Add(new SelectListItem{Text="NH", Value="NH"});
            stateList.Add(new SelectListItem{Text="NJ", Value="NJ"});
            stateList.Add(new SelectListItem{Text="NM", Value="NM"});
            stateList.Add(new SelectListItem{Text="NV", Value="NV"});
            stateList.Add(new SelectListItem{Text="NY", Value="NY"});
            stateList.Add(new SelectListItem{Text="OH", Value="OH"});
            stateList.Add(new SelectListItem{Text="OK", Value="OK"});
            stateList.Add(new SelectListItem{Text="OR", Value="OR"});
            stateList.Add(new SelectListItem{Text="PA", Value="PA"});
            stateList.Add(new SelectListItem{Text="PR", Value="PR"});
            stateList.Add(new SelectListItem{Text="PW", Value="PW"});
            stateList.Add(new SelectListItem{Text="RI", Value="RI"});
            stateList.Add(new SelectListItem{Text="SC", Value="SC"});
            stateList.Add(new SelectListItem{Text="SD", Value="SD"});
            stateList.Add(new SelectListItem{Text="TN", Value="TN"});
            stateList.Add(new SelectListItem{Text="TX", Value="TX"});
            stateList.Add(new SelectListItem{Text="UT", Value="UT"});
            stateList.Add(new SelectListItem{Text="VI", Value="VI"});
            stateList.Add(new SelectListItem{Text="VT", Value="VT"});
            stateList.Add(new SelectListItem{Text="VA", Value="VA"});
            stateList.Add(new SelectListItem{Text="WA", Value="WA"});
            stateList.Add(new SelectListItem{Text="WI", Value="WI"});
            stateList.Add(new SelectListItem{Text="WV", Value="WV"});
            stateList.Add(new SelectListItem{Text="WY", Value="WY"});

            return stateList;
        }

        /// <summary>
        /// Disposed method.
        /// </summary>
        /// <param name="disposing">Indicates if needs to be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}