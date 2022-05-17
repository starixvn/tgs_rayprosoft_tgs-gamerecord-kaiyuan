namespace Tgs.GameRecord.KaiyuanService.Core
{
    public interface IGameRecordMessageSender
    {
        ValueTask SendGameRoundAsync(GameRoundMessageModel data);
    }
}
