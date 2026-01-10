using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace UsersMembers.Infrastructure.IaC
{
    public class UsersMembersStack : Stack
    {
        public UsersMembersStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var usersTable = new Table(this, "UsersTable", new TableProps
            {
                TableName = "Users",

                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute { Name = "Id", Type = AttributeType.STRING },

                BillingMode = BillingMode.PAY_PER_REQUEST,
                RemovalPolicy = RemovalPolicy.RETAIN 
            });

            // 1. Queue (Entry Point)
            var queue = new Queue(this, "AuditLogQueue", new QueueProps
            {
                QueueName = "AuditLogQueue",
                VisibilityTimeout = Duration.Seconds(45),
                DeadLetterQueue = new DeadLetterQueue
                {
                    MaxReceiveCount = 3,
                    Queue = new Queue(this, "AuditLogDLQ", new QueueProps
                    {
                        QueueName = "AuditLogQueueDLQ"
                    })
                }
            });

            // 2. DynamoDB Table (Hot Storage)
            var table = new Table(this, "AuditLogTable", new TableProps
            {
                TableName = "AuditLogTable",
                PartitionKey = new Amazon.CDK.AWS.DynamoDB.Attribute { Name = "Id", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                TimeToLiveAttribute = "ttl",
                Stream = StreamViewType.NEW_AND_OLD_IMAGES,
                RemovalPolicy = RemovalPolicy.DESTROY // For dev/test. In prod, use RETAIN.
            });

            // 3. S3 Bucket (Archive)
            var bucket = new Bucket(this, "ArchiveBucket", new BucketProps
            {
                BucketName = "users-members-audit-archive",
                EnforceSSL = true,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                LifecycleRules = new ILifecycleRule[]
                {
                    new LifecycleRule
                    {
                        Transitions = new ITransition[]
                        {
                            new Transition
                            {
                                StorageClass = StorageClass.GLACIER_INSTANT_RETRIEVAL,
                                TransitionAfter = Duration.Days(1)
                            }
                        }
                    }
                },
                RemovalPolicy = RemovalPolicy.RETAIN
            });

            // 4. Lambda 1: Ingestion (SQS -> DynamoDB)
            // Use environment variable for CI/CD, fallback to relative path for local dev
            var lambdaBinaryPath = System.Environment.GetEnvironmentVariable("LAMBDA_BINARY_PATH") 
                ?? "../UsersMembers.Infrastructure/bin/Debug/net8.0";

            // 4. Lambda 1: Ingestion (SQS -> DynamoDB)
            var ingestionFunction = new Function(this, "IngestionFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Code = Code.FromAsset(lambdaBinaryPath),
                Handler = "UsersMembers.Infrastructure::UsersMembers.Infrastructure.Serverless.DynamoDbIngestionHandler::FunctionHandler",
                Timeout = Duration.Seconds(30),
                Environment = new Dictionary<string, string>
                {
                    { "DYNAMODB_TABLE_NAME", table.TableName }
                }
            });

            ingestionFunction.AddEventSource(new SqsEventSource(queue, new SqsEventSourceProps
            {
                BatchSize = 10,
                ReportBatchItemFailures = true 
            }));

            queue.GrantConsumeMessages(ingestionFunction);
            table.GrantWriteData(ingestionFunction);

            // 5. Lambda 2: Archiver (DynamoDB Stream -> S3)
            var archivalFunction = new Function(this, "ArchivalFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Code = Code.FromAsset(lambdaBinaryPath),
                Handler = "UsersMembers.Infrastructure::UsersMembers.Infrastructure.Serverless.DynamoDbStreamArchiver::FunctionHandler",
                Timeout = Duration.Seconds(30),
                Environment = new Dictionary<string, string>
                {
                    { "ARCHIVE_BUCKET_NAME", bucket.BucketName }
                }
            });

            archivalFunction.AddEventSource(new DynamoEventSource(table, new DynamoEventSourceProps
            {
                StartingPosition = StartingPosition.TRIM_HORIZON,
                BatchSize = 10,
                RetryAttempts = 2
            }));

            table.GrantStreamRead(archivalFunction);
            bucket.GrantPut(archivalFunction);

            // 6. Outputs
            new CfnOutput(this, "QueueUrl", new CfnOutputProps { Value = queue.QueueUrl });
            new CfnOutput(this, "TableName", new CfnOutputProps { Value = table.TableName });
            new CfnOutput(this, "BucketName", new CfnOutputProps { Value = bucket.BucketName });
            new CfnOutput(this, "UsersTableName", new CfnOutputProps { Value = usersTable.TableName });
        }
    }
}
