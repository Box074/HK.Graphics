
namespace HKGraphics.Effects;

public abstract class EffectBase<T> : IDisposable where T : UnityEngine.Object
{
    public abstract void Dispose();
    public abstract T UnityObject { get; }
    public static implicit operator T(EffectBase<T> self) => self.UnityObject;
    public bool DontDestroy { get; set; }
    ~EffectBase()
    {
        Dispose();
    }
}

