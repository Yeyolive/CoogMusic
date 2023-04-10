using System;
using System.ComponentModel.DataAnnotations;

using CoogMusic.Model;

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
    public Boolean? Explicit;
    public TimeSpan? Duration;
    public Boolean? deleted;
    public byte[]? trackBytes;
}