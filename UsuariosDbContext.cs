using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace api_authentication_boberto
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UsuarioModel> Usuarios { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioModel>(entity =>
            {
                 entity.ToTable("usuarios");

                entity.HasKey(e => e.UsuarioId).HasName("usuarios_pkey");
                entity.Property(e => e.UsuarioId).HasColumnName("usuarioid");
                entity.Property(e => e.UsuarioId).UseIdentityByDefaultColumn();

                entity.Property(e => e.Nome).HasColumnName("nome");

                entity.HasKey(e => e.Email).HasName("usuarios_email_key");
                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Senha).HasColumnName("senha");

                entity.Property(e => e.NumeroCelular).HasColumnName("numerocelular");


            });

        }


    }
}
