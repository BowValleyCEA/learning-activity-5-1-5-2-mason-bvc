namespace BeKindRewind;

public record struct Customer
{
    public required string FullName;
    public required string EmailAddress;
    public override readonly int GetHashCode() => Util.Djb2(FullName) ^ Util.Djb2(EmailAddress);
}
