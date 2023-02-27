using System;
using System.Collections.Generic;

namespace learnUkk.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Bio { get; set; }

    public string Password { get; set; } = null!;

    public string? Picture { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Tweet> Tweets { get; } = new List<Tweet>();
}
