using BankStatementDownloader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BankStatementDownloader.Services
{
    public class BankAccountClient
    {
        private HttpClient _client;
        private IConfiguration _configuration;
        private ILogger<BankAccountClient> _logger;
        private string _xsrfToken;

        public BankAccountClient(HttpClient client,  IConfiguration configuration, ILogger<BankAccountClient> logger)
        {
            _configuration = configuration;
            _client = client;
            _logger = logger;
        }

        public async Task<LoginStep2Response> Login()
        {
            try
            {
                // Login step: 1to2
                var loginStep1Response = await _client.PostAsync($"/frontend-web/app/j_spring_security_check?step=1to2&j_username={_configuration["BankApi:Username"]}", null);
                loginStep1Response.EnsureSuccessStatusCode();

                // Login step: 2to2
                string maskedPassword = Utils.Utils.MaskPassword(_configuration["BankApi:Password"], JsonConvert.DeserializeObject<LoginStep1Response>(await loginStep1Response.Content.ReadAsStringAsync()).MaskIn01Format);
                var loginStep2Response = await _client.PostAsync($"frontend-web/app/j_spring_security_check?step=2&j_username={_configuration["BankApi:Username"]}&j_password={maskedPassword}", null);
                loginStep2Response.EnsureSuccessStatusCode();

                _xsrfToken = new Regex("XSRF-TOKEN=(.*); Path=/").Match(loginStep2Response.Headers.GetValues("Set-Cookie").First(s => s.Contains("XSRF-TOKEN"))).Groups[1].ToString();
                _client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", _xsrfToken);

                return JsonConvert.DeserializeObject<LoginStep2Response>(await loginStep2Response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                var exceptionMessage = $"An error occured during login process: {ex.Message}";
                _logger.LogError(exceptionMessage);

                throw new Exception(exceptionMessage);
            }
        }

        public async Task<BankAccountsList> GetBankAccountsList(string accountId, bool includeLimitedAccess, int pageNumber, int pageSize)
        {
            try
            {
                var bankAccountsListResponse = await _client.GetAsync($"/frontend-web/api/account?customerId={_configuration["BankApi:Username"]}&includeLimitedAccess=false&pageNumber={pageNumber}&pageSize={pageSize}");
                bankAccountsListResponse.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<BankAccountsList>(await bankAccountsListResponse.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                var exceptionMessage = $"An error occured during getting bank accounts list process {ex.Message}";
                _logger.LogError(exceptionMessage);

                throw new Exception(exceptionMessage);
            }
        }

        public async Task<BankStatementsList> GetBankStatementsList(string accountId, DateTime dateFrom, DateTime dateTo, bool fetchAll)
        {
            try
            {
                var bankStatementsListResponse = await _client.GetAsync($"/frontend-web/api/account_statement?accountId={accountId}&customerId={_configuration["BankApi:Username"]}&dateFrom={dateFrom.ToString("yyyy-MM-dd")}&dateTo={dateTo.ToString("yyyy-MM-dd")}&fetchAll={fetchAll}");
                bankStatementsListResponse.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<BankStatementsList>(await bankStatementsListResponse.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                var exceptionMessage = $"An error occured during getting bank statements list process {ex.Message}";
                _logger.LogError(exceptionMessage);

                throw new Exception(exceptionMessage);
            }
        }

        public async Task<FileContentResult> DownloadBankStatement(string formatType, string contentDisposition, string language, int statementId, bool defaultFlag, string extendedFormatType)
        {
            try
            {
                var bankStatementResponse = await _client.GetAsync($"/frontend-web/api/account_statement/downloads/download_statement_list_report.json?formatType={formatType}&contentDisposition={contentDisposition}&language={language}&statementId={statementId}&default={defaultFlag}&extendedFormatType={extendedFormatType}");
                bankStatementResponse.EnsureSuccessStatusCode();

                return new FileContentResult(await bankStatementResponse.Content.ReadAsByteArrayAsync(), bankStatementResponse.Content.Headers.ContentType.MediaType);
            }
            catch (Exception ex)
            {
                var exceptionMessage = $"An error occured during bank statement download process {ex.Message}";
                _logger.LogError(exceptionMessage);

                throw new Exception(exceptionMessage);
            }
        }

        public async Task Logout()
        {
            try
            {
                var bankStatementsListResponse = await _client.GetAsync($"/frontend-web/app/logout.html?X-XSRF-TOKEN={_xsrfToken}");
                bankStatementsListResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                var exceptionMessage = $"An error occured during logout process {ex.Message}";
                _logger.LogError(exceptionMessage);

                throw new Exception(exceptionMessage);
            }
        }
    }
}
