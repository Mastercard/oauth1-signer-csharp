using System;

namespace Mastercard.Developer.OAuth1Signer.Core.Utils
{
    /// <summary>
    /// A class adding the Uri.HexEscape implementation (only supported starting from .NET Standard 2.0).
    /// See: https://docs.microsoft.com/en-us/dotnet/api/system.uri.hexescape
    /// </summary>
    internal static class UriUtils
    {
        private static readonly char[] HexUpperChars = {
            '0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        internal static string HexEscape(char character)
        {
            if (character > '\xff')
            {
                throw new ArgumentOutOfRangeException(nameof(character));
            }
            var chars = new char[3];
            var pos = 0;
            EscapeAsciiChar(character, chars, ref pos);
            return new string(chars);
        }

        private static void EscapeAsciiChar(char ch, char[] to, ref int pos)
        {
            to[pos++] = '%';
            to[pos++] = HexUpperChars[(ch & 0xf0) >> 4];
            to[pos++] = HexUpperChars[ch & 0xf];
        }
    }
}
