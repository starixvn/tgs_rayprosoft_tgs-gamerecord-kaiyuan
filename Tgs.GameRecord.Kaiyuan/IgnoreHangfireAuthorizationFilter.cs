using Hangfire.Dashboard;

namespace Tgs.GameRecord.Kaiyuan;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
