using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TopGear.Domain.Entities;

public class Vendor
{
    public Guid VendorId { get; set; } = Guid.NewGuid();

    [Required]
    public string VendorName { get; set; } = null!;
}
