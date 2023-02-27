using System;
using System.Collections.Generic;

namespace learnUkk.Models;

public partial class TagComment
{
    public int Id { get; set; }

    public int CommentId { get; set; }

    public int TagId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
