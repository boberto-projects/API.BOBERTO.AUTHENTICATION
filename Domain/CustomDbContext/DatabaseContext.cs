using api_authentication_boberto.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace api_authentication_boberto.Domain.CustomDbContext
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserModel> Usuarios { get; set; }
        public DbSet<UserConfigModel> UsuariosConfig { get; set; }
        public DbSet<ApiKeyModel> ApiKey { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.UserId).HasName("users_pkey");
                entity.Property(e => e.UserId).HasColumnName("id");
                entity.Property(e => e.UserId).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");

                entity.Property(e => e.UserConfigId).HasColumnName("users_config_id");

                entity.Property(e => e.LastLogin).HasColumnName("last_login");

                entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasDefaultValue(RolesEnum.USER)
                .HasConversion(new EnumToNumberConverter<RolesEnum, int>());


                entity.HasOne(e => e.UserConfig)
                .WithOne()
                .HasForeignKey<UserConfigModel>(e => e.UserConfigId)
                .OnDelete(DeleteBehavior.Cascade);


                entity.HasMany(c => c.ApiKeys)
                .WithOne()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                //entity.HasOne(e => e.ApiKey)
                //.WithOne()
                //.HasForeignKey<ApiKeyModel>(e => e.ApiKeyId)
                //.OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<UserConfigModel>(entity =>
            {

                entity.ToTable("users_config");

                entity.HasKey(e => e.UserConfigId).HasName("users_config_pkey");
                entity.Property(e => e.UserConfigId).HasColumnName("id");
                entity.Property(e => e.UserConfigId).UseIdentityAlwaysColumn();

                entity.Property(e => e.EnabledEmail).HasColumnName("enabled_email");
                entity.Property(e => e.EnabledPhoneNumber).HasColumnName("enabled_phone_number");
            });

            modelBuilder.Entity<ApiKeyModel>(entity =>
            {

                entity.ToTable("api_keys");

                entity.HasKey(e => e.ApiKeyId).HasName("api_keys_pkey");
                entity.Property(e => e.ApiKeyId).HasColumnName("id");
                entity.Property(e => e.ApiKeyId).UseIdentityAlwaysColumn();

                entity.Property(e => e.ApiKey).HasColumnName("apikey");
                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.Property(e => e.Scopes)
                .HasColumnName("scopes")
                .HasConversion(new ValueConverter<List<string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<string>>(v)));

                entity.HasOne(e => e.User)
                .WithMany(c => c.ApiKeys)
                .HasForeignKey(e => e.UserId);
            });
        }


    }
}
