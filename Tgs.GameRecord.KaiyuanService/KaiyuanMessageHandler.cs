using System.Text;
using System.Text.Json;
using Tgs.GameRecord.KaiyuanService.Core.JobWorkerServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.KaiyuanService
{
    internal class KaiyuanMessageHandler : IMessageHandler
    {
        private readonly ILogger<KaiyuanMessageHandler> _logger;
        private readonly ICommandService<KaiyuanJobWorkerCommand> _service;

        public KaiyuanMessageHandler(
            ILogger<KaiyuanMessageHandler> logger,
            ICommandService<KaiyuanJobWorkerCommand> service
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data.Span);
            var messageModel = JsonSerializer.Deserialize<KaiyuanMessageModel>(json);

            if (messageModel == null)
            {
                _logger.LogError("Information Received:{LogInfo}", messageModel);
                return ValueTask.CompletedTask;
            }

            _logger.LogInformation("Information Received:{LogInfo}", messageModel);

            return _service.ExecuteAsync(new KaiyuanJobWorkerCommand()
            {
                GameProviderCode = messageModel.GameProviderCode,
                SendOn = messageModel.SendDateTime,
                ReceiveOn = DateTimeOffset.UtcNow
            }, cancellationToken);
        }
    }
}
