
namespace HKGraphics.Effects;

public abstract class CameraEffectBase<T> : MonoBehaviour where T : CameraEffectBase<T>.EffectBehaviour
{
    private T? _effect;
    protected virtual void OnEnable()
    {
        Behaviour.enabled = true;
    }
    protected virtual void OnDisable()
    {
        if (_effect == null) return;
        _effect.enabled = false;
    }
    protected virtual void OnDestroy()
    {
        if (_effect == null) return;
        Destroy(_effect);
        _effect = null;
    }
    public T Behaviour
    {
        get
        {
            if (_effect == null)
            {
                _effect = Camera.main.gameObject.AddComponent<T>();
                _effect.ctrl = this;
            }
            return _effect;
        }
    }
    public abstract class EffectBehaviour : MonoBehaviour
    {
        internal CameraEffectBase<T> ctrl = null!;

        private void Update() {
            if(ctrl == null)
            {
                DestroyImmediate(this);
            }
        }
        protected abstract void OnRenderImage(RenderTexture src, RenderTexture dest);
    }
    public abstract class EffectBehaviour<TParent> : EffectBehaviour where TParent : CameraEffectBase<T>
    {
        public TParent Controller => (TParent)ctrl;
    }
}
