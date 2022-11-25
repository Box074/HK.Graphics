
namespace HKGraphics;

public record struct MeshTriangle(MeshVertex a, MeshVertex b, MeshVertex c)
{
    public MeshVertex a = a;
    public MeshVertex b = b;
    public MeshVertex c = c;
}

public static class TriangleUtils
{
    public static (MeshLineEx ab, MeshLineEx bc, MeshLineEx ac) GetLines(this MeshTriangle triangle) =>
        (new(triangle.a, triangle.b), new(triangle.b, triangle.c), new(triangle.a, triangle.c));
    public static MeshLineEx[] GetLineArray(this MeshTriangle triangle) => new MeshLineEx[]{
        new(triangle.a, triangle.b), new(triangle.b, triangle.c), new(triangle.a, triangle.c)
        };
    public static MeshLineEx GetLine(this MeshTriangle triangle, int id) => id switch
    {
        0 => new(triangle.a, triangle.b),
        1 => new(triangle.b, triangle.c),
        2 => new(triangle.a, triangle.c),
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };
    public static int GetLineId(this MeshTriangle triangle, MeshLine line) => triangle.GetLineArray()
        .Where(x => x.AsLine().Order() == line.Order()).Count();
    public static int GetLineId(this MeshTriangle triangle, MeshLineEx line) => triangle.GetLineArray()
        .Where(x => x.AsLine().Order() == line.AsLine().Order()).Count();

}
