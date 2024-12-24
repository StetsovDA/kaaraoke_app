//using Microsoft.EntityFrameworkCore;

//public class Track
//{
//    public string AudioFilePath { get; set; }
//    public string LrcFilePath { get; set; }
//    public string Title { get; set; }
//    public byte[] AlbumCover { get; set; }
//}

//public class LRCLine
//{
//    public int Time { get; set; }
//    public string Text { get; set; }
//}

//public class MusicDbContext : DbContext
//{
//    public DbSet<Track> Tracks { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        optionsBuilder.UseSqlServer("YourConnectionStringHere");
//    }
//}
