using Microsoft.EntityFrameworkCore;
using IotHomeAPI.Models;

namespace IotHomeAPI.Data;

public class IotHomeDbContext : DbContext
{
    public IotHomeDbContext(DbContextOptions<IotHomeDbContext> options) : base(options) { }

    public DbSet<DhtReading> DhtReadings => Set<DhtReading>();
}