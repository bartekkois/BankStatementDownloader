using BankStatementDownloader.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BankStatementDownloader.Services
{
    public interface IBankAccountClient
    {
        Task<LoginStep2Response> Login();
        Task<BankAccountsList> GetBankAccountsList(string accountId);
        Task<BankStatementsList> GetBankStatementsList(string accountId, DateTime dateFrom, DateTime dateTo, bool fetchAll);
        Task<FileContentResult> DownloadBankStatement(string formatType, string contentDisposition, string language, int statementId, bool defaultFlag, string extendedFormatType);
        Task Logout();
    }
}