namespace BankStatementDownloader.Models
{
    public class BankStatement
    {
        public int StatementId { get; set; }
        public double DateFrom { get; set; }
        public double DateTo { get; set; }
    }
}