using System;
using System.Collections.Generic;
using System.Text;

namespace TopGear.Application.DTOs.VendorDTO;

public class VendorDTO
{
    public Guid VendorId { get; set; }

    public string VendorName { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? ContactPerson { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int TotalPartsSupplied { get; set; }
}
