using System;
using System.Collections.Generic;
using System.Text;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class VendorRepository(AppDbContext context): RepositoryBase<Vendor>(context), IVendorRepository 
{
}
