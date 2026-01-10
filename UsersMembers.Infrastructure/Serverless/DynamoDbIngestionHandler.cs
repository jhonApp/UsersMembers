using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UsersMembers.Infrastructure.Serverless
{
    public class DynamoDbIngestionHandler
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _tableName;

        public DynamoDbIngestionHandler()
        {
            _dynamoDb = new AmazonDynamoDBClient();
            _tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME") ?? "EventsTable";
        }

        public DynamoDbIngestionHandler(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
            _tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME") ?? "EventsTable";
        }

        // MUDANÇA 1: O retorno agora é SQSBatchResponse
        public async Task<SQSBatchResponse> FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            var batchItemFailures = new List<SQSBatchResponse.BatchItemFailure>();
            
            // Dica de Performance: Carregar a tabela é pesado. Em high-scale, cachear isso seria ideal.
            var table = Table.LoadTable(_dynamoDb, _tableName);

            foreach (var message in evnt.Records)
            {
                try
                {
                    context.Logger.LogInformation($"Processing message {message.MessageId}");

                    var doc = Document.FromJson(message.Body);

                    // Idempotency
                    if (!doc.ContainsKey("Id"))
                    {
                        doc["Id"] = message.MessageId;
                    }

                    // TTL (90 dias)
                    if (!doc.ContainsKey("ttl")) // Só adiciona se não vier no payload
                    {
                        long ttl = DateTimeOffset.UtcNow.AddDays(90).ToUnixTimeSeconds();
                        doc["ttl"] = ttl;
                    }

                    doc["IngestedAt"] = DateTime.UtcNow.ToString("O");

                    await table.PutItemAsync(doc);
                }
                catch (Exception e)
                {
                    // MUDANÇA 2: Não damos throw. Logamos o erro e marcamos SÓ ESSA mensagem como falha.
                    context.Logger.LogError($"Failed to process message {message.MessageId}: {e.Message}");
                    
                    batchItemFailures.Add(new SQSBatchResponse.BatchItemFailure
                    {
                        ItemIdentifier = message.MessageId
                    });
                }
            }

            // MUDANÇA 3: Retornamos para o SQS quem falhou.
            // Se a lista estiver vazia, o SQS entende que tudo foi sucesso e deleta o lote da fila.
            return new SQSBatchResponse(batchItemFailures);
        }
    }
}