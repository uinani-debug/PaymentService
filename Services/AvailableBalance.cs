namespace AccountLibrary.API.Services
{
    public partial class PaymentLibraryRepository
    {
        public class AvailableBalance
        {
            public string Currency { get; set; } = "GBP";
            public double Amount { get; set; }
        }
    }
}
