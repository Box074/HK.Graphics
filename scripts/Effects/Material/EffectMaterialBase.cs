
namespace HKGraphics.Effects;

public abstract class EffectMaterialBase : EffectBase<Material>
{
    public abstract Material Material { get; }
    public override Material UnityObject => Material;
}
