using System;
using System.Collections.Generic;
using System.Text;
using TopGear.Application.DTOs.PartsDTO;
using TopGear.Application.DTOs.VendorDTO;

namespace TopGear.Application.Interfaces;

public interface IVendorService
{
    Task<IEnumerable<VendorDTO>> GetVendorAsync();
    Task<VendorDTO> GetVendorByIdAsync(Guid id);
    Task<VendorDTO> CreateVendorAsync(CreateVendorDTO vendorCreateDTO);
    Task<VendorDTO?> UpdateVendorAsync(Guid id, EditVendorDTO vendorUpdateDTO);
    Task<bool> DeleteVendorAsync(Guid id);
}
