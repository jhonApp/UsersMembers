using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using System.Text.Json;

namespace UsersMembers.Infrastructure.Serverless
{
    public class DynamoDbStreamArchiver
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public DynamoDbStreamArchiver()
        {
            _s3Client = new AmazonS3Client();
            _bucketName = Environment.GetEnvironmentVariable("ARCHIVE_BUCKET_NAME") ?? "user-members-archive";
        }

        public DynamoDbStreamArchiver(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
            _bucketName = Environment.GetEnvironmentVariable("ARCHIVE_BUCKET_NAME") ?? "user-members-archive";
        }

        public async Task FunctionHandler(DynamoDBEvent evnt, ILambdaContext context)
        {
            foreach (var record in evnt.Records)
            {
                // We only care about REMOVE events (TTL expirations or manual deletions)
                if (record.EventName == "REMOVE")
                {
                    var oldImage = record.Dynamodb.OldImage;
                    
                    // Convert DynamoDB JSON format to standard JSON
                    var jsonString = ConvertToJson(oldImage);

                    // Create a unique key for S3
                    // Structure: year/month/day/id.json
                    string objectKey = GenerateObjectKey(oldImage);

                    try
                    {
                        var putRequest = new PutObjectRequest
                        {
                            BucketName = _bucketName,
                            Key = objectKey,
                            ContentBody = jsonString,
                            ContentType = "application/json"
                        };

                        await _s3Client.PutObjectAsync(putRequest);
                        context.Logger.LogInformation($"Archived item to {_bucketName}/{objectKey}");
                    }
                    catch (Exception e)
                    {
                        context.Logger.LogError($"Failed to archive item: {e.Message}");
                        throw;
                    }
                }
            }
        }

        private string ConvertToJson(Dictionary<string, DynamoDBEvent.AttributeValue> dynamoItem)
        {
            // Simplified conversion. In a real app, use a robust converter or a library method if available.
            // For now, serializing the dictionary of AttributeValues gives us a structure like {"Key": {"S": "Value"}}.
            // This is acceptable for archival compliance often, but ideally we'd unmarshall it.
            return JsonSerializer.Serialize(dynamoItem);
        }

        private string GenerateObjectKey(Dictionary<string, DynamoDBEvent.AttributeValue> item)
        {
            string id = "unknown_id";
            if (item.ContainsKey("Id") && item["Id"].S != null)
            {
                id = item["Id"].S;
            }

            var now = DateTime.UtcNow;
            return $"{now.Year}/{now.Month:D2}/{now.Day:D2}/{id}_{now.Ticks}.json";
        }
    }
}
