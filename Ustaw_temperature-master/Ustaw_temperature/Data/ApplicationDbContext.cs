using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ustaw_temperature.Models;

namespace Ustaw_temperature.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Ustaw_temperature.Models.Mieszkanie> Mieszkanie { get; set; } = default!;
        public DbSet<Ustaw_temperature.Models.Harmonogram> Harmonogram { get; set; } = default!;

    }
}
