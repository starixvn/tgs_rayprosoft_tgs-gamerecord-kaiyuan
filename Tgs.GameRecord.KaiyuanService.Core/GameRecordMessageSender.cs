using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.MessageQueue;

namespace Tgs.GameRecord.KaiyuanService.Core
{
    internal class GameRecordMessageSender : IGameRecordMessageSender
    {
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;
        private readonly ILogger<GameRecordMessageSender> _logger;
        private readonly IMessageSender _messageSender;

        public GameRecordMessageSender(
            ILogger<GameRecordMessageSender> logger,
            IMessageSender messageSender,
            IOptions<GameRecordInfoOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _gameRecordInfoOptions = options.Value;
        }

        public ValueTask SendGameRoundAsync(GameRoundMessageModel data)
        {
            _logger.Log(LogLevel.Information,
                new EventId(1000, nameof(GameRecordMessageSender)),
                new
                {
                    MessageQueue = "tgs.gp.gamerecord.database.round",
                    Data = data,
                    SendDateTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8)).ToString("O")
                },
                null,
                (state, ex) => JsonSerializer.Serialize(state));
            return _messageSender.PublishAsync($"tgs.gp.gamerecord.database.round.{_gameRecordInfoOptions.GameProviderCode.ToLower()}", data.ToBson());
        }
    }
}
