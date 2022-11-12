
namespace HKGraphics.CameraEffects;

public static class ColorEffectHelper
{
    public static CoroutineInfo Lerp(this IColorEffect effect, Vector4 from, Vector4 to, float time, bool ignoreTimeScale = false)
    {
        IEnumerator LerpCor()
        {
            //GraphicsLibrary.Instance.Log($"Lerp: {from} {to} {time}");
            var esec = 1 / time;
            var p = 0f;
            while(p < 1)
            {
                yield return null;
                if(ignoreTimeScale)
                {
                    p += esec * Time.unscaledDeltaTime;
                }
                else
                {
                    p += esec * Time.deltaTime;
                }
                p = Mathf.Clamp01(p);
                effect.Vector = Vector4.Lerp(from, to, p);
            }
        }
        return LerpCor().StartCoroutine();
    }
    public static CoroutineInfo Lerp(this IColorEffect effect, Vector4 to, float time, bool ignoreTimeScale = false) => effect.Lerp(effect.Vector, to, time, ignoreTimeScale);
}
