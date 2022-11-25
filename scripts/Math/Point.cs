
namespace HKGraphics;

public record struct MeshVertex(Vector2 position, Vector2 uv)
{
    public Vector2 position = position;
    public Vector2 uv = uv;
}

[Flags]
public enum PointDirection
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8
}

public static class PointUtils
{
    public static bool IsInvaild(this PointDirection direction) => (direction.HasFlag(PointDirection.Up) && direction.HasFlag(PointDirection.Down)) ||
       (direction.HasFlag(PointDirection.Left) && direction.HasFlag(PointDirection.Right));
    public static PointDirection GetDirection(this Vector2 self, Vector2 target, float epsilon = float.Epsilon)
    {
        PointDirection result = PointDirection.None;
        if (Mathf.Abs(self.x - target.x) >= epsilon)
        {
            if (self.x > target.x)
            {
                result |= PointDirection.Left;
            }
            else if (self.x < target.x)
            {
                result |= PointDirection.Right;
            }
        }
        if (Mathf.Abs(self.y - target.y) >= epsilon)
        {
            if (self.y > target.y)
            {
                result |= PointDirection.Down;
            }
            else if (self.y < target.y)
            {
                result |= PointDirection.Up;
            }
        }
        return result;
    }

    public static bool InBox(this Vector2 pos, Rect rect) => (pos.x >= rect.xMin && pos.x <= rect.xMax) && (pos.y >= rect.yMin && pos.y <= rect.yMax);
    public static PointDirection InBoxDirection(this Vector2 pos, Rect rect, float epsilon = float.Epsilon)
    {
        if (pos.InBox(rect)) return PointDirection.None;
        PointDirection result = PointDirection.None;
        var dlb = rect.min.GetDirection(pos, epsilon);
        var dru = rect.max.GetDirection(pos, epsilon);
        var dlu = GetDirection(new(rect.xMin, rect.yMax), pos, epsilon);
        var drb = GetDirection(new(rect.xMax, rect.yMin), pos, epsilon);
        result |= (dlb & PointDirection.Left);
        result |= (dru & (PointDirection.Right | PointDirection.Up));
        result |= (dlu & (PointDirection.Left | PointDirection.Up));
        result |= (drb & (PointDirection.Right | PointDirection.Down));
        return result;
    }

    public static Vector2 Constrained(this Vector2 self, Rect rect)
    {
        var dir = self.InBoxDirection(rect);
        if (dir.HasFlag(PointDirection.Left))
        {
            if (dir.HasFlag(PointDirection.Up))
            {
                self = new(rect.xMin, rect.yMax);
            }
            else if (dir.HasFlag(PointDirection.Down))
            {
                self = new(rect.xMin, rect.yMin);
            }
            else
            {
                self.x = rect.xMin;
            }
        }
        else if (dir.HasFlag(PointDirection.Right))
        {
            if (dir.HasFlag(PointDirection.Up))
            {
                self = new(rect.xMax, rect.yMax);
            }
            else if (dir.HasFlag(PointDirection.Down))
            {
                self = new(rect.xMax, rect.yMin);
            }
            else
            {
                self.x = rect.xMax;
            }
        }
        return self;
    }
}
