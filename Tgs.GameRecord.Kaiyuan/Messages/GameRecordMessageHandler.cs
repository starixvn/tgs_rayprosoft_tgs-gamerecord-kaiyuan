using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Kaiyuan.Common;
using Tgs.GameRecord.Kaiyuan.Core.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Kaiyuan.Messages;

public class GameRecordMessageHandler : IMessageHandler
{
    private readonly ILogger<GameRecordMessageHandler> _logger;
    private readonly ICommandService<PullRecordCommand> _service;

    public GameRecordMessageHandler(
        ILogger<GameRecordMessageHandler> logger,
        ICommandService<PullRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var message = JsonSerializer.Deserialize<GameRecordMessage>(json)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        _logger.LogInformation("LogOn:{logOn} Information Request:{message}", DateTime.Now.ToString("o"), message);

        var command = new PullRecordCommand(
            ApiUrl: message.ApiUrl,
            Deskey: message.Deskey,
            Md5key: message.Md5key,
            AgentId: message.AgentId,
            StartTime: message.StartTime,
            EndTime: message.EndTime);

        return _service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
