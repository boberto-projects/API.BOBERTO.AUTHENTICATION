using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using System.Diagnostics;
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
                entity.Property(e => e.UsuarioId).HasColumnName("id");
                entity.Property(e => e.UsuarioId).UseIdentityAlwaysColumn();

                entity.Property(e => e.Nome).HasColumnName("nome");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Senha).HasColumnName("senha");

                entity.Property(e => e.NumeroCelular).HasColumnName("numero_celular");

                entity.Property(e => e.UsuarioConfigId).HasColumnName("usuario_config_id");

                entity.HasOne(e => e.UsuarioConfig)
                .WithOne()
                .HasForeignKey<UsuarioConfigModel>(e => e.UsuarioConfigId)
                .OnDelete(DeleteBehavior.Cascade);
                
            });

            modelBuilder.Entity<UsuarioConfigModel>(entity =>
            {

                entity.ToTable("usuarios_config");

                entity.HasKey(e => e.UsuarioConfigId).HasName("usuarios_config_pkey");
                entity.Property(e => e.UsuarioConfigId).HasColumnName("id");
                entity.Property(e => e.UsuarioConfigId).UseIdentityAlwaysColumn();

                entity.Property(e => e.UsarEmail).HasColumnName("usaremail");
                entity.Property(e => e.UsarNumeroCelular).HasColumnName("usarnumerocelular");

                entity.HasOne(e => e.Usuario)
                 .WithOne()
                 .HasForeignKey<UsuarioConfigModel>(e => e.UsuarioConfigId);

            });
        }


    }
}
