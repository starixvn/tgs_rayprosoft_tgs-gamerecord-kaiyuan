using Tgs.GameRecord.Kaiyuan.Core.JobApplicationServices;

namespace Tgs.GameRecord.Kaiyuan.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
