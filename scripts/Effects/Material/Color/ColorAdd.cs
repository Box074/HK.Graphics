
namespace HKGraphics.Effects;

public class ColorAdd : EffectMaterialBase
{
    public ColorAdd(float r = 0, float b = 0, float g = 0, float a = 0)
    {
        scale = new(r, g, b, a);
    }
    private Vector4 scale = new(1, 1, 1, 1);
    public float R { get => scale.x; set { scale.x = value; Update(); } }
    public float G { get => scale.y; set { scale.y = value; Update(); } }
    public float B { get => scale.z; set { scale.z = value; Update(); } }
    public float A { get => scale.w; set { scale.w = value; Update(); } }
    public Vector4 Vector { get => scale; set { scale = value; Update(); } }
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
    private static readonly int shaderInstanceID = GraphicsLibrary.shaders["ColorAddShader"].GetInstanceID();
    public override Material Material
    {
        get
        {
            if (material == null || material.shader.GetInstanceID() != shaderInstanceID)
            {
                material = new(GraphicsLibrary.shaders["ColorAddShader"]);
            }
            Update();
            return material;
        }
    }
}
