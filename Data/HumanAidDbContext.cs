using Microsoft.EntityFrameworkCore;

namespace HumanAid.Data
{
    public class HumanAidDbContext : DbContext
    {
        public HumanAidDbContext(DbContextOptions<HumanAidDbContext> options)
              : base(options)
        {
        }
    }
}
