namespace BeKindRewind;

internal static class Util
{
    // http://www.cse.yorku.ca/~oz/hash.html
    public static int Djb2(string s)
    {
        int hash = 5381;

        foreach (char c in s)
        {
            hash = ((hash << 5) + hash) + c; /* hash * 33 + c */
        }

        return hash;
    }
}
