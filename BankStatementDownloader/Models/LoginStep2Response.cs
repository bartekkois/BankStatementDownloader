using Newtonsoft.Json;
using System.Collections.Generic;

namespace BankStatementDownloader.Models
{
    public class LoginStep2Response
    {
        public string RedirectURL { get; set; }
        public string Status { get; set; }
        public string AuthPairStatusCheckInterval { get; set; }
        public string InCorrectReason { get; set; }

        [JsonIgnore]
        public List<object> Params { get; set; }
    }
}
