
namespace HKGraphics.Effects;

public class WaveMesh : EffectMeshBase
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
    public List<Vector2> points = new();
    public List<Vector2> uv = new();
    public int NI = 100;
    private Mesh? _mesh;
    public void Update()
    {
        if (_mesh == null)
        {
            _mesh = new();
        }
        //Points
        {
            List<MeshTriangle> triangles = CollectionCache<List<MeshTriangle>>.Borrow(points.Count * NI);
            void PutTriangle(float lx, float luvx, MeshVertex vert, MeshVertex lvert)
            {
                triangles.Add(new(lvert, vert, new(new(lx, 0), new(luvx, 0))));
            }
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var uv = this.uv[i];
                if (i == points.Count - 1)
                {
                    break;
                }
                var npoint = points[i + 1];
                var nuv = this.uv[i + 1];
                var ep_point = (npoint - point) / NI;
                var ep_uv = (nuv - uv) / NI;

                var masterX = (point.y > npoint.y) ? point.x : npoint.x;
                var masterUVX = (point.y > npoint.y) ? uv.x : nuv.x;

                var sub = (point.y > npoint.y) ? npoint : point;
                var subUV = (point.y > npoint.y) ? nuv : uv;

                //FIXME: Use a Smooth curves
                //FIXME: Triangles are missing
                var lp = new MeshVertex(point, uv);

                for (int i2 = 1; i2 <= NI; i2++)
                {
                    var mvp = new MeshVertex(point + (ep_point * i2), uv + (ep_uv * i2));
                    PutTriangle(masterX, masterUVX, mvp, lp);
                    lp = mvp;
                }
                PutTriangle(masterX, masterUVX, new(sub, subUV), lp);
                PutTriangle(masterX, masterUVX, new(sub, subUV), new(new(sub.x, 0), new(subUV.x, 0)));
                /*triangles.Add(new(
                    new(new(masterX, 0), new(masterUVX, 0)),
                    new(new(sub.x, 0), new(subUV.x, 0)),
                    new(sub, subUV)
                    ));*/
            }
            _mesh.Fill(triangles);

            triangles.RecyleCollection();
        }
    }
}

