namespace MinshpWebApp.Api.Utils
{
    public static class StringFormatting
    {

        public static string Capitalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.ToLower();
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
