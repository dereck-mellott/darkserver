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
    public class AccountController : Controller
    {
        // GET: Account/Register
        public ActionResult Register()
        {
            SelectList accountTypes = new SelectList(DarkFunctionManager.GetAccountTypes());
            SelectList securityQuestions = new SelectList(DarkFunctionManager.GetSecurityQuestions());
            ViewBag.AccountTypes = accountTypes;
            ViewBag.SecurityQuestions = securityQuestions;
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = DarkAccountManager.CreateUserAccount(model.Username, model.Enabled, model.Password, model.AccountType, model.Answer, model.SecurityQuestion);
                if (result == 1)
                {
                    return RedirectToAction("Login", "Session");
                }
            }
            return View(model);
        }
    }
}