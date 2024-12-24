using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AudioFile
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string Title { get; set; }
    public virtual ICollection<LRCFile> LRCFiles { get; set; }
}

public class AlbumCover
{
    public int Id { get; set; }
    public string ImagePath { get; set; }
    public virtual AudioFile AudioFile { get; set; }
}

public class LRCFile
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public virtual AudioFile AudioFile { get; set; }
    public int AudioFileId { get; internal set; }
}
