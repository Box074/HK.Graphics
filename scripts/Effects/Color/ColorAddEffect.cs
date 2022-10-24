
namespace HKGraphics.Effects;

public class ColorAddEffect : CameraEffectBase<ColorAddEffect.ColorAddEffectBeh>, IColorEffect
{
    internal ColorAdd _add = new();
    EffectMaterialBase IColorEffect.EffectMaterial => _add;
    public Vector4 Vector
    {
        get => _add.Vector;
        set => _add.Vector = value;
    }
    public class ColorAddEffectBeh : ColorAddEffect.EffectBehaviour<ColorAddEffect>
    {
        protected override void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, Controller._add);
        }
    }
}
