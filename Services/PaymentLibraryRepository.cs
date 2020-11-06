using AccountLibrary.API.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dapper;
using System.Data;

using AccountLibrary.API.Models;
using Microsoft.Data.SqlClient;
using PaymentService.API.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace AccountLibrary.API.Services
{
    public partial class PaymentLibraryRepository : IPaymentLibraryRepository, IDisposable
    {

        private readonly string connstring;
        private readonly string serviceURL;
        private IDbConnection Connection => new SqlConnection(connstring);
        public PaymentLibraryRepository()
        {
            serviceURL = "http://mae-poc-1562794669.us-east-2.elb.amazonaws.com/accountDetailsService/api/v1/getAccountDetails?AccountNumber=";
            connstring = "Server=192.168.0.164;Database=maecbsdb;user=SA;Password=TCSuser1123;";
        }

        public bool CreditAmount(PaymentRequest req)
        {
            using (var c = Connection)
            {
                c.Open();
                var p = new DynamicParameters();
                p.Add("accountNumber", req.AccountIdentifier, DbType.String, ParameterDirection.Input);

                string query = "select * from vw_transactiondetails where account_identifier= @accountNumber order by transaction_date desc";
                var x = c.Query<AccountTransaction>(query, p);

                c.Close();
                return true;
            }
        }
        public async System.Threading.Tasks.Task<double> GetAccountDetailsAsync(string accountNumber)
        {
            using var client = new HttpClient();
            var url = serviceURL + accountNumber;
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var resp = await response.Content.ReadAsStringAsync();
            var accountdetailsresponse = JsonConvert.DeserializeObject<AccountDetails>(resp);

            return accountdetailsresponse.AvailableBalance.Amount;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
