using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace API_project.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int IdOrder { get; set; }

    public int IdTovar { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    [ValidateNever]
    public virtual Order IdOrderNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Tovar IdTovarNavigation { get; set; } = null!;
}
