namespace ETA.Integrator.Server.Helpers.Enums
{
    public enum InvoiceStatus
    {
        Rejected = -1,
        NotSubmitted = 0,
        Submitted = 1,
        Success = 2
    }

    public enum ClientType
    {
        Consumer = 1,
        ConsumerAuth = 2,
        Provider = 3
    }
}
