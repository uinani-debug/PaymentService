using AccountLibrary.API.Entities;
using PaymentService.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace AccountLibrary.API.Services
{
    public interface IPaymentLibraryRepository
    {       
       bool CreditAmount(PaymentRequest accountNumber);
    //    bool DebitAmount(PaymentRequest accountNumber);

    }
}
