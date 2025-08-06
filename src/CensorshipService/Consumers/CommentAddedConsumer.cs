using CensorshipService.Services;
using MassTransit;
using MassTransit.Transports;
using Messaging;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CensorshipService.Consumers
{
    public class CommentAddedConsumer(ITextCensorshipService censor, ISendEndpointProvider publish) : IConsumer<CommentAdded>
    {
        private readonly ITextCensorshipService _censor = censor;
        private readonly ISendEndpointProvider _publish = publish;

        public async Task Consume(ConsumeContext<CommentAdded> context)
        {
            var msg = context.Message;
            var text = msg.Text;

            var hasBannedWords = _censor.ContainsBannedWords(text);

            var resultToSend = new CensorChecked { HasBannedWords = hasBannedWords };

            var endpoint = await _publish.GetSendEndpoint(new Uri("queue:comment-censor-checked"));

            resultToSend.CommentId = msg.CommentId;
            await endpoint.Send(resultToSend);
        }
    }
}
