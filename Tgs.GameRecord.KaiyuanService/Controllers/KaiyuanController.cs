using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.KaiyuanService.Core.ApplicationServices;
using Tgs.GameRecord.KaiyuanService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.KaiyuanService.Controllers;

[ApiController]
[Route("KY")]
public class KaiyuanController : ControllerBase
{
    [HttpPost]
    public async ValueTask<GameRecordResultViewModel> GameRecordAsync(
        [FromServices] IGetService<GameRecordQuery, GameRecordResult> service,
        GameRecordQueryViewModel model)
    {
        var result = await service.GetAsync(new GameRecordQuery()
        {
            BrandCode = model.BrandCode,
            CurrencyCode = model.CurrencyCode,
            StartTime = model.StartTime,
            EndTime = model.EndTime
        }).ConfigureAwait(false);

        return new GameRecordResultViewModel()
        {
            Success = result.Success,
            LastGameRecordTime = result.LastGameRecordTime,
            SuccessCount = result.SuccessCount,
            FailureCount = result.FailureCount,
            Count = result.Count,
        };
    }
}
