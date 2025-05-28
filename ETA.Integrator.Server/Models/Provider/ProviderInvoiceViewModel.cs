using ETA.Integrator.Server.Models.Consumer.ETA;
using System.ComponentModel.DataAnnotations;

namespace ETA.Integrator.Server.Models.Provider
{
    public class ProviderInvoiceViewModel
    {
        public int InvoiceId { set; get; }
        public string InvoiceType { set; get; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string RegistrationNumber { set; get; } = string.Empty;
        public DateTime CreatedDate { set; get; }
        public ReceiverAddressModel ReceiverAddress { set; get; } = new();
        public string ReceiverName { set; get; } = string.Empty;
        public int ReceiverId { set; get; }
        public bool IsReviewed { set; get; } = false;
        public decimal VatNet { set; get; }
        public decimal NetPrice { set; get; }
        public List<InvoiceLineModel> InvoiceItems { set; get; } = new();
    }
}
