using System.Collections.Generic;

namespace BankStatementDownloader.Models
{
    public class LoginStep1Response
    {
        public string Method { get; set; }
        public string TransportEncryptionsType { get; set; }
        public string EdAes { get; set; }
        public bool EncAes { get; set; }
        public int FieldCount { get; set; }
        public List<int> MaskedFieldIndexSet { get; set; }
        public string MaskIn01Format { get; set; }
    }
}
