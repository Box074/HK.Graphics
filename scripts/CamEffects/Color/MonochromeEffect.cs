
namespace HKGraphics.CameraEffects;

public class MonochromeEffect : CameraEffectBase<DefaultEffectBehaviour>, IColorEffect
{
    internal Monochrome _monochrome = new();
    EffectMaterialBase IColorEffect.EffectMaterial => _monochrome;
    public Vector4 Vector
    {
        get => _monochrome.Weight;
        set => _monochrome.Weight = value;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _monochrome.Dispose();
    }
    protected override void OnInitEffectBehaviour(DefaultEffectBehaviour behaviour)
    {
        behaviour.Material = _monochrome;
    }
}
