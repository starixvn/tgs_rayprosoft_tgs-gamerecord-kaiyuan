using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.KaiyuanService.Core;

namespace Tgs.GameRecord.KaiyuanService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InfoController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public object Get()
        {
            var gameRecordInfo = _configuration.GetSection("GameRecordInfo").Get<GameRecordInfoOptions>();
            var mongoSetting = _configuration.GetSection("MongoSetting").Get<MongoSettingOptions>();

            return new
            {
                gameRecordInfo,
                mongoSetting
            };
        }
    }
}
