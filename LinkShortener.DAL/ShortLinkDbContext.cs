using LinkShortener.Domain.Models;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace LinkShortener.DAL
{
    public partial class ShortLinkDbContext : DbContext
    {
        public ShortLinkDbContext()
        {
        }

        public ShortLinkDbContext(DbContextOptions<ShortLinkDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Link> Links { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("server=localhost;user=root;password=123123123;database=shortlinkdb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>(entity =>
            {
                entity.ToTable("links");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CountClick).HasColumnType("int(11)");

                entity.Property(e => e.DateCreate).HasColumnType("date");

                entity.Property(e => e.LongUrl)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.ShortUrl)
                    .IsRequired()
                    .HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
