using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Lab3_NhatNguyen_WebApp.Models;

namespace Lab3_NhatNguyen_WebApp.Services
{
    public class AmazonS3Management
    {
        public static AmazonS3Client client;
        public static void InitializeClient(string accessKey,string secretKey)
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.CACentral1);
        }
        public static async Task<bool> UploadFile(IFormFile file,string fileName)
        {
            if (file != null && file.Length > 0)
            {
                var bucketName = "moviesdatabucket";
                var newFileName = fileName + Path.GetExtension(file.FileName);
                var keyName = newFileName; // Specify the folder and new file name in the bucket
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var fileTransferUtility = new TransferUtility(AmazonS3Management.client);
                        await fileTransferUtility.UploadAsync(stream, bucketName, keyName);
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        public static async Task<bool> DeleteObject(string key)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = "moviesdatabucket",
                    Key = key
                };

                var response = await client.DeleteObjectAsync(deleteObjectRequest);

                // Check the response for success
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    // Object deleted successfully
                    return true;
                }
                else
                {
                    // Object deletion failed
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
