using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Kaiyuan.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Kaiyuan.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	TimeProvider timeProvider,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	private readonly double _intervalMinutes = options.Value.IntervalMinutesMax < options.Value.IntervalMinutes
		? options.Value.IntervalMinutesMax
		: options.Value.IntervalMinutesMinimum > options.Value.IntervalMinutes
		? options.Value.IntervalMinutesMinimum
		: options.Value.IntervalMinutes;

	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var endTime = DateTimeOffset.ParseExact(currentTime.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:00K", null, styles:
		System.Globalization.DateTimeStyles.RoundtripKind);
		var startTime = endTime.AddMinutes(_intervalMinutes * -1);

		var message = new KaiyuanGameRecordMessage
		{
			ApiUrl = command.ApiUrl,
			Deskey = command.Deskey,
			Md5key = command.Md5key,
			AgentId = command.AgentId,
			StartTime = startTime,
			EndTime = endTime
		};

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = KaiyuanNatsConst.SchedulePublish,
				Data = new
				{
					command.ApiUrl,
					command.Deskey,
					command.Md5key,
					command.AgentId,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u")
				},
				Message = "This message publishing handler."
			}));

		return messageSender.PublishAsync(
			subject: KaiyuanNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(message),
			cancellationToken: cancellationToken);
	}
}
