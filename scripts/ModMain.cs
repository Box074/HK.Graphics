
namespace HKGraphics;

class GraphicsLibrary : ModBase<GraphicsLibrary>
{
    public static Texture2D? tatlas;
    public static Texture2D? otex;
    public override void Initialize()
    {
        if(DebugManager.IsDebug(this)) Test();
    }

    public void Test()
    {
        tatlas = Resources.FindObjectsOfTypeAll<Texture2D>().First(x => x.name == "sactx-0-2048x2048-DXT5-Title-dc83ffda");
        Stopwatch watch = new();
        watch.Start();
        otex = tatlas.Cut(new(512,512, 512, 1024), TextureFormat.RGBA32);
        otex = otex.Clone(new(1024, 2048));
        watch.Stop();
        Log($"Cut: {watch.ElapsedMilliseconds}ms");
    }
}
