using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data
{
    public class ProductDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Product> products { get; set; }
    }
}
