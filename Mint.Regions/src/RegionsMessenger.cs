namespace Mint.Server.Regions;

public class RegionsMessenger : PlayerMessenger
{
    public override void CleanSend(MessageMark mark, string? source, string message, params object?[] objects) {}
    public override void Send(MessageMark mark, string? source, string message, params object?[] objects) {}
    public override void SendPage(string headerFormat, IList<string> lines, int page, string? footerFormat = null, string? nextPageFormat = null) {}
}