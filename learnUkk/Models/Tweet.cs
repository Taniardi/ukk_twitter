using System;
using System.Collections.Generic;

namespace learnUkk.Models;

public partial class Tweet
{
    public int Id { get; set; }

    public string Tweet1 { get; set; } = null!;

    public int UserId { get; set; }

    public string? Picture { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<TagTweet> TagTweets { get; } = new List<TagTweet>();

    public virtual User User { get; set; } = null!;
}
