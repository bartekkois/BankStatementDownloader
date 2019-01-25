using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BankStatementDownloader.Middlewares
{
    public class AllowedIpAddressesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AllowedIpAddressesMiddleware> _logger;
        private readonly string _allowedIpAddressesList;

        public AllowedIpAddressesMiddleware(RequestDelegate next, ILogger<AllowedIpAddressesMiddleware> logger, string allowedIpAddressesList)
        {
            _next = next;
            _logger = logger;
            _allowedIpAddressesList = allowedIpAddressesList;
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            _logger.LogDebug($"Request from remote IP address: {remoteIp}");

            var badIp = true;
            foreach (var address in _allowedIpAddressesList.Split(','))
            {
                var testIp = IPAddress.Parse(address);
                if (testIp.GetAddressBytes().SequenceEqual(remoteIp.GetAddressBytes()))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                _logger.LogInformation($"Forbidden request from remote IP address: {remoteIp}");
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await _next.Invoke(context);
        }
    }
}
