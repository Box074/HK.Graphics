
namespace HKGraphics.Effects;

public class ColorScaleEffect : CameraEffectBase<ColorScaleEffect.ColorScaleEffectBeh>, IColorEffect
{
    internal ColorScale _scale = new();
    EffectMaterialBase IColorEffect.EffectMaterial => _scale;
    public Vector4 Vector
    {
        get => _scale.Scale;
        set => _scale.Scale = value;
    }
    public class ColorScaleEffectBeh : ColorScaleEffect.EffectBehaviour<ColorScaleEffect>
    {
        protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, Controller._scale);
        }
    }
}
