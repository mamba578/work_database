using System;
using System.Collections.Generic;

namespace API_project.Models;

public partial class Point
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public int HomeNumber { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
