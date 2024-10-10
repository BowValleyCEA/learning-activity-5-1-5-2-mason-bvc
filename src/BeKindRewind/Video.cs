namespace BeKindRewind;

public record struct Video
{
    public required string Name;
    public required string Genre;
    public required TimeSpan Duration;
    public required DateTime ReleaseDate;

    public override readonly int GetHashCode() => Util.Djb2(Name) ^ Duration.GetHashCode() ^ ReleaseDate.GetHashCode();
}
