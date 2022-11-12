
namespace HKGraphics;

class GraphicsLibrary : ModBase<GraphicsLibrary>
{
    
    public static Dictionary<string, Shader> shaders = new();
    public static readonly AssetBundle empty_scene = Application.platform switch {
        RuntimePlatform.WindowsPlayer => ModRes.AB_EMPTY_SCENE_WIN,
        RuntimePlatform.LinuxPlayer => ModRes.AB_EMPTY_SCENE_LINUX,
        RuntimePlatform.OSXPlayer => ModRes.AB_EMPTY_SCENE_MAC,
        _ => throw new PlatformNotSupportedException()
    };
    public static readonly AssetBundle main_ab = Application.platform switch {
        RuntimePlatform.WindowsPlayer => ModRes.AB_MAIN_WIN,
        RuntimePlatform.LinuxPlayer => ModRes.AB_MAIN_LINUX,
        RuntimePlatform.OSXPlayer => ModRes.AB_MAIN_MAC,
        _ => throw new PlatformNotSupportedException()
    };
    public static readonly string EmptySceneName = "HKGraphicsEmptyScene";
    static GraphicsLibrary()
    {
        foreach(var v in main_ab.LoadAllAssets<Shader>())
        {
            shaders[v.name.Split('/')[1]] = v;
        }
    }
    public override void Initialize()
    {
        if(DebugManager.IsDebug(this)) Test();
    }
    public void Test()
    {
        wm.points = new()
        {
            new(0, 1),
            new(1, 2),
            new(2, 3),
            new(3, 2),
            new(4, 1)
        };
        wm.uv = new()
        {
            new(0, 1),
            new(0, 1),
            new(0, 1),
            new(0, 1),
            new(0, 1),
            new(0, 1),
            new(0, 1),
            new(0, 1),
            new(0, 1)
        };
        wm.NI = 50;
        wm.Update();
        /*wm.AddVert(new MeshVertex(new Vector2(0, 0), new Vector2(0, 0)));
        wm.AddVert(new MeshVertex(new Vector2(1, 2), new Vector2(1, 1)));
        wm.AddVert(new MeshVertex(new Vector2(2, 3), new Vector2(1, 1)));
        wm.AddVert(new MeshVertex(new Vector2(4, 2), new Vector2(1, 1)));
        wm.AddVert(new MeshVertex(new Vector2(5, 5), new Vector2(1, 1)));
        wm.AddVert(new MeshVertex(new Vector2(6, 0), new Vector2(1, 1)));
        wm.Update();/*/
        mat.mainTexture = Texture2D.whiteTexture;

        ModHooks.HeroUpdateHook += () =>
        {
            if(testMesh == null)
            {
                testMesh = new();
                testMesh.AddComponent<MeshRenderer>().material = mat;
                testMesh.AddComponent<MeshFilter>().sharedMesh = wm;
            }
            testMesh.transform.position = HeroController.instance.transform.position;
        };
    }
    private static Material mat = new(Shader.Find("Diffuse"));
    private static WaveMesh wm = new();
    private static GameObject testMesh;
}
