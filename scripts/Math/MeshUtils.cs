
namespace HKGraphics;



public static class MeshUtils
{
    private record class MeshCutTriangle(MeshTriangle Triangle, MeshLineEx oline, MeshLine line, MeshCutTriangle other = null!)
    {
        public MeshCutTriangle other { get; set; } = other;
    }
    public static int GetTriangles(this Mesh mesh, List<MeshTriangle> triangles)
    {
        if (triangles == null) throw new ArgumentNullException(nameof(triangles));

        var allTrianglesInner = CollectionCache<List<int>>.Borrow((int)mesh.GetTrianglesCountImpl(0) * 3);
        var allUVInner = CollectionCache<List<Vector2>>.Borrow();
        var allPoints = CollectionCache<List<Vector3>>.Borrow();
        mesh.GetUVs(0, allUVInner);
        mesh.GetVertices(allPoints);
        mesh.GetTriangles(allTrianglesInner, 0);


        for (int i = 0; i < allTrianglesInner.Count; i++)
        {
            int a = i++;
            int b = i++;
            int c = i;
            Vector2 pa = allPoints[a];
            Vector2 pb = allPoints[b];
            Vector2 pc = allPoints[c];
            MeshTriangle triangle = new(new(pa, allUVInner[a]), new(pb, allUVInner[b]), new(pc, allUVInner[c]));
            triangles.Add(triangle);
        }

        var result = allTrianglesInner.Count;
        allTrianglesInner.RecyleCollection();
        allUVInner.RecyleCollection();
        allPoints.RecyleCollection();
        return result;
    }
    //TODO: 
    public static void Cut(this Mesh mesh, Rect rect, List<MeshTriangle> triangles)
    {
        if (triangles == null) throw new ArgumentNullException(nameof(triangles));

        var borderL = new MeshLine(rect.min, new(rect.xMin, rect.yMax)).AsStraightLine();
        var borderR = new MeshLine(rect.max, new(rect.xMax, rect.yMin)).AsStraightLine();
        var borderU = new MeshLine(rect.max, new(rect.xMin, rect.yMax)).AsStraightLine();
        var borderB = new MeshLine(rect.min, new(rect.xMax, rect.yMin)).AsStraightLine();

        var allTriangles = CollectionCache<List<MeshTriangle>>.Borrow((int)mesh.GetTrianglesCountImpl(0) * 3);
        var cutTriangles = CollectionCache<List<MeshTriangle>>.Borrow();

        #region Get All Triangles
        {
            mesh.GetTriangles(allTriangles);
            foreach (var v in allTriangles)
            {
                
                int inBoxCount = (v.a.position.InBox(rect) ? 1 : 0) + (v.b.position.InBox(rect) ? 1 : 0) + (v.c.position.InBox(rect) ? 1 : 0);
                if (inBoxCount == 0) continue;
                if (inBoxCount < 3)
                {
                    cutTriangles.Add(v);
                }
                else
                {
                    triangles.Add(v);
                }
            }
        }
        #endregion
        var l_cutL = CollectionCache<HashSet<MeshCutTriangle>>.Borrow(cutTriangles.Count / 4);
        var l_cutR = CollectionCache<HashSet<MeshCutTriangle>>.Borrow(cutTriangles.Count / 4);
        var l_cutU = CollectionCache<HashSet<MeshCutTriangle>>.Borrow(cutTriangles.Count / 4);
        var l_cutD = CollectionCache<HashSet<MeshCutTriangle>>.Borrow(cutTriangles.Count / 4);
        #region Category
        {

            void CheckPoint(PointDirection dir, MeshTriangle t, MeshLineEx l0, MeshLineEx l1)
            {

                MeshCutTriangle item = new(t, l0, l0.AsLine().Constrained(rect)!.Value);
                MeshCutTriangle item1 = new(t, l1, l1.AsLine().Constrained(rect)!.Value);
                item.other = item1;
                if (dir.HasFlag(PointDirection.Left))
                {
                    l_cutL!.Add(item);
                    //l_cutL!.Add(item1);
                }
                if (dir.HasFlag(PointDirection.Right))
                {
                    l_cutR!.Add(item);
                    //l_cutR!.Add(item1);
                }
                if (dir.HasFlag(PointDirection.Up))
                {
                    l_cutU!.Add(item);
                    //l_cutU!.Add(item1);
                }
                if (dir.HasFlag(PointDirection.Down))
                {
                    l_cutD!.Add(item);
                    //l_cutD!.Add(item1);
                }
            }
            foreach (var v in cutTriangles)
            {
                var ad = v.a.position.InBoxDirection(rect);
                var bd = v.b.position.InBoxDirection(rect);
                var cd = v.c.position.InBoxDirection(rect);

                var lines = v.GetLines();
                CheckPoint(ad, v, lines.ab, lines.ac);
                CheckPoint(bd, v, lines.ab, lines.bc);
                CheckPoint(cd, v, lines.bc, lines.ac);
            }
        }
        #endregion
        #region Cut Triangles
        {
            void CutTriangles(HashSet<MeshCutTriangle> cur, HashSet<MeshCutTriangle> next, PointDirection dir)
            {
                MeshVertex ProcessLine(MeshCutTriangle mct)
                {
                    (var t, var oline, var line, var other) = mct;
                    var odline = line.Order();
                    var p = dir switch 
                    {
                        PointDirection.Left => odline.from,
                        PointDirection.Right => odline.to,
                        PointDirection.Up => odline.from,
                        PointDirection.Down => odline.to,
                        _ => throw new InvalidOperationException()
                    };
                    var od_oline = oline.Order();
                    var s = od_oline.AsLine().LocationL(p);
                    if(s < 0) throw new InvalidOperationException();
                    var uv = Vector2.Lerp(od_oline.from.uv, od_oline.to.uv, s);
                    var pp = new MeshVertex(p, uv);
                    return pp;
                }
                foreach(var m in cur)
                {
                    var p1 = ProcessLine(m);
                    var p2 = ProcessLine(m.other);

                    var o_vertex = CollectionCache<HashSet<MeshVertex>>.Borrow(3);
                    o_vertex.Add(m.oline.from);
                    o_vertex.Add(m.oline.to);
                    o_vertex.Add(m.other.oline.from);
                    o_vertex.Add(m.other.oline.to);

                    MeshVertex outp;
                    var inp = CollectionCache<List<MeshVertex>>.Borrow(2);
                    foreach(var v in o_vertex)
                    {
                        if(v.position.InBox(rect)) inp.Add(v);
                        else outp = v;
                    }

                    var t1 = new MeshTriangle(p1, p2, inp[0]);
                    var t2 = new MeshTriangle(inp[0], inp[1], 
                        (Vector2.Distance(p1.position, inp[1].position) > Vector2.Distance(p2.position, inp[1].position)) ? p2 : p1);
                    triangles.Add(t1);
                    triangles.Add(t2);

                    o_vertex.RecyleCollection();
                    inp.RecyleCollection();
                }
            }
            CutTriangles(l_cutL, l_cutU, PointDirection.Left);
            CutTriangles(l_cutU, l_cutR, PointDirection.Right);
            CutTriangles(l_cutR, l_cutD, PointDirection.Up);
            CutTriangles(l_cutD, l_cutL, PointDirection.Down);
        }
        #endregion
        
        #region Free
        allTriangles.RecyleCollection();
        l_cutL.RecyleCollection();
        l_cutR.RecyleCollection();
        l_cutU.RecyleCollection();
        l_cutD.RecyleCollection();
        #endregion
    }

