using learnUkk.Models;
using Microsoft.AspNetCore.Mvc;

namespace learnUkk.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UkkTwitterContext db;
        public ProfileController(UkkTwitterContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            //get profile data
            var user = db.Users.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Cookies["user"]));
            return View(new userProfile { Bio = user.Bio, Email = user.Email, Picture = user.Picture, userId = user.Id, userName = user.UserName});
        }

        [HttpPost]
        public async Task<ActionResult> Update(userProfile req)
        {
            //update profile data
            var user = db.Users.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Cookies["user"]));

            if (req.file != null)
            {
                var fileName = DateTime.Now.ToFileTime() + Path.GetExtension(req.file.FileName);
                using (var stream = System.IO.File.Create(Path.GetFullPath("wwwroot/images/" + fileName)))
                {
                    await req.file.CopyToAsync(stream);
                    user.Picture = fileName;
                }
            }

            user.Bio = req.Bio;
            db.SaveChanges();

            return Redirect("/Home");
        }
    }
}
