using learnUkk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace learnUkk.Controllers
{
    public class HomeController : Controller
    {
        private readonly UkkTwitterContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UkkTwitterContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult Index()
        {
            //get data tweet
            var tweetData = new List<tweetShow>();

            db.Tweets.Include(x => x.User).Include(x => x.TagTweets).ThenInclude(x => x.Tag).Include(x => x.Comments).ThenInclude(x => x.User).Include(x => x.Comments).ThenInclude(x => x.TagComments).ThenInclude(x => x.Tag).ToList().ForEach(x =>
            {
                var listTagTweet = new List<tag>();
                var commentList = new List<commentShow>();

                x.Comments.ToList().ForEach(y =>
                {
                    var listTagComment = new List<tag>();
                    listTagComment.AddRange(y.TagComments.Select(z => new tag { Name = z.Tag.Name }));

                    commentList.Add(new commentShow
                    {
                        CommentId = y.Id,
                        Comment1 = y.Comment1,
                        Picture = y.Picture,
                        User = new userShow { userId = y.User.Id, Email = y.User.Email, userName = y.User.UserName,  Bio = y.User.Bio, Picture = y.User.Picture },
                        Tag = listTagComment
                    });

                });

                listTagTweet.AddRange(x.TagTweets.Select(y => new tag { Name = y.Tag.Name }));

                tweetData.Add(new tweetShow
                {
                    idTweet = x.Id,
                    Tweet1 = x.Tweet1,
                    User = new userShow { userId = x.User.Id, Email = x.User.Email, userName = x.User.UserName ,Bio = x.User.Bio, Picture = x.User.Picture },
                    Picture = x.Picture,
                    Tag = listTagTweet,
                    CommentList = commentList
                });
            });

            return View(new tweetPage { tweetData = tweetData });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}