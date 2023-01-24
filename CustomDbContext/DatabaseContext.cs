using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace api_authentication_boberto.CustomDbContext
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<UsuarioConfigModel> UsuariosConfig { get; set; }
        public DbSet<ApiKeyModel> ApiKey { get; set; }

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

                entity.Property(e => e.UltimoLogin).HasColumnName("ultimo_login");


                entity.HasOne(e => e.UsuarioConfig)
                .WithOne()
                .HasForeignKey<UsuarioConfigModel>(e => e.UsuarioConfigId)
                .OnDelete(DeleteBehavior.Cascade);


                entity.HasMany(c => c.ApiKeys)
                .WithOne()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

                //entity.HasOne(e => e.ApiKey)
                //.WithOne()
                //.HasForeignKey<ApiKeyModel>(e => e.ApiKeyId)
                //.OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<UsuarioConfigModel>(entity =>
            {

                entity.ToTable("usuarios_config");

                entity.HasKey(e => e.UsuarioConfigId).HasName("usuarios_config_pkey");
                entity.Property(e => e.UsuarioConfigId).HasColumnName("id");
                entity.Property(e => e.UsuarioConfigId).UseIdentityAlwaysColumn();

                entity.Property(e => e.UsarEmail).HasColumnName("usaremail");
                entity.Property(e => e.UsarNumeroCelular).HasColumnName("usarnumerocelular");
            });

            modelBuilder.Entity<ApiKeyModel>(entity =>
            {

                entity.ToTable("api_keys");

                entity.HasKey(e => e.ApiKeyId).HasName("api_keys_pkey");
                entity.Property(e => e.ApiKeyId).HasColumnName("id");
                entity.Property(e => e.ApiKeyId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasColumnName("apikey");
                entity.Property(e => e.Scopes).HasColumnName("scopes");
                entity.Property(e => e.UsuarioId).HasColumnName("usuarioid");

                entity.HasOne(e => e.Usuario)
                .WithMany(c => c.ApiKeys)
                .HasForeignKey(e => e.UsuarioId);


            });
        }


    }
}
