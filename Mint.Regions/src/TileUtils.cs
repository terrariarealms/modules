using Microsoft.Xna.Framework;

namespace Mint.Server.Regions;

public static class TileUtils
{
    internal static Rectangle ToRectangle(Point point1, Point point2)
    {
        int minX = Math.Min(point1.X, point2.X);
        int minY = Math.Min(point1.Y, point2.Y);
        int maxX = Math.Max(point1.X, point2.X);
        int maxY = Math.Max(point1.Y, point2.Y);

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    internal static bool HandleTilePoint(Player player, Point point)
    {
        if (player.CanBuildIn(point.X, point.Y))
            return true;

        Notify(player);
        return false;
    }

    internal static bool HandleTilePoints(Player player, Point point1, Point point2)
    {
        Rectangle rectangle = ToRectangle(point1, point2);
        return HandleTileRectangle(player, rectangle);
    }

    internal static bool HandleTileRectangle(Player player, Rectangle rectangle)
    {
        if (player.CanBuildIn(rectangle))
            return true;

        Notify(player);
        return false;
    }

    internal static void Notify(Player player)
    {
        PlayerRegion rgData = player.GetRegion();
        if (rgData.warningThreshold > DateTime.UtcNow)
            return;
        
        rgData.warningThreshold = DateTime.UtcNow.AddSeconds(3);
        player.Messenger.Send(MessageMark.Error, "Regions", "You do not have permission to build here.");
    }
}