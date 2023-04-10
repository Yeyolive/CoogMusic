using System;
using CoogMusic.Pages;

public class AlbumInfo
{
    public int Id { get; set; }
    public int ArtistId { get; set; }
    public int AlbumId { get; set; }
    public int? SongCount { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public TimeSpan? Duration { get; set; }
    public Boolean? deleted;
    public IFormFile? art;

}



