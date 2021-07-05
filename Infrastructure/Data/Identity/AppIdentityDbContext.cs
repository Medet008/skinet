using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Identity
{
    // Затем нам нужно настроить AppIdentityDbContext, чтобы включить новый AppRole, и нам также нужно указать здесь тип Id.
    // Поскольку мы придерживаемся значений по умолчанию, это будет означать, что мы используем строку в качестве идентификатора для этих классов идентификации
    public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) {}

    // Это необходимо сделать, чтобы избежать ошибок первичного ключа идентификации () 163
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
        }
    }
}