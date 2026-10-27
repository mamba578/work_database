using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace API_project.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    [ValidateNever]
    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
