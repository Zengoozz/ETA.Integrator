using ETA.Integrator.Server.Interface;
using HMS.Core.Models.ETA;
using Newtonsoft.Json;

namespace ETA.Integrator.Server.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private string CheckIfFileExit(string fileName)
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = baseDirectory + fileName;

                bool isFileExist = File.Exists(filePath);

                return isFileExist ? filePath : "";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EnvironmentModel? GetETAConfig()
        {
            try
            {
                var filePath = CheckIfFileExit("ETAConfiguration.json");

                if (filePath != "")
                {
                    string json = File.ReadAllText(filePath);

                    EnvironmentModel? environmentVariables = JsonConvert.DeserializeObject<EnvironmentModel>(json);

                    return environmentVariables;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SetETAConfig(string clientId = "", string clientSecret = "", string generatedToken = "")
        { // RESTRICTION: No Empty Values can be added even if it's the real value -- ZENGO
            try
            {
                var filePath = CheckIfFileExit("ETAConfiguration.json");

                if (filePath != "")
                {
                    var latestConfig = GetETAConfig();

                    if (latestConfig != null)
                    {
                        latestConfig.Values.ForEach(row =>
                        {
                            if (row.Key == "clientId" && clientId != "")
                                row.Value = clientId;

                            if (row.Key == "clientSecret" && clientSecret != "")
                                row.Value = clientSecret;

                            if (row.Key == "generatedAccessToken" && generatedToken != "")
                                row.Value = generatedToken;
                        });
                    }
                    else
                    {
                        throw new Exception(message: "No Config File Exist");
                    }

                    string json = JsonConvert.SerializeObject(latestConfig);


                    File.WriteAllText(filePath, json);

                    return true;
                }

                //Error File doesn't exist 
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
