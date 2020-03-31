using System.Data.Entity;

namespace EFCars
{
    public class CarDb : DbContext
    {
        public DbSet<Car> Cars { get; set; }

    }
}
