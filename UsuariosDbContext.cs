using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace api_authentication_boberto
{
    public class UsuariosDbContext : DbContext
    {
        public UsuariosDbContext(DbContextOptions<UsuariosDbContext> options) : base(options)
        {

        }

        public Microsoft.EntityFrameworkCore.DbSet<UsuarioModel> Usuarios { get; set; }

    }
}
