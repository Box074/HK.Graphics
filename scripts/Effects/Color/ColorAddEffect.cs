
namespace HKGraphics.Effects;

public class ColorAddEffect : CameraEffectBase<DefaultEffectBehaviour>, IColorEffect
{
    internal ColorAdd _add = new();
    EffectMaterialBase IColorEffect.EffectMaterial => _add;
    public Vector4 Vector
    {
        get => _add.Vector;
        set => _add.Vector = value;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _add.Dispose();
    }
    protected override void OnInitEffectBehaviour(DefaultEffectBehaviour behaviour)
    {
        behaviour.Material = _add;
    }
}
