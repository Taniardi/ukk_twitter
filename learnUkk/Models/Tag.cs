using System;
using System.Collections.Generic;

namespace learnUkk.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<TagComment> TagComments { get; } = new List<TagComment>();

    public virtual ICollection<TagTweet> TagTweets { get; } = new List<TagTweet>();
}