    public static void Fill(this Mesh mesh, params StructArrayProvider<MeshTriangle>[] triangleGroups)
    {
        mesh.Clear();
        var triangleCount = (int)triangleGroups.Average(x => x.Count);
        Dictionary<MeshVertex, int> vertTable = CollectionCache<Dictionary<MeshVertex, int>>.Borrow(triangleCount * 2);
        List<Vector3> vertex = CollectionCache<List<Vector3>>.Borrow(triangleCount * 2);
        List<Vector2> uv = CollectionCache<List<Vector2>>.Borrow(triangleCount * 2);
        List<int> t_triangles = CollectionCache<List<int>>.Borrow(triangleCount * 3);
        try
        {
            void PutVertex(MeshVertex vert)
            {
                t_triangles.Add(vertTable.TryGetOrAddValue(vert, () =>
                {
                    vertex.Add(vert.position);
                    uv.Add(vert.uv);
                    return vertex.Count - 1;
                }));
            }
            foreach (var v in triangleGroups)
            {
                for (int i = 0; i < v.Count; i++)
                {
                    var triangle = v[i];
                    PutVertex(triangle.a);
                    PutVertex(triangle.b);
                    PutVertex(triangle.c);
                }
            }

            mesh.vertices = vertex.ToArray();
            mesh.triangles = t_triangles.ToArray();
            mesh.uv = uv.ToArray();
        }
        finally
        {
            uv.RecyleCollection();
            vertex.RecyleCollection();
            vertTable.RecyleCollection();
            t_triangles.RecyleCollection();
        }
    }
}
