using RPPSampleMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace RPPSampleMVC.Controllers
{
    public class PaymentController : Controller
    {
        const string MERCHANTCODE = "rppTestMerchant";
        const string CHECKSUMKEY = "UWf6a7cDCP";

        public ActionResult PaymentRequest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PaymentRequest(RequestDetails request)
        {
            Random rnd = new Random();
            string PRN = "PRN" + rnd.Next(100000, 999999);
            PaymentRequest PAYMENTREQUEST = PaymentHelper.SendRequest(PRN, request.AMOUNT, request.PURPOSE, request.USERNAME, request.USERMOBILE, request.USEREMAIL);
            return View("PostRequest", PAYMENTREQUEST);
        }

        public ActionResult PaymentResponse()
        {
            string STATUS = Request.Form["STATUS"];
            string ENCDATA = Request.Form["ENCDATA"];
            PaymentResponse response = PaymentHelper.GetResponse(STATUS, ENCDATA);
            return View(response);
        }

    }
}
