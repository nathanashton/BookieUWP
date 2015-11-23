using System.Text.RegularExpressions;

namespace Bookie.Domain
{
    public static class PdfIsbnParser
    {
        private static string _isbn = string.Empty;

        public static string FindIsbn(string text)
        {
            var rFileIsbn = Regex.Match(text, @"ISBN.*?([X\d\-_ .]{10,20})");
            if (!rFileIsbn.Success)
            {
                return null;
            }
            _isbn = rFileIsbn.Groups[1].ToString();
            _isbn = _isbn.Replace(".", string.Empty);
            _isbn = _isbn.Replace(" ", string.Empty);
            _isbn = _isbn.Replace("-", string.Empty);
            _isbn = _isbn.Replace("_", string.Empty);
            return _isbn;
        }
    }
}