
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
    public static Texture2D? tatlas;
    public static Texture2D? otex;
    public static Texture2D? tex2;
    public static TextureMask? mask;
    public void Test()
    {
        tatlas = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "sactx-0-2048x2048-DXT5-UI-923ee0f1");
        Stopwatch watch = new();
        watch.Start();
        otex = tatlas.Cut(new(512,512, 512, 1024), TextureFormat.RGBA32);
        mask = new TextureMask(true, new(512, 1024), new TextureMask.VectorGroup(new(10, 20), new(256, 128), new(256, 256)));
        tex2 = otex.CreateReadable(material: mask);
        watch.Stop();
        Log($"Cut: {watch.ElapsedMilliseconds}ms");

        bool nextA = false;
        bool f = true;
        ModHooks.HeroUpdateHook += () =>
        {
            if(cae == null)
            {
                cae = HeroController.instance.gameObject.AddComponent<ColorAddEffect>();
            }
            if(f)
            {
                nextA = !nextA;
                f = false;
                var cor = cae.Lerp(nextA ? new(0f,0f,0f,1) : new(-1f,-1f,-1f,1), nextA ? 0.35f : 4f);
                cor.onFinished += _ =>
                {
                    f = true;
                };
            }
        };
    }
    private static ColorAddEffect? cae;
}
