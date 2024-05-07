using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Kaiyuan.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Kaiyuan.Controllers;

[ApiController]
[Route("KY")]
public class KaiyuanController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "apiUrl": "https://wc1-record.uaphl791.com/",
	///             "deskey": "E2F3A416006F84F8",
	///             "md5key": "B563DF2A3DE58DFB",
	///             "agentId": "72412",
	///             "startTime": "2023-07-26T11:50:00+08:00",
	///             "endTime": "2023-07-26T12:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		KaiyuanGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: KaiyuanNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///             "apiUrl": "https://wc1-record.uaphl791.com/",
	///             "deskey": "E2F3A416006F84F8",
	///             "md5key": "B563DF2A3DE58DFB",
	///             "agentId": "72412",
	///             "startTime": "2023-07-26T11:50:00+08:00",
	///             "endTime": "2023-07-26T12:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		KaiyuanGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: KaiyuanNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
