using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace api_authentication_boberto.CustomDbContext
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<UsuarioConfigModel> UsuariosConfig { get; set; }


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

            modelBuilder.Entity<UsuarioConfigModel>(entity =>
            {

                entity.ToTable("usuarios_config");

                entity.HasKey(e => e.UsuarioConfigId).HasName("usuarios_config_pkey");
                entity.Property(e => e.UsuarioConfigId).HasColumnName("usuarios_config_id");

                entity.Property(e => e.UsarEmail).HasColumnName("usaremail");
                entity.Property(e => e.UsarNumeroCelular).HasColumnName("UsarNumerocelular");
                entity.HasOne(e => e.Usuario)
                .WithOne().HasForeignKey("fk_usuarios");
            });

        }


    }
}
