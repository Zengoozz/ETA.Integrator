namespace ETA.Integrator.Server.Models.Consumer.ETA
{
    public class PaymentModel
    {
        public string BankName { get; set; } = string.Empty;
        public string BankAddress { get; set; } = string.Empty;
        public string BankAccountNo { get; set; } = string.Empty;
        public string BankAccountIBAN { get; set; } = string.Empty;
        public string SwiftCode { get; set; } = string.Empty;
        public string Terms { get; set; } = string.Empty;
    }
}
