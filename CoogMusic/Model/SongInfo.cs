using System;
using System.ComponentModel.DataAnnotations;

using CoogMusic.Pages;

public class SongInfo
{
    public int? songId;
    public int? userId;
    public int artistId;
    public String? RecordLabel;
    public String? CreateDate;
    public String? Name;
    public String? artist;
    public String? genre;
    public String? title;
    public Boolean? deleted;
    public Boolean? Explicit;
    public TimeSpan? Duration;
    public IFormFile? songFile;
}

public class SongView
{
    public int? songId;
    public int? userId;
    public int? artistId;
    public String? RecordLabel;
    public String? CreateDate;
    public String? artistName;
    public String? artist;
    public String? genre;
    public String? title;
    public Boolean? deleted;
    public byte[]? trackBytes;
}

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