using Microsoft.Xna.Framework;
using Mint.Server;
using Terraria;
using Terraria.ID;
using Player = Mint.Server.Player;

namespace Mint.Vulnpatch;

public unsafe static class TileExtensions
{
    public static Rectangle ToRectangle(Point point1, Point point2)
    {
        int minX = Math.Min(point1.X, point2.X);
        int minY = Math.Min(point1.Y, point2.Y);
        int maxX = Math.Max(point1.X, point2.X);
        int maxY = Math.Max(point1.Y, point2.Y);

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    public static bool InWorld(this Rectangle rectangle)
    {
        int xw = rectangle.X + rectangle.Width;
        int yh = rectangle.Y + rectangle.Height;

        return rectangle.X > 0 && rectangle.X < Main.maxTilesX
                && rectangle.Y > 0 && rectangle.Y < Main.maxTilesY
                && xw > 0 && xw < Main.maxTilesX
                && yh > 0 && yh < Main.maxTilesY;
    }

    public static bool InWorld(this Point point)
    {
        return point.X > 0 && point.X < Main.maxTilesX
                && point.Y > 0 && point.Y < Main.maxTilesY;
    }

    public static bool InWorld(int x, int y)
    {
        return x > 0 && x < Main.maxTilesX
                && y > 0 && y < Main.maxTilesY;
    }

    public static bool InRange(this Player player, int x, int y, int radius)
    {
        float num = player.TPlayer.position.X - x * 16;
        float num2 = player.TPlayer.position.Y - y * 16;
        return (float)Math.Sqrt(num * num + num2 * num2) < radius * 16;
    }

    public static bool CanKillTileV1(this Player player, int x, int y)
    {

        return false;
    }
}