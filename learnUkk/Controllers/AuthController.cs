using learnUkk.Helper;
using learnUkk.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace learnUkk.Controllers
{
    public class AuthController : Controller
    {
        private readonly UkkTwitterContext db;

        public AuthController(UkkTwitterContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            Response.StatusCode = 200;
            return View();
        }

        public IActionResult Register()
        {
            Response.StatusCode = 200;
            return View();
        }

        public IActionResult Login()
        {
            Response.StatusCode = 200;
            return View("index");
        }

        [HttpPost]
        public async Task<ActionResult> Login(loginReq req)
        {
            //check email
            var checkEmail = db.Users.FirstOrDefault(x => x.Email == req.email);

            if(checkEmail == null)
            {
                TempData["msg"] = Core.alertMessage("Email or Password Wrong!");
                Response.StatusCode = 400;
                return View("index");
            }

            //Hash password 
            var password = string.Concat(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(req.password)).Select(x => x.ToString("x2")));

            //compare hashed password and password in datbase
            if(checkEmail.Password != password)
            {
                TempData["msg"] = Core.alertMessage("Email or Password Wrong!");
                Response.StatusCode = 400;
                return View("index");
            }

            //add cookies
            Response.Cookies.Append("user", checkEmail.Id.ToString());

            Response.StatusCode = 200;
            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<ActionResult> Register(registerReq req)
        {
            if(req.email.Length > 50 )
            {
                TempData["msg"] = Core.alertMessage("max character emai is 50 character!");
                Response.StatusCode = 400;
                return View("Register");
            }
            if(req.username.Length > 100)
            {
                TempData["msg"] = Core.alertMessage("max character user name is 100!");
                Response.StatusCode = 400;
                return View("Register");
            }
            //check password and retype password
            if(req.password != req.retype_password)
            {
                TempData["msg"] = Core.alertMessage("Password and retype password must be same!");
                Response.StatusCode = 400;
                return View("Register");
            }

            //check email already exist
            var checkEmail = db.Users.FirstOrDefault(x => x.Email == req.email);

            if(checkEmail != null)
            {
                TempData["msg"] = "<script>alert('Email already used!');</script>";
                Response.StatusCode = 400;
                return View("Register");
            }

            //hash password
            var insertPassword = string.Concat(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(req.password)).Select(x => x.ToString("x2")));

            //save register data
            var obj = new User
            {
                Email = req.email,
                UserName = req.username,
                Password = insertPassword
            };

            db.Users.Add(obj);
            db.SaveChanges();

            Response.StatusCode = 200;
            return Redirect("/Home");
        }

        public async Task<ActionResult> Logout()
        {
            //remove cookie and redirect to login parth
            Response.Cookies.Delete("user");
            return Redirect("/Auth");
        }
    }
}
