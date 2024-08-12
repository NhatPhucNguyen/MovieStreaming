using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Lab3_NhatNguyen_WebApp.Services
{
    public static class AmazonDynamoDBManagement
    {
        public static AmazonDynamoDBClient client;
        public static void InitializeClient(string accessKey,string secretKey)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
        }
    }
}
