
namespace HKGraphics.Materials;

public abstract class EffectMaterialBase : IDisposable
{
    public abstract void Dispose();
    public abstract Material Material { get; }
    public static implicit operator Material(EffectMaterialBase mask) => mask.Material;
    public bool DontDestroy { get; set; }
    ~EffectMaterialBase()
    {
        Dispose();
    }
}
