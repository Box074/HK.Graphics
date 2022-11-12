
namespace HKGraphics.Effects;


//TODO:
public class PolygonMesh : EffectMeshBase
{
    public override void Dispose()
    {
        if (DontDestroy) return;
        if (_mesh != null)
        {
            UnityEngine.Object.DestroyImmediate(_mesh);
            _mesh = null;
        }
    }
    public override Mesh UnityObject
    {
        get
        {
            Update();
            return _mesh!;
        }
    }
    private Mesh? _mesh;
    private record struct LinePair(int a, int b);
    private record struct Triangle(int a, int b, int c);
    private List<DataHolder<MeshVertex>> vert = new();
    private HashSet<int> dirtyVert = new();
    private HashSet<LinePair> lines = new();
    private HashSet<Triangle> triangles = new();
    public int VertCount => vert.Count;
    public bool RequireUpdate => dirtyVert.Count > 0;
    public int AddVert(DataHolder<MeshVertex> vert)
    {
        var id = VertCount;
        if (id != 0)
        {
            SetVertDirty(id - 1);
            SetVertDirty(0);
        }
        this.vert.Add(default);
        SetVert(id, vert);
        SetVertDirty(id);
        return id;
    }
    public bool SetVert(int id, DataHolder<MeshVertex> vert)
    {
        if (id >= VertCount) return false;
        this.vert[id] = vert;
        vert.onUpdate += (o, n) =>
        {
            if (o != n.value)
            {
                SetVertDirty(id);
            }
        };
        SetVertDirty(id);
        return true;
    }
    public void SetVertDirty(int id)
    {
        if (id >= VertCount) throw new ArgumentOutOfRangeException(nameof(id));
        if (!dirtyVert.Add(id)) return;
        lines.RemoveWhere(x => x.a == id || x.b == id);
        triangles.RemoveWhere(x => x.a == id || x.b == id || x.c == id);
    }
    private void TryAddLine(int a, int b)
    {
        lines.Add(new(Mathf.Min(a, b), Mathf.Max(a, b)));
    }
    private void TryAddTriangle(int a, int b, int c)
    {
        if (a > b)
        {
            var t = a;
            a = b;
            b = t;
        }
        if (b > c)
        {
            var t = b;
            b = c;
            c = t;
        }
        if (a > c)
        {
            var t = c;
            c = a;
            a = t;
        }
        var triangle = new Triangle(a, b, c);
        TryAddLine(a, b);
        TryAddLine(a, c);
        TryAddLine(b, c);
        triangles.Add(triangle);
    }
    private MeshLine GetLine(LinePair line)
    {
        return new(vert[line.a].value.position, vert[line.b].value.position);
    }
    public void Update()
    {
        if(_mesh == null)
        {
            _mesh = new();
        }
        if (VertCount < 3) return;
        for (int i = 0; i < VertCount - 1; i++)
        {
            TryAddLine(i, i + 1);
        }
        TryAddLine(0, VertCount - 1);
        bool CanLink(int a, int b)
        {
            var pa = vert[a].value.position;
            var pb = vert[b].value.position;
            var line = new MeshLine(pa, pb);
            if (Mathf.Abs(a - b) <= 1 || (Mathf.Min(a, b) == 0 && Mathf.Max(a, b) == VertCount - 1)) return false;
            var r = lines.Any(x => GetLine(x).IsTouch(line, false, out _));
            if (!r) return false;
            return true;
        }
        foreach (var v in dirtyVert)
        {
            for (int i = 0; i < VertCount; i++)
            {
                if (!CanLink(v, i)) continue;
                for (int i2 = 0; i2 < VertCount; i2++)
                {
                    if(CanLink(v, i2) && CanLink(i, i2))
                    {
                        TryAddTriangle(v, i, i2);
                    }
                }
            }
        }
        dirtyVert.Clear();

        var t_vert = new Vector3[VertCount];
        var t_uv = new Vector2[VertCount];
        for(int i = 0; i < VertCount ; i++)
        {
            var v = vert[i].value;
            t_vert[i] = v.position;
            t_uv[i] = v.uv;
        }
        _mesh.vertices = t_vert;
        _mesh.uv = t_uv;

        var t_triangles = new int[triangles.Count * 3];
        var t_tindex = 0;
        foreach(var v in triangles)
        {
            t_triangles[t_tindex++] = v.a;
            t_triangles[t_tindex++] = v.b;
            t_triangles[t_tindex++] = v.c;
        }
        _mesh.triangles = t_triangles;
    }
}
