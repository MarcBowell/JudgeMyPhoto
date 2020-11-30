namespace Marcware.JudgeMyPhoto.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static string EmptyIfNull(this string text)
        {
            if (text == null)
                return string.Empty;
            else
                return text;
        }
    }
}
