namespace ETA.Integrator.Server.Helpers.Enums
{
    public enum InvoiceStatus
    {
        Cancelled = -3,
        NotValid = -2,
        Rejected = -1,
        NotSubmitted = 0,
        Submitted = 1, // InProgress
        Valid = 2
    }

    public enum ClientType
    {
        Consumer = 1,
        ConsumerAuth = 2,
        Provider = 3
    }
}
