using ETA.Integrator.Server.Helpers.Enums;
using RestSharp;

namespace ETA.Integrator.Server.Models.Core
{
    public class GenericRequest
    {
        public RestRequest Request { get; set; } = new();
        public ClientType ClientType { get; set; }

    }
}
