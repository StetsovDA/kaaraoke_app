using System.Data.Entity;

public class KaraokeContext : DbContext
{
    public DbSet<AudioFile> AudioFiles { get; set; }
    public DbSet<AlbumCover> AlbumCovers { get; set; }
    public DbSet<LRCFile> LRCFiles { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AudioFile>()
            .HasMany(a => a.LRCFiles)
            .WithRequired()
            .WillCascadeOnDelete(false);
        
    }

}
