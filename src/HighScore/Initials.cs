namespace HighScore;

public record struct Initials(char initial1, char initial2, char initial3)
{
    char Initial1 = initial1;
    char Initial2 = initial2;
    char Initial3 = initial3;

    public static bool TryParse(string? str, out Initials initials)
    {
        initials = default;

        if (str is null)
        {
            return false;
        }

        if (str.Length > 3)
        {
            return false;
        }

        str = str.ToUpper();

        if (str.Any(character => !char.IsLetter(character)))
        {
            return false;
        }

        initials = new Initials(str[0], str[1], str[2]);

        return true;
    }

    public override string ToString() => new string([Initial1, Initial2, Initial3]);
}
