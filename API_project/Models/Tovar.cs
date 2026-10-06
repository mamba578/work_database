using System;
using System.Collections.Generic;

namespace API_project.Models;

public partial class Tovar
{
    public int Id { get; set; }

    public string Article { get; set; } = null!;

    public string Title { get; set; } = null!;

    public double Price { get; set; }

    public int IdCategory { get; set; }

    public int IdBrand { get; set; }

    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public string Status { get; set; } = null!;

    public string Photo { get; set; } = null!;

    public virtual Brand IdBrandNavigation { get; set; } = null!;

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
