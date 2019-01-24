using System;
using System.Linq;
using System.Threading.Tasks;
using BankStatementDownloader.Models;
using BankStatementDownloader.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace BankStatementDownloader.Pages
{
    public class IndexModel : PageModel
    {
        public BankStatementsList BankStatementsList;
        public string ExceptionMessage { get; set; }

        public async Task<IActionResult> OnGet([FromServices]BankAccountClient client, [FromServices]IConfiguration configuration, int numberOfDays=14)
        {
            try
            {
                if ((await client.Login()).Status != "CREDENTIALS_CORRECT")
                    throw new Exception("Błąd logowania do systemu bankowego");

                var bankAccountsList = await client.GetBankAccountsList(configuration["BankApi:Username"], false, 1, 10);
                BankStatementsList = await client.GetBankStatementsList(bankAccountsList.Content.First(s => s.AccountNo == configuration["BankApi:AccountNo"]).AccountId, DateTime.Now.AddDays(numberOfDays * -1), DateTime.Now, false);
                await client.Logout();

                return Page();
            }
            catch(Exception ex)
            {
                ExceptionMessage = ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnGetDownloadMt940BankStatement([FromServices]BankAccountClient client, [FromServices]IConfiguration configuration, int statementId, double dateFrom)
        {
            try
            {
                if ((await client.Login()).Status != "CREDENTIALS_CORRECT")
                    throw new Exception("Błąd logowania do systemu bankowego");

                var bankStatement = await client.DownloadBankStatement("pdf", "download", "pl", statementId, false, "MT940");
                var bankStatementDateFrom = Utils.Utils.UnixTimeStampToLocalDateTime(dateFrom);
                await client.Logout();

                return File(bankStatement.FileContents, bankStatement.ContentType, "wyciag_" + bankStatementDateFrom.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt");
            }
            catch (Exception ex)
            {
                ExceptionMessage = ex.Message;
                return Page();
            }
        }
    }
}
