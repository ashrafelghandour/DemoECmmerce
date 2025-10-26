using Microsoft.EntityFrameworkCore;
using OrderApi.Domin.Entites;

namespace OrderApi.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }


        public DbSet<Order> orders { get; set; }
    }
}
