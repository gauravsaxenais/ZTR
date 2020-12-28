namespace ZTR.Framework.Business.Models
{
    public static class ExtensionMethods
    {
        public static string RemoveNewline(this string input)
        {
            return input.Replace("\r", string.Empty);
        }
    }
}
