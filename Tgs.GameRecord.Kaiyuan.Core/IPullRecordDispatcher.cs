using Tgs.GameRecord.Kaiyuan.Core.ApplicationServices;

namespace Tgs.GameRecord.Kaiyuan.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
