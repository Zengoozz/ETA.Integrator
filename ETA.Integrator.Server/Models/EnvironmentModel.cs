namespace HMS.Core.Models.ETA
{
    public class EnvironmentModel
    {
        public string? getClientId
        {
            get
            {

                return Values?.FirstOrDefault(v => v.Key == "clientId")?.Value;
            }
        }
        public string? getClientSecret
        {
            get
            {

                return Values?.FirstOrDefault(v => v.Key == "clientSecret")?.Value;
            }
        }
        public required string Name { get; set; }
        public required List<EnvironmentVariableModel> Values { get; set; }
    }

    public class EnvironmentVariableModel
    {
        public required string Key { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
