
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
                var cor = cae.Lerp(new(UnityEngine.Random.value * 2 - 1,UnityEngine.Random.value * 2 - 1,UnityEngine.Random.value * 2 - 1), 1.5f);
                cor.onFinished += _ =>
                {
                    f = true;
                };
            }
        };
    }
    private static ColorAddEffect? cae;
}
