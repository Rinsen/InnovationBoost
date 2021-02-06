using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackDbContext : DbContext
    {
        public OutbackDbContext(DbContextOptions<OutbackDbContext> optionsBuilder)
            : base(optionsBuilder)
        {
        }

        public DbSet<OutbackAllowedCorsOrigin> AllowedCorsOrigins { get; set; }
        public DbSet<OutbackClient> Clients { get; set; }
        public DbSet<OutbackClientClaim> ClientClaims { get; set; }
        public DbSet<OutbackClientLoginRedirectUri> ClientLoginRedirectUris { get; set; }
        public DbSet<OutbackClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }
        public DbSet<OutbackClientScope> ClientScopes { get; set; }
        public DbSet<OutbackClientSecret> ClientSecrets { get; set; }
        public DbSet<OutbackClientSupportedGrantType> ClientSupportedGrantTypes { get; set; }
        public DbSet<OutbackCodeGrant> CodeGrants { get; set; }
        public DbSet<OutbackPersistedGrant> PersistedGrants { get; set; }
        public DbSet<OutbackRefreshTokenGrant> RefreshTokenGrants { get; set; }
        public DbSet<OutbackScope> OutbackScopes { get; set; }
        public DbSet<OutbackScopeClaim> OutbackScopeClaims { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutbackAllowedCorsOrigin>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackClientClaim>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackClientLoginRedirectUri>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackClientPostLogoutRedirectUri>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackClientScope>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackClientSecret>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackClientSupportedGrantType>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackCodeGrant>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackPersistedGrant>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackRefreshTokenGrant>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackClient>(client =>
            {
                client.HasKey(m => m.Id);
                client.HasMany(m => m.AllowedCorsOrigins).WithOne();
                client.HasMany(m => m.ClientClaims).WithOne();
                client.HasMany(m => m.CodeGrants).WithOne();
                client.HasMany(m => m.LoginRedirectUris).WithOne();
                client.HasMany(m => m.PersistedGrants).WithOne();
                client.HasMany(m => m.PostLogoutRedirectUris).WithOne();
                client.HasMany(m => m.RefreshTokenGrants).WithOne();
                client.HasMany(m => m.Scopes).WithOne();
                client.HasMany(m => m.Secrets).WithOne();
                client.HasMany(m => m.SupportedGrantTypes).WithOne();
            });

            modelBuilder.Entity<OutbackScopeClaim>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<OutbackScope>(scope =>
            {
                scope.HasKey(m => m.Id);
                scope.HasMany(m => m.ClientScopes).WithOne();
                scope.HasMany(m => m.ScopeClaims).WithOne();
            });
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetCreatedUpdatedAndTimestampOnSave();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetCreatedUpdatedAndTimestampOnSave();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetCreatedUpdatedAndTimestampOnSave()
        {
            var saveStartTime = DateTimeOffset.Now;

            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is ICreatedAndUpdatedTimestamp && e.State == EntityState.Added))
            {
                ((ICreatedAndUpdatedTimestamp)entry.Entity).Created = saveStartTime;
                ((ICreatedAndUpdatedTimestamp)entry.Entity).Updated = saveStartTime;
            }

            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is ICreatedAndUpdatedTimestamp && e.State == EntityState.Modified))
            {
                ((ICreatedAndUpdatedTimestamp)entry.Entity).Updated = saveStartTime;
            }
        }








    }
}


