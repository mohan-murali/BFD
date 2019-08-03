﻿using BFD.Common;
using BFD.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BFD.Proxy
{
    public class AccountSummaryApiConfiguration
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string Version { get; set; }

    }

    public class AccountDetailProxy
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<AccountSummaryApiConfiguration> _accountSummaryApiConfiguration;

        public AccountDetailProxy(IOptions<AccountSummaryApiConfiguration> accountSummaryApiConfiguration, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _accountSummaryApiConfiguration = accountSummaryApiConfiguration;
        }

        public async Task<IEnumerable<AccountBalance>> GetAccountSummary()
        {
            List<AccountBalance> accountBalances = new List<AccountBalance>();
            DBSAccountsSummary dBSAccountsSummary = new DBSAccountsSummary();

            var request = new HttpRequestMessage(HttpMethod.Get, _accountSummaryApiConfiguration.Value.Url);
            request.Headers.Add("clientId", _accountSummaryApiConfiguration.Value.ClientId);
            request.Headers.Add("accessToken", _accountSummaryApiConfiguration.Value.AccessToken);
            request.Headers.Add("version", _accountSummaryApiConfiguration.Value.Version);
            request.Headers.Add("User-Agent", "Business Finance Dashboard");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                dBSAccountsSummary = await response.Content.ReadAsAsync<DBSAccountsSummary>();
            }

            if (dBSAccountsSummary.SavingsAccounts != null)
            {
                foreach (DBSAccountSummary dbsAccountSummary in dBSAccountsSummary.SavingsAccounts)
                {
                    accountBalances.Add(new AccountBalance
                    {
                        AccountType = Constants.AccountType.Savings,
                        BankName = "DBS",
                        AccountNumber = dbsAccountSummary.AccountNumber,
                        Balance = dbsAccountSummary.Balances?.AvailableBalance?.Amount ?? 0
                    });
                }
            }

            if (dBSAccountsSummary.CurrentAccounts != null)
            {
                foreach (DBSAccountSummary dbsAccountSummary in dBSAccountsSummary.CurrentAccounts)
                {
                    accountBalances.Add(new AccountBalance
                    {
                        AccountType = Constants.AccountType.Current,
                        BankName = "DBS",
                        AccountNumber = dbsAccountSummary.AccountNumber,
                        Balance = dbsAccountSummary.Balances?.AvailableBalance?.Amount ?? 0
                    }                    );
                }
            }
            return accountBalances;
        }
        
        public List<AccountBalance> GetAccountBalances()
        {
            return new List<AccountBalance>
            {
                new AccountBalance { AccountNumber= "243158", BankName= "DBS", Month = "Jan", Balance= 2000},
                new AccountBalance { AccountNumber= "123157", BankName= "CBA", Month = "Jan", Balance= 3300},
                new AccountBalance { AccountNumber= "756213", BankName= "ING", Month = "Jan", Balance= 5535},
                new AccountBalance { AccountNumber= "354134", BankName= "HDFC", Month = "Jan", Balance= 1235},
                new AccountBalance { AccountNumber= "457841", BankName= "DBS", Month = "Feb", Balance= 1800},
                new AccountBalance { AccountNumber= "321121", BankName= "CBA", Month = "Feb", Balance= 3500},
                new AccountBalance { AccountNumber= "535434", BankName= "ING", Month = "Feb", Balance= 5500},
                new AccountBalance { AccountNumber= "897653", BankName= "HDFC", Month = "Feb", Balance= 1200},
                new AccountBalance { AccountNumber= "843231", BankName= "DBS", Month = "Mar", Balance= 2200},
                new AccountBalance { AccountNumber= "673312", BankName= "CBA", Month = "Mar", Balance= 2700},
                new AccountBalance { AccountNumber= "975624", BankName= "ING", Month = "Mar", Balance= 5000},
                new AccountBalance { AccountNumber= "453211", BankName= "HDFC", Month = "Mar", Balance= 1250},
                new AccountBalance { AccountNumber= "665422", BankName= "DBS", Month = "Apr", Balance= 2345},
                new AccountBalance { AccountNumber= "362421", BankName= "CBA", Month = "Apr", Balance= 2500},
                new AccountBalance { AccountNumber= "785544", BankName= "ING", Month = "Apr", Balance= 4815},
                new AccountBalance { AccountNumber= "853246", BankName= "HDFC", Month = "Apr", Balance= 1800}
            };
        }
    }

}
