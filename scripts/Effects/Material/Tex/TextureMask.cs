
namespace HKGraphics.Effects;

public class TextureMask : EffectMaterialBase
{
    public record struct VectorGroup(Vector2 a, Vector2 b, Vector2 c);
    ~TextureMask()
    {
        Dispose();
    }
    public TextureMask(Vector2Int maskSize, params VectorGroup[] groups) : this(false, maskSize, groups)
    {

    }
    
    public TextureMask(bool keepOutside, Vector2Int maskSize, params VectorGroup[] groups)
    {
        this.keepOutside = keepOutside;
        maskTex = new(maskSize.x, maskSize.y, 0, RenderTextureFormat.R8);
        RenderTexture rtemp = new(maskSize.x, maskSize.y, 0, RenderTextureFormat.R8);
        Material mat = new(GraphicsLibrary.shaders["TextureMaskMakerShader"]);
        foreach(var v in groups)
        {
            mat.SetVector("_P0", new(v.a.x / maskSize.x, v.a.y / maskSize.y));
            mat.SetVector("_P1", new(v.b.x / maskSize.x, v.b.y / maskSize.y));
            mat.SetVector("_P2", new(v.c.x / maskSize.x, v.c.y / maskSize.y));
            Graphics.Blit(rtemp, maskTex, mat);
            Graphics.Blit(maskTex, rtemp);
        }
        rtemp.Release();
        UnityEngine.Object.DestroyImmediate(mat);
    }
    private RenderTexture? maskTex;
    private Material? maskMat;
    private bool keepOutside = false;
    private static readonly int applyShaderInstanceID = GraphicsLibrary.shaders["TextureMaskApplyShader"].GetInstanceID();
    public override Material Material
    {
        get
        {
            if(maskTex == null) throw new InvalidOperationException();
            if(maskMat == null || maskMat.shader.GetInstanceID() != applyShaderInstanceID)
            {
                maskMat = new(GraphicsLibrary.shaders["TextureMaskApplyShader"]);
            }
            maskMat.SetTexture("_MaskTex", maskTex);
            maskMat.SetInt("_CutCover", keepOutside ? 1 : 0);
            return maskMat;
        }
    }
    public bool KeepOutside
    {
        get => keepOutside;
        set 
        {
            keepOutside = value;
            if(maskMat != null)
            {
                maskMat.SetInt("_CutCover", keepOutside ? 1 : 0);
            }
        }
    }
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        if(DontDestroy) return;
        if(maskTex != null)
        {
            maskTex.Release();
            maskTex = null;
        }
        if(maskMat != null)
        {
            UnityEngine.Object.DestroyImmediate(maskMat);
            maskMat = null;
        }
    }
}
