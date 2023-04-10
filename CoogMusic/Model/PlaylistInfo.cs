using System;
using CoogMusic.Pages;

public class PlaylistInfo
{
    public int Id;
    public int UserId;
    public int SongCount;
    public String? Title;
    public String? Description;
    public Boolean? Deleted;
    public TimeSpan? Duration;
}

public class PlaylistSongInfo
{
    public int PlaylistId { get; set; }
    public int SongId { get; set; }
}