
namespace HKGraphics.Effects;

public class Monochrome : EffectMaterialBase
{
    public Monochrome(float r = 1, float b = 1, float g = 1)
    {
        weight = new(r, g, b);
    }
    private Vector3 weight = new(1, 1, 1);
    public float R { get => weight.x; set { weight.x = value; Update(); } }
    public float G { get => weight.y; set { weight.y = value; Update(); } }
    public float B { get => weight.z; set { weight.z = value; Update(); } }
    public Vector3 Weight { get => weight; set { weight = value; Update(); } }
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
        material.SetVector("_ColorWeight", weight);
    }
    private static readonly int shaderInstanceID = GraphicsLibrary.shaders["MonochromeShader"].GetInstanceID();
    public override Material Material
    {
        get
        {
            if (material == null || material.shader.GetInstanceID() != shaderInstanceID)
            {
                material = new(GraphicsLibrary.shaders["MonochromeShader"]);
            }
            Update();
            return material;
        }
    }
}
