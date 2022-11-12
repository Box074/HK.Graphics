
namespace HKGraphics.Effects;

public class ColorScale : EffectMaterialBase
{
    public ColorScale(float r = 1, float b = 1, float g = 1, float a = 1)
    {
        scale = new(r, g, b, a);
    }
    private Vector4 scale = new(1, 1, 1, 1);
    public float RScale { get => scale.x; set { scale.x = value; Update(); } }
    public float GScale { get => scale.y; set { scale.y = value; Update(); } }
    public float BScale { get => scale.z; set { scale.z = value; Update(); } }
    public float AScale { get => scale.w; set { scale.w = value; Update(); } }
    public Vector4 Scale { get => scale; set { scale = value; Update(); } }
    private Material? material = null;
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        if(DontDestroy) return;
        if (material != null)
        {
            UnityEngine.Object.DestroyImmediate(material);
            material = null;
        }
    }
    private void Update()
    {
        if (material == null) return;
        material.SetVector("_ColorScale", scale);
    }
    private static readonly int shaderInstanceID = GraphicsLibrary.shaders["ColorScaleShader"].GetInstanceID();
    public override Material Material
    {
        get
        {
            if (material == null || material.shader.GetInstanceID() != shaderInstanceID)
            {
                material = new(GraphicsLibrary.shaders["ColorScaleShader"]);
            }
            Update();
            return material;
        }
    }
}
