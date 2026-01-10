using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using UsersMembers.Domain.Interfaces;

namespace UsersMembers.Infrastructure.Events
{
    public class SqsEventProducer : IEventProducer
    {
        private readonly IAmazonSQS _sqsClient;

        public SqsEventProducer(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }

        public async Task PublishAsync<T>(T @event, string queueUrl)
        {
            var messageBody = JsonSerializer.Serialize(@event);

            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            await _sqsClient.SendMessageAsync(request);
        }
    }
}
