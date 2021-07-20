using gbajax.Security;
using gbajax.Service;
using gbajax.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace gbajax.Controllers
{
    public class MembersController : Controller
    {

        private readonly MembersDBService membersSerivce = new MembersDBService();
        private readonly MailService mailService = new MailService();
        // GET: Members
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Guestbooks");
            }

            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            if (ModelState.IsValid)
            {
                RegisterMember.newMember.Password = RegisterMember.Password;
                string AuthCode = mailService.GetValidatcode();
                RegisterMember.newMember.AuthCode = AuthCode;
                membersSerivce.Register(RegisterMember.newMember);

                string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));

                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("EmailValidate", "Members", new
                    {
                        Account = RegisterMember.newMember.Account,
                        AuthCode = AuthCode
                    })
                };

                string MailBody = mailService.GetRegisterEmailBody(TempMail, RegisterMember.newMember.Name, ValidateUrl.ToString().Replace("%3F", "?"));
                mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);

                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                return RedirectToAction("RegisterResult");

            }

            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            return View(RegisterMember);
        }

        public ActionResult RegisterResult()
        {
            return View();
        }

        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            return Json(membersSerivce.AccountCheck(RegisterMember.newMember.Account), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmailValidate(string Account, string Authcode)
        {
            ViewData["EmailValidate"] = membersSerivce.EmailValidate(Account, Authcode);
            return View();
        }

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "GuestBooks");
            }  
            return View();
            
               
        }

        [HttpPost]
        public ActionResult Login (MembersLoginViewModel LoginMember)
        {
            string ValidateStr = membersSerivce.LoginCheck(LoginMember.Account, LoginMember.Password);
            if (String.IsNullOrEmpty(ValidateStr))
            {
                string RoleData = membersSerivce.GetRole(LoginMember.Account);
                JwtService jwtService = new JwtService();
                string CookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);
                HttpCookie cookie = new HttpCookie(CookieName);
                cookie.Value = Server.UrlEncode(Token);
                Response.Cookies.Add(cookie);
                Response.Cookies[CookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                return RedirectToAction("Index", "Guestbooks");
            }

            else
            {
                ModelState.AddModelError("", ValidateStr);
                return View(LoginMember);
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            string CookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            HttpCookie cookie = new HttpCookie(CookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            return RedirectToAction("Login");

        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel ChangeData)
        {
            if(ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersSerivce.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }

            return View();
        }



    }
}