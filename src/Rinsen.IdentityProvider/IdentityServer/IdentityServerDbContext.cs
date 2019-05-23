using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerDbContext : DbContext
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public DbSet<IdentityServerApiResource> IdentityServerApiResources { get; set; }

        public DbSet<IdentityServerApiResourceClaim> IdentityServerApiResourceClaims { get; set; }

        public DbSet<IdentityServerApiResourceScope> IdentityServerApiResourceScopes { get; set; }

        public DbSet<IdentityServerApiResourceScopeClaim> IdentityServerApiResourceScopeClaims { get; set; }

        public DbSet<IdentityServerApiResourceSecret> IdentityServerApiResourceSecrets { get; set; }

        public DbSet<IdentityServerClient> IdentityServerClients { get; set; }

        public DbSet<IdentityServerClientClaim> IdentityServerClientClaims { get; set; }

        public DbSet<IdentityServerClientCorsOrigin> IdentityServerClientCorsOrigins { get; set; }

        public DbSet<IdentityServerClientGrantType> IdentityServerClientGrantTypes { get; set; }

        public DbSet<IdentityServerClientIdpRestriction> IdentityServerClientIdpRestrictions { get; set; }

        public DbSet<IdentityServerClientPostLogoutRedirectUri> IdentityServerClientPostLogoutRedirectUris { get; set; }

        public DbSet<IdentityServerClientRedirectUri> IdentityServerClientRedirectUris { get; set; }

        public DbSet<IdentityServerClientScope> IdentityServerClientScopes { get; set; }

        public DbSet<IdentityServerClientSecret> IdentityServerClientSecrets { get; set; }

        public DbSet<IdentityServerIdentityResource> IdentityServerIdentityResources { get; set; }

        public DbSet<IdentityServerIdentityResourceClaim> IdentityServerIdentityResourceClaims { get; set; }

        public DbSet<IdentityServerIdentityResourceProperty> IdentityServerIdentityResourceProperties { get; set; }

        public DbSet<IdentityServerPersistedGrant> IdentityServerPersistedGrants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityServerApiResource>(apiResources =>
            {
                apiResources.HasKey(m => m.Id);
                apiResources.Ignore(m => m.State);
                apiResources.HasMany(m => m.ApiSecrets).WithOne().HasForeignKey(m => m.ApiResourceId).IsRequired();
                apiResources.HasMany(m => m.Claims).WithOne().HasForeignKey(m => m.ApiResourceId).IsRequired();
                apiResources.HasMany(m => m.Scopes).WithOne().HasForeignKey(m => m.ApiResourceId).IsRequired();
                apiResources.HasMany(m => m.Properties).WithOne().HasForeignKey(m => m.ApiResourceId).IsRequired();
            });

            modelBuilder.Entity<IdentityServerApiResourceClaim>(apiResourceClaims =>
            {
                apiResourceClaims.HasKey(m => m.Id);
                apiResourceClaims.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerApiResourceScope>(apiResourceScopes =>
            {
                apiResourceScopes.HasKey(m => m.Id);
                apiResourceScopes.Ignore(m => m.State);
                apiResourceScopes.HasMany(m => m.Claims).WithOne().HasForeignKey(m => m.ApiResourceScopeId).IsRequired();
            });

            modelBuilder.Entity<IdentityServerApiResourceScopeClaim>(apiResourceScopeClaims =>
            {
                apiResourceScopeClaims.HasKey(m => m.Id);
                apiResourceScopeClaims.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerApiResourceSecret>(apiResourceSecret =>
            {
                apiResourceSecret.HasKey(m => m.Id);
                apiResourceSecret.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerApiResourceProperty>(properties =>
            {
                properties.HasKey(m => m.Id);
                properties.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClient>(clients =>
            {
                clients.HasKey(m => m.Id);
                clients.Property(m => m.AccessTokenType).HasColumnType("tinyint");
                clients.Property(m => m.RefreshTokenUsage).HasColumnType("tinyint");
                clients.Property(m => m.RefreshTokenExpiration).HasColumnType("tinyint");
                clients.HasMany(m => m.Claims).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.HasMany(m => m.AllowedCorsOrigins).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.HasMany(m => m.AllowedGrantTypes).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.HasMany(m => m.AllowedScopes).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.HasMany(m => m.ClientSecrets).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.HasMany(m => m.IdentityProviderRestrictions).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.HasMany(m => m.PostLogoutRedirectUris).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.HasMany(m => m.RedirectUris).WithOne().HasForeignKey(m => m.ClientId).IsRequired();
                clients.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientClaim>(clientClaims =>
            {
                clientClaims.HasKey(m => m.Id);
                clientClaims.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientCorsOrigin>(clientCorsOrigins =>
            {
                clientCorsOrigins.HasKey(m => m.Id);
                clientCorsOrigins.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientGrantType>(clientGrantTypes =>
            {
                clientGrantTypes.HasKey(m => m.Id);
                clientGrantTypes.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientIdpRestriction>(clientIdpRescritions =>
            {
                clientIdpRescritions.HasKey(m => m.Id);
                clientIdpRescritions.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientPostLogoutRedirectUri>(clientPostLogoutRedirectUris =>
            {
                clientPostLogoutRedirectUris.HasKey(m => m.Id);
                clientPostLogoutRedirectUris.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientRedirectUri>(clientRedirectUris =>
            {
                clientRedirectUris.HasKey(m => m.Id);
                clientRedirectUris.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientScope>(clientScopes =>
            {
                clientScopes.HasKey(m => m.Id);
                clientScopes.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerClientSecret>(clientSecrets =>
            {
                clientSecrets.HasKey(m => m.Id);
                clientSecrets.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerIdentityResource>(identityResources =>
            {
                identityResources.HasKey(m => m.Id);
                identityResources.Ignore(m => m.State);
                identityResources.HasMany(m => m.Claims).WithOne().HasForeignKey(m => m.IdentityResourceId).IsRequired();
                identityResources.HasMany(m => m.Properties).WithOne().HasForeignKey(m => m.IdentityResourceId).IsRequired();
            });

            modelBuilder.Entity<IdentityServerIdentityResourceClaim>(resourceClaims =>
            {
                resourceClaims.HasKey(m => m.Id);
                resourceClaims.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerIdentityResourceProperty>(properties =>
            {
                properties.HasKey(m => m.Id);
                properties.Ignore(m => m.State);
            });

            modelBuilder.Entity<IdentityServerPersistedGrant>(persistedGrants =>
            {
                persistedGrants.HasKey(m => m.Id);
                persistedGrants.Ignore(m => m.State);
            });

        }

        public Task<int> SaveAnnotatedGraphAsync(IObjectWithState objectGraph)
        {
            ChangeTracker.TrackGraph(
                objectGraph,
                n =>
                {
                    var entity = (IObjectWithState)n.Entry.Entity;
                    n.Entry.State = entity.State == ObjectState.Added
                        ? EntityState.Added
                        : entity.State == ObjectState.Modified
                            ? EntityState.Modified
                            : entity.State == ObjectState.Deleted
                                ? EntityState.Deleted
                                : EntityState.Unchanged;
                });

            return SaveChangesAsync();
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

