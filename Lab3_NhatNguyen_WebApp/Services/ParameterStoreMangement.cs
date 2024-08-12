using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace Lab3_NhatNguyen_WebApp.Services
{
    public static class ParameterStoreMangement
    {
        public static string DBUsername { get; set; }
        public static string DBPassword { get; set; }
        public static AmazonSimpleSystemsManagementClient client;
        public async static void GetDBParameter(string accessKey,string secretKey)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            client = new AmazonSimpleSystemsManagementClient(credentials,Amazon.RegionEndpoint.CACentral1);
            var request = new GetParameterRequest()
            {
                Name = "DBUsername",
                WithDecryption = true
            };
            var result = await client.GetParameterAsync(request);
            DBUsername = result.Parameter.Value;
            request = new GetParameterRequest()
            {
                Name = "DBPassword",
                WithDecryption = true
            };
            result = await client.GetParameterAsync(request);
            DBPassword = result.Parameter.Value;
        }
    }
}
