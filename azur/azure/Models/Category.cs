using System;
using System.Collections.Generic;

namespace azure.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
