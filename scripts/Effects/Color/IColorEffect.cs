
namespace HKGraphics.Effects;

public interface IColorEffect
{
    EffectMaterialBase EffectMaterial { get; }
    Vector4 Vector { get; set; }
}
