using System;
using System.Collections.Generic;

namespace learnUkk.Models;

public partial class TagTweet
{
    public int Id { get; set; }

    public int TweetId { get; set; }

    public int TagId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Tag Tag { get; set; } = null!;

    public virtual Tweet Tweet { get; set; } = null!;
}
