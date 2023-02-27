using learnUkk.Helper;
using learnUkk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using static learnUkk.Models.tweetPage;

namespace learnUkk.Controllers
{
    public class TweetController : Controller
    {
        private readonly UkkTwitterContext db;

        public TweetController(UkkTwitterContext db)
        {
            this.db = db;
        }


        [HttpPost]
        public async Task<ActionResult> Create(createPostReq req)
        {   
            if(req.tweet_text.Length > 255)
            {
                TempData["msg"] = Core.alertMessage("tweet max length is 255");
                Response.StatusCode = 400;
                return View("~/Views/Home/Create.cshtml");
            }
            //check user
            string? fileName = null;
            var user = db.Users.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Cookies["user"]));
            if (user == null)
            {
                return NotFound("User not found.");
            }
            //save data
            if(req.file != null)
            {
                fileName = DateTime.Now.ToFileTime() + Path.GetExtension(req.file.FileName);
                using (var stream = System.IO.File.Create(Path.GetFullPath("wwwroot/images/" +  fileName)))
                {
                    await req.file.CopyToAsync(stream);
                }
            }
            var tag_tweet = req.tweet_text.Split(' ').Where(x => x.Contains("#")).ToList();
            tag_tweet.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);
                if (checkTag == null)
                {
                    db.Tags.Add(new Tag
                    {
                        Name = x.ToString()
                    });

                    db.SaveChanges();
                }
            });

            //save tweets

            db.Tweets.Add(new Tweet
            {
                Tweet1 = req.tweet_text,
                UserId = user.Id,
                Picture = fileName
            });

            await db.SaveChangesAsync();

            var twets = db.Tweets.OrderByDescending(x => x.Id).FirstOrDefault();

            tag_tweet.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);

                db.TagTweets.Add(new TagTweet
                {
                    TweetId = twets.Id,
                    TagId = checkTag.Id
                });
                db.SaveChanges();
            });

            //save
            db.SaveChanges();
            Response.StatusCode = 200;
            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<ActionResult> Comment(tweetPage req)
        {
            if (req.commentPostReq.tweet_text.Length > 255)
            {
                TempData["msg"] = Core.alertMessage("Comment max length is 255");
                Response.StatusCode = 400;
                return Redirect("/Home");
            }
            //check user
            string? fileName = null;
            var user = db.Users.FirstOrDefault(x => x.Id == Convert.ToInt32(Request.Cookies["user"]));
            if (user == null)
            {
                return NotFound("User not found.");
            }

            //save image
            if (req.commentPostReq.file != null)
            {
                fileName = DateTime.Now.ToFileTime() + Path.GetExtension(req.commentPostReq.file.FileName);
                using (var stream = System.IO.File.Create(Path.GetFullPath("wwwroot/images/" + fileName)))
                {
                    await req.commentPostReq.file.CopyToAsync(stream);
                }
            }

            //save tag
            var tag_comment = req.commentPostReq.tweet_text.Split(' ').Where(x => x.Contains("#")).ToList();
            tag_comment.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);
                if (checkTag == null)
                {
                    db.Tags.Add(new Tag
                    {
                        Name = x.ToString()
                    });

                    db.SaveChanges();
                }
            });

            db.Comments.Add(new Comment
            {
                TweetId = req.commentPostReq.tweetId,
                Comment1 = req.commentPostReq.tweet_text,
                UserId = user.Id,
                Picture = fileName
            });

            await db.SaveChangesAsync();

            var comment = db.Tweets.OrderByDescending(x => x.Id).FirstOrDefault();

            //add comment tag
            tag_comment.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);

                db.TagComments.Add(new TagComment
                {
                    CommentId = comment.Id,
                    TagId = checkTag.Id
                });
                db.SaveChanges();
            });

            //save
            db.SaveChanges();
            Response.StatusCode = 200;
            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<ActionResult> Search(tweetPage search)
        {

            if(search.searchString == null)
            {
                return Redirect("/Home");
            }

            var tweetData = new List<tweetShow>();

            db.Tweets.Include(x => x.User).Include(x => x.TagTweets).ThenInclude(x => x.Tag).Include(x => x.Comments).ThenInclude(x => x.User).Include(x => x.Comments).ThenInclude(x => x.TagComments).ThenInclude(x => x.Tag).ToList().Where(x => x.TagTweets.ToList().Where(i => i.Tag.Name.ToLower().Contains(search.searchString.ToLower())).Any() || x.Comments.ToList().Where(i => i.TagComments.Where(j => j.Tag.Name.ToLower().Contains(search.searchString.ToLower())).Any()).Any()).ToList().ForEach(x =>
            {
                var listTagTweet = new List<tag>();
                var commentList = new List<commentShow>();

                x.Comments.ToList().ForEach(y =>
                {
                    var listTagComment = new List<tag>();
                    listTagComment.AddRange(y.TagComments.Select(z => new tag { Name = z.Tag.Name }));

                    commentList.Add(new commentShow
                    {
                        Comment1 = y.Comment1,
                        Picture = y.Picture,
                        User = new userShow { userId = y.User.Id, Email = y.User.Email, userName = y.User.UserName, Bio = y.User.Bio, Picture = y.User.Picture },
                        Tag = listTagComment
                    });

                });

                listTagTweet.AddRange(x.TagTweets.Select(y => new tag { Name = y.Tag.Name }));

                tweetData.Add(new tweetShow
                {
                    idTweet = x.Id,
                    Tweet1 = x.Tweet1,
                    User = new userShow { userId = x.User.Id, Email = x.User.Email, userName = x.User.UserName, Bio = x.User.Bio, Picture = x.User.Picture },
                    Picture = x.Picture,
                    Tag = listTagTweet,
                    CommentList = commentList
                });
            });

            Response.StatusCode = 200;
            return View("~/Views/Home/index.cshtml", new tweetPage { tweetData = tweetData });
        }

        [HttpPost]
        public async Task<ActionResult> EditTweet(tweetPage req)
        {
            var tweet = db.Tweets.Include(x => x.User).FirstOrDefault(x => x.Id == req.editTweetId);

            Response.StatusCode = 200;
            return View("~/Views/Home/EditTweet.cshtml", new editTweet { idTweet = tweet.Id, Picture = tweet.Picture, Tweet1 = tweet.Tweet1, User = new userShow { Bio = tweet.User.Bio, Email = tweet.User.Email, userId = tweet.User.Id, Picture = tweet.User.Picture, userName = tweet.User.UserName } });
        }

        [HttpPost]
        public async Task<ActionResult> EditComment(tweetPage req)
        {
            var comment = db.Comments.Include(x => x.User).FirstOrDefault(x => x.Id == req.editCommentId);

            Response.StatusCode = 200;
            return View("~/Views/Home/EditComment.cshtml", new editComment { idComment = comment.Id, Picture = comment.Picture, Comment1 = comment.Comment1 });
        }

        [HttpPost]
        public async Task<ActionResult> EditTweetPost(editTweet req)
        {
            if(req.Tweet1.Length > 255)
            {
                TempData["msg"] = Core.alertMessage("Password and retype password must be same!");
                Response.StatusCode = 400;
                return Redirect("/Home");
            }

            var tweet = db.Tweets.Include(x => x.User).FirstOrDefault(x => x.Id == req.idTweet);

            tweet.Tweet1 = req.Tweet1;

            if (req.file != null)
            {
                var fileName = DateTime.Now.ToFileTime() + Path.GetExtension(req.file.FileName);
                using (var stream = System.IO.File.Create(Path.GetFullPath("wwwroot/images/" + fileName)))
                {
                    await req.file.CopyToAsync(stream);
                    tweet.Picture = fileName;
                }
            }

            db.TagTweets.RemoveRange(tweet.TagTweets.ToList());
            db.SaveChanges();

            var tag_tweets = req.Tweet1.Split(' ').Where(x => x.Contains("#")).ToList();
            tag_tweets.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);
                if (checkTag == null)
                {
                    db.Tags.Add(new Tag
                    {
                        Name = x.ToString()
                    });

                    db.SaveChanges();
                }
            });

            tag_tweets.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);

                db.TagTweets.Add(new TagTweet
                {
                    TweetId = req.idTweet,
                    TagId = checkTag.Id
                });
                db.SaveChanges();
            });


            Response.StatusCode = 200;
            return Redirect("/Home");
        }


        [HttpPost]
        public async Task<ActionResult> EditCommentPost(editComment req)
        {
            var comment = db.Comments.Include(x => x.User).FirstOrDefault(x => x.Id == req.idComment);

            comment.Comment1 = req.Comment1;

            if (req.file != null)
            {
                var fileName = DateTime.Now.ToFileTime() + Path.GetExtension(req.file.FileName);
                using (var stream = System.IO.File.Create(Path.GetFullPath("wwwroot/images/" + fileName)))
                {
                    await req.file.CopyToAsync(stream);
                    comment.Picture = fileName;
                }
            }

            db.TagComments.RemoveRange(comment.TagComments.ToList());
            db.SaveChanges();

            var tag_tweets = req.Comment1.Split(' ').Where(x => x.Contains("#")).ToList();
            tag_tweets.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);
                if (checkTag == null)
                {
                    db.Tags.Add(new Tag
                    {
                        Name = x.ToString()
                    });

                    db.SaveChanges();
                }
            });

            tag_tweets.ForEach(x =>
            {
                var checkTag = db.Tags.FirstOrDefault(y => y.Name == x);

                db.TagComments.Add(new TagComment
                {
                    CommentId = req.idComment,
                    TagId = checkTag.Id
                });
                db.SaveChanges();
            });


            Response.StatusCode = 200;
            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTweet(tweetPage req)
        {
            var checkTweets = db.Tweets.FirstOrDefault(x => x.Id == req.editTweetId);
            var checkTweetsTag = db.TagTweets.Where(x => x.TweetId == req.editTweetId);
            var checkComment = db.Comments.Include(x => x.TagComments).Where(x => x.TweetId == req.editTweetId).ToList();
            checkComment.ForEach(x =>
            {
                db.TagComments.RemoveRange(x.TagComments);
            });

            db.Comments.RemoveRange(checkComment);
            db.TagTweets.RemoveRange(checkTweetsTag);
            db.Tweets.Remove(checkTweets);
            db.SaveChanges();

            Response.StatusCode = 200;
            return Redirect("/Home"); 
        }

        [HttpPost]
        public async Task<ActionResult> DeleteComment(tweetPage req)
        {
            var checkComment = db.Comments.FirstOrDefault(x => x.Id == req.editCommentId);
            var checkCommentTag = db.TagComments.Where(x => x.CommentId == req.editCommentId);

            db.TagComments.RemoveRange(checkCommentTag);
            db.Comments.Remove(checkComment);
            db.SaveChanges();

            Response.StatusCode = 200;
            return Redirect("/Home");
        }

    }
}
