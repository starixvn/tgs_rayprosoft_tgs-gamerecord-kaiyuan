using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Kaiyuan.Common;

public record KaiyuanGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://wc1-record.uaphl791.com/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "E2F3A416006F84F8")]
	public string Deskey { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "B563DF2A3DE58DFB")]
	public string Md5key { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "72412")]
	public string AgentId { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-07-26T11:50:00+08:00")]
	public DateTimeOffset StartTime { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-07-26T12:00:00+08:00")]
	public DateTimeOffset EndTime { get; init; } = default!;
}
