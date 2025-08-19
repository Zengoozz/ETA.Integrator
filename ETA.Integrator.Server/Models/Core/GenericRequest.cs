using ETA.Integrator.Server.Helpers.Enums;
using RestSharp;

namespace ETA.Integrator.Server.Models.Core
{
    public class GenericRequest
    {
        public RestRequest Request { get; set; } = new();
        public ClientType ClientType { get; set; }
        public bool DoRetry { get
            {
                if (ClientType == ClientType.Consumer)
                    return true;

                return false;
            } 
        }

    }
}
