using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider;

namespace Rinsen.Messaging
{
    public class MessageDbContext : DbContext
    {
        public MessageDbContext(DbContextOptions<MessageDbContext> dbContextOptions)
            :base(dbContextOptions)
        {

        }
        public DbSet<Message> Messages { get; set; }

        public DbSet<HandledMessage> HandledMessages { get; set; }

        public DbSet<Identity> Identities { get; set; }

        public DbSet<PreferedMessageType> PreferedMessageTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageIdentity>()
                .HasKey(m => m.IdentityId);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<PreferedMessageType>()
                .HasKey(m => new { m.IdentityId, m.Type });

            modelBuilder.Entity<HandledMessage>()
                .HasKey(m => new { m.MessageId, m.Sent } );

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(i => i.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .HasPrincipalKey(i => i.IdentityId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(i => i.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .HasPrincipalKey(i => i.IdentityId);

            modelBuilder.Entity<Message>()
               .HasMany(m => m.HandledMessages)
               .WithOne(i => i.Message)
               .HasForeignKey(m => m.MessageId)
               .HasPrincipalKey(i => i.Id);


        }
    }
}
