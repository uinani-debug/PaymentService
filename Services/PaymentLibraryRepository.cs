﻿using AccountLibrary.API.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dapper;
using System.Data;

using AccountLibrary.API.Models;
using Microsoft.Data.SqlClient;
using PaymentService.API.Models;

namespace AccountLibrary.API.Services
{
    public class PaymentLibraryRepository : IPaymentLibraryRepository, IDisposable
    {

        private readonly string connstring;
        private IDbConnection Connection => new SqlConnection(connstring);
        public PaymentLibraryRepository()
        {

            //  connstring = "Server=192.168.0.164;Database=maecbsdb;user=SA;Password=TCSuser1123;Trusted_Connection=True;";
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
                var x = c.Query<AccountTransaction>(query,p);
                                
                c.Close();
                return true;
            }           
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
