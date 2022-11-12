
namespace HKGraphics;

public record struct MeshLine(Vector2 from, Vector2 to);
public record struct MeshLineEx(MeshVertex from, MeshVertex to);
public record struct MeshStraightLine(float a, float b);

public static class LineUtils
{
    public static Vector2 GetPoint(this MeshStraightLine sline, float xOry, bool knowX = true) => knowX ? new(xOry, sline.a * xOry + sline.b)
        : new((xOry - sline.b) / sline.a, xOry);
    public static Vector2? GetPoint(this MeshLine line, float xOry, bool knowX = true)
    {
        line = line.Order();
        var r = line.AsStraightLine().GetPoint(xOry, knowX);
        if(r.x < line.from.x || r.x > line.to.x) return null;
        return r;
    }
    public static MeshStraightLine AsStraightLine(this MeshLine line)
    {
        var f = line.from;
        var t = line.to;
        var a = (f.x - t.x) / (f.y - t.y);
        var b = f.y - (a * f.x);
        return new(a, b);
    }
    public static MeshLine AsLine(this MeshLineEx line) => new(line.from.position, line.to.position);
    public static bool IsTouch(this MeshStraightLine a, MeshStraightLine b, out Vector2 intersection)
    {
        intersection = default;
        if (a.a == b.a) return false;
        var x = (b.b - a.b) / (a.a - b.a);
        var y = a.a * x + a.b;
        intersection = new(x, y);
        return true;
    }
    public static MeshLine? Constrained(this MeshStraightLine sline, Rect rect)
    {
        if (!sline.ThroughBox(rect, out _)) return null;
        var min = sline.GetPoint(rect.xMin);
        var max = sline.GetPoint(rect.xMax);
        if (min.y < rect.yMin)
        {
            min = sline.GetPoint(rect.yMin, false);
        }
        else if (min.y > rect.yMax)
        {
            min = sline.GetPoint(rect.yMax, false);
        }
        if (max.y < rect.yMin)
        {
            max = sline.GetPoint(rect.yMin, false);
        }
        else if (max.y > rect.yMax)
        {
            max = sline.GetPoint(rect.yMax, false);
        }
        return new(min, max);
    }
    public static bool ThroughBox(this MeshStraightLine sline, Rect rect, out (Vector2?, Vector2?) intersection)
    {
        intersection = default;
        Vector2?[] inter = new Vector2?[2];
        var index = 0;
        var lb = new MeshLine(rect.min, new(rect.xMin, rect.yMax));
        var rb = new MeshLine(new(rect.xMax, rect.yMin), rect.max);
        var tb = new MeshLine(new(rect.xMin, rect.yMax), rect.max);
        var bb = new MeshLine(rect.min, new(rect.xMax, rect.yMin));
        if (lb.IsTouch(sline, true, out var v1)) inter[index++] = v1;
        if (rb.IsTouch(sline, true, out var v2)) inter[index++] = v2;
        if (tb.IsTouch(sline, true, out var v3)) inter[index++] = v3;
        if (bb.IsTouch(sline, true, out var v4)) inter[index++] = v4;
        intersection = (inter[0], inter[1]);
        return index > 0;
    }
    public static MeshLineEx Order(this MeshLineEx line) => 
        new(line.from.position.x > line.to.position.x ? line.to : line.from, line.from.position.x > line.to.position.x ? line.from : line.to);
    public static MeshLine Order(this MeshLine line) => 
        new(line.from.x > line.to.x ? line.to : line.from, line.from.x > line.to.x ? line.from : line.to);
    public static bool ThroughBox(this MeshLine line, Rect rect, out (Vector2?, Vector2?) intersection)
    {
        intersection = default;
        Vector2?[] inter = new Vector2?[2];
        var index = 0;
        var lb = new MeshLine(rect.min, new(rect.xMin, rect.yMax));
        var rb = new MeshLine(new(rect.xMax, rect.yMin), rect.max);
        var tb = new MeshLine(new(rect.xMin, rect.yMax), rect.max);
        var bb = new MeshLine(rect.min, new(rect.xMax, rect.yMin));
        if (lb.IsTouch(line, true, out var v1)) inter[index++] = v1;
        if (rb.IsTouch(line, true, out var v2)) inter[index++] = v2;
        if (tb.IsTouch(line, true, out var v3)) inter[index++] = v3;
        if (bb.IsTouch(line, true, out var v4)) inter[index++] = v4;
        intersection = (inter[0], inter[1]);
        return index > 0;
    }
    public static bool ThroughPoint(this MeshStraightLine sline, Vector2 point) => sline.GetPoint(point.x) == point;
    public static bool ThroughPoint(this MeshLine line, Vector2 point)
    {
        line = line.Order();
        if (line.from.x > point.x || line.to.x < point.x) return false;
        return line.AsStraightLine().ThroughPoint(point);
    }
    public static float LocationL(this MeshLine line, Vector2 point)
    {
        if (!line.ThroughPoint(point)) return -1;
        line = line.Order();
        var r = new MeshLine(line.from, point).GetLength() / line.GetLength();
        if(Mathf.Abs(line.from.x - point.x) > Mathf.Abs(line.to.x - point.x)) r = 1 - r;
        return r;
    }
    public static float GetLength(this MeshLine line) => Vector2.Distance(line.from, line.to);
    public static MeshLine? Constrained(this MeshLine line, Rect rect)
    {
        if (!line.ThroughBox(rect, out _)) return null;
        line = line.Order();
        var fd = line.from.InBoxDirection(rect);
        var td = line.to.InBoxDirection(rect);
        var sline = line.AsStraightLine();
        var t_cline = sline.Constrained(rect);
        if (t_cline == null) return null;
        var cline = t_cline.Value;
        if (fd.HasFlag(PointDirection.Left))
        {
            line.from = cline.from;
        }
        else if (fd.HasFlag(PointDirection.Right))
        {
            line.from = cline.to;
        }
        if (td.HasFlag(PointDirection.Left))
        {
            line.from = cline.from;
        }
        else if (td.HasFlag(PointDirection.Right))
        {
            line.from = cline.to;
        }
        return line.Order();
    }
    public static bool IsTouch(this MeshLine a, MeshStraightLine b, bool includeEndPoints, out Vector2 intersection)
    {
        a = a.Order();
        if (!IsTouch(a.AsStraightLine(), b, out intersection)) return false;
        if (includeEndPoints)
        {
            if (intersection.x == a.from.x || intersection.x == a.to.x) return true;
        }
        return intersection.x > a.from.x && intersection.x < a.to.x;
    }
    public static bool IsTouch(this MeshStraightLine a, MeshLine b, bool includeEndPoints, out Vector2 intersection)
        => b.IsTouch(a, includeEndPoints, out intersection);
    public static bool IsTouch(this MeshLine a, MeshLine b, bool includeEndPoints, out Vector2 intersection)
    {
        a = a.Order();
        b = b.Order();
        var af = a.from;
        var at = a.to;
        var bf = b.from;
        var bt = b.to;

        var sa = a.AsStraightLine();
        var sb = b.AsStraightLine();
        intersection = default;
        if (!sa.IsTouch(sb, out var i)) return false;
        if (includeEndPoints)
        {
            if (i.x == af.x || i.x == at.x || i.x == bf.x || i.x == bt.x) return true;
        }
        if (i.x < Mathf.Min(af.x, at.x) || i.x > Mathf.Max(af.x, at.x)) return false;
        if (i.x < Mathf.Min(bf.x, bt.x) || i.x > Mathf.Max(bf.x, bt.x)) return false;

        intersection = i;
        return true;
    }
}
