using System;
using System.Collections.Generic;
using System.Text;
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
            modelBuilder.Entity<IdentityServerApiResource>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerApiResource>()
                .HasMany(r => r.ApiSecrets)
                .WithOne()
                .HasForeignKey(s => s.ApiResourceId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerApiResource>()
                .HasMany(r => r.Claims)
                .WithOne()
                .HasForeignKey(s => s.ApiResourceId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerApiResource>()
                .HasMany(r => r.Scopes)
                .WithOne()
                .HasForeignKey(s => s.ApiResourceId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerApiResourceClaim>()
                .HasKey(m => m.Id);
            
            modelBuilder.Entity<IdentityServerApiResourceScope>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerApiResourceScope>()
                .HasMany(m => m.UserClaims)
                .WithOne()
                .HasForeignKey(m => m.ApiResourceScopeId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerApiResourceScopeClaim>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerApiResourceSecret>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClient>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.Claims)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.AllowedCorsOrigins)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.AllowedGrantTypes)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.AllowedScopes)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.ClientSecrets)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.IdentityProviderRestrictions)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.PostLogoutRedirectUris)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClient>()
                .HasMany(m => m.RedirectUris)
                .WithOne()
                .HasForeignKey(m => m.ClientId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerClientClaim>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClientCorsOrigin>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClientGrantType>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClientIdpRestriction>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClientPostLogoutRedirectUri>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClientRedirectUri>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClientScope>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerClientSecret>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerIdentityResource>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerIdentityResource>()
                .HasMany(m => m.Claims)
                .WithOne()
                .HasForeignKey(m => m.IdentityResourceId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerIdentityResource>()
                .HasMany(m => m.Properties)
                .WithOne()
                .HasForeignKey(m => m.IdentityResourceId)
                .IsRequired();

            modelBuilder.Entity<IdentityServerIdentityResourceClaim>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerIdentityResourceProperty>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<IdentityServerPersistedGrant>()
                .HasKey(m => m.Id);

        }
    }
}
