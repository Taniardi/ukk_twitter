using System;
using System.Collections.Generic;

namespace learnUkk.Models;

public partial class Comment
{
    public int Id { get; set; }

    public string Comment1 { get; set; } = null!;

    public string? Picture { get; set; }

    public int UserId { get; set; }

    public int TweetId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<TagComment> TagComments { get; } = new List<TagComment>();

    public virtual Tweet Tweet { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
