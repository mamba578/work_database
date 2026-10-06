using System;
using System.Collections.Generic;

namespace API_project.Models;

public partial class Order
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime DeliveryDate { get; set; }

    public double TotalPrice { get; set; }

    public int IdStatus { get; set; }

    public int IdPoint { get; set; }

    public string PaymentType { get; set; } = null!;

    public virtual Point IdPointNavigation { get; set; } = null!;

    public virtual OrderStatus IdStatusNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
