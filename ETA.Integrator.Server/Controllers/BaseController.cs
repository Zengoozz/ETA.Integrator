﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;

namespace ETA.Integrator.Server.Controllers
{
    public class BaseController : ControllerBase
    {
        readonly RestClient _client;
        private readonly CustomConfigurations _customConfig;
        public BaseController(
            IOptions<CustomConfigurations> customConfigurations
            )
        {
            _customConfig = customConfigurations.Value;

            if (_customConfig.API_URL != null)
            {
                var opt = new RestClientOptions(_customConfig.API_URL);
                _client = new RestClient(opt);
            }
            else
            {
                throw new Exception("Error: Getting connection string");
            }
        }
    }
}
