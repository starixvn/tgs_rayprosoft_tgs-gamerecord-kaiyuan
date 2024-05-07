using Tgs.GameRecord.Kaiyuan.Core.JobApplicationServices;

namespace Tgs.GameRecord.Kaiyuan.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
