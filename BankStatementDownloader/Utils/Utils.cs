using System;
using System.Linq;
using System.Text;

namespace BankStatementDownloader.Utils
{
    public static class Utils
    {
        public static string MaskPassword(string unmaskedPassword, string maskIn01Format)
        {
            var maskedPassword = new StringBuilder();

            foreach (var bit in maskIn01Format.Select((x, i) => new { Value = x, Index = i }))
                if (bit.Value == '1')
                    maskedPassword.Append(unmaskedPassword[bit.Index]);

            return maskedPassword.ToString();
        }

        public static DateTime UnixTimeStampToLocalDateTime(double unixTimeStamp) => 
            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimeStamp).ToLocalTime();
    }
}
