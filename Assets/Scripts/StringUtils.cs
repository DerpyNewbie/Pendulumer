public static class StringUtils
{
    public static string ToOrdinal(int num)
    {
        if (num <= 0) return num.ToString();
        if (num % 100 is 11 or 12 or 13) return num + "th";

        return (num % 10) switch
        {
            1 => num + "st",
            2 => num + "nd",
            3 => num + "rd",
            _ => num + "th"
        };
    }
}