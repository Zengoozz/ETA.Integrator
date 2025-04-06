namespace ETA.Integrator.Server.Models.ETA
{
    public class InvoiceLineModel
    {
        public string Description { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;// "GS1" or "EGS"
        public string ItemCode { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty; // e.g., "kg"
        public decimal Quantity { get; set; }
        public ValueModel UnitValue { get; set; } = new ValueModel();
        public decimal SalesTotal { get; set; }
        public decimal Total { get; set; }
        public decimal ValueDifference { get; set; }
        public decimal TotalTaxableFees { get; set; }
        public decimal NetTotal { get; set; }
        public decimal ItemsDiscount { get; set; }
        public DiscountModel Discount { get; set; } = new DiscountModel();
        public List<TaxableItemModel> TaxableItems { get; set; } = new List<TaxableItemModel>();
        public required string InternalCode { get; set; }
    }
}
