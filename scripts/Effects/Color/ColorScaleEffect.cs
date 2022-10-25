
namespace HKGraphics.Effects;

public class ColorScaleEffect : CameraEffectBase<DefaultEffectBehaviour>, IColorEffect
{
    internal ColorScale _scale = new();
    EffectMaterialBase IColorEffect.EffectMaterial => _scale;
    public Vector4 Vector
    {
        get => _scale.Scale;
        set => _scale.Scale = value;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _scale.Dispose();
    }
    protected override void OnInitEffectBehaviour(DefaultEffectBehaviour behaviour)
    {
        behaviour.Material = _scale;
    }
}
