namespace sts_ai_support.Models
{
    public class AzureOpenAISettings
    {
        public required string ApiKey {  get; set; }
        public required string DeploymentName { get; set; }
        public required string Endpoint { get; set; }
    }
}
