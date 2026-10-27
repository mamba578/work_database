using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace API_project.Models;

public partial class OrderStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    [ValidateNever]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
