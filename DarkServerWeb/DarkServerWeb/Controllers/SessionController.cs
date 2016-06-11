using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using DarkServerWeb.Models;
using DarkServer;

namespace DarkServerWeb.Controllers
{
    public class SessionController : Controller
    {
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        // GET: Session/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Session/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string errorMessage = "Unexpected Login failure. Please try again.";
            DarkLoginSession s = Session["DarkLoginSession"] as DarkLoginSession;
            switch (s.Authenticate(model.Username, model.Password))
            {
                case AuthenticationResult.Success:
                    {
                        return RedirectToLocal(returnUrl);
                    }
                case AuthenticationResult.InvalidSession:
                    {
                        errorMessage = "You are already logged into this website from this device. Ensure that you have logged out, and wait a few minutes before trying again.";
                        break;
                    }
                case AuthenticationResult.InvalidLogin:
                    {
                        errorMessage = "The username and password combination do not match. Please try again.";
                        break;
                    }
            }
            ModelState.AddModelError("", errorMessage);
            return View(model);
        }

        public ActionResult Logoff()
        {
            DarkLoginSession s = Session["DarkLoginSession"] as DarkLoginSession;
            s.Logoff();
            return RedirectToAction("Index", "Home");
        }
    }
}