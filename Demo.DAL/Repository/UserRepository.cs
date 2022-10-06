using Demo.DAL.Context;
using Demo.Entities;

namespace Demo.DAL.Repository
{
    public class UserRepository : GenericRepository<Users>
    {
        public UserRepository(DemoDbContext context) : base(context)
        {
        }
    }
}

