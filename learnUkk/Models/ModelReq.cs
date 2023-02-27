using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace learnUkk.Models
{
    public class ModelReq
    {
    }
    public class loginReq
    {
        [Required, EmailAddress]
        public string email { get; set; }
        [Required, PasswordPropertyText]
        public string password { get; set; }
    }
    public class registerReq
    {
        [Required, EmailAddress]
        public string email { get; set; }
        [Required]
        public string username { get; set; }
        [Required, PasswordPropertyText]
        public string password { get; set; }
        [Required, PasswordPropertyText]
        public string retype_password { get; set; }
    }

    public class tweetPage
    {
        public List<tweetShow> tweetData { get; set; }
        public commentPostReq commentPostReq { get; set; }
        public string searchString { get; set; }
        public int editTweetId { get; set; }
        public int editCommentId { get; set; }
    }

    public class tweetShow
    {
        public int idTweet { get; set; }
        public string Tweet1 { get; set; } = null!;

        public userShow User { get; set; }

        public string? Picture { get; set; }

        public List<tag> Tag { get; set; }

        public List<commentShow> CommentList { get; set; }

    }
    public class commentShow
    {
        public int CommentId { get; set; }
        public string Comment1 { get; set; } = null!;

        public string? Picture { get; set; }

        public userShow User { get; set; }
        
        public List<tag> Tag { get; set; }

    }

    public class userShow
    {
        public int userId { get; set; }
        public string Email { get; set; } = null!;
        public string userName { get; set; }
        public string? Bio { get; set; }
        public string? Picture { get; set; }
    }

    public class tag
    {
        public string Name { get; set; } = null!;
    }

    public class createPostReq
    {
        public IFormFile file { get; set; }

        public string tweet_text { get; set; }
    }


    public class commentPostReq
    {
        public IFormFile file { get; set; }
        public string tweet_text { get; set; }
        public int tweetId { get; set; }
    }

    public class editTweet
    {
        public int idTweet { get; set; }
        public string Tweet1 { get; set; } = null!;
        public userShow User { get; set; }
        public string? Picture { get; set; }
        public IFormFile file { get; set; }

    }

    public class editComment
    {
        public int idComment { get; set; }
        public string Comment1 { get; set; } = null!;
        public string? Picture { get; set; }
        public IFormFile file { get; set; }

    }
    public class userProfile
    {
        public int userId { get; set; }
        public string Email { get; set; } = null!;
        public string userName { get; set; }

        public string? Bio { get; set; }

        public string? Picture { get; set; }
        public IFormFile file { get; set; }
    }
}
