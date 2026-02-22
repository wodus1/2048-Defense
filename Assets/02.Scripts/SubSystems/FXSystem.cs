using UnityEngine;


public class FXSystem : MonoBehaviour, ISubSystem // FX 시스템
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;

    [SerializeField] private Transform root;
    [SerializeField] private Camera worldCamera;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolingSystem = gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
    }

    public void Deinitialize()
    {
        gameManager = null;
        poolingSystem = null;
    }

    public void Play(ParticleSystem particle, RectTransform targetRect, Vector2 screenOffset, Quaternion quaternion)
    {
        CreatePool(particle);

        var fx = poolingSystem.GetFXPool(particle);
        var instance = fx.GetComponent<FXInstance>();

        instance.Initialize(this, particle, false);

        PlaceAtUIRectScreen(fx.transform, targetRect, screenOffset, quaternion);

        instance.Play();
    }

    public FXInstance PlayLoop(ParticleSystem particle, RectTransform targetRect, Vector2 screenOffset, Quaternion quaternion)
    {
        CreatePool(particle);

        var fx = poolingSystem.GetFXPool(particle);
        var instance = fx.GetComponent<FXInstance>();

        instance.Initialize(this, particle, true);

        PlaceAtUIRectScreen(fx.transform, targetRect, screenOffset, quaternion);

        instance.Play();
        return instance;
    }

    public void Return(ParticleSystem key, ParticleSystem instance)
    {
        poolingSystem.ReturnFXPool(key, instance);
    }

    public void StopLoop(FXInstance handle)
    {
        if (handle == null || poolingSystem == null) return;

        var key = handle.ParticleKey;
        var particle = handle.Particle;

        if (key == null || particle == null) return;

        handle.Stop();
        Return(key, particle);
    }

    private void CreatePool (ParticleSystem particle)
    {
        if (poolingSystem.fxPoolsContains(particle)) return;

        poolingSystem.CreateFXPool(particle, root, 1);
    }

    private void PlaceAtUIRectScreen(Transform fxTransform, RectTransform targetRect, Vector2 screenOffset, Quaternion quaternion)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, targetRect.position);
        screenPos += screenOffset;

        Vector3 worldPos = worldCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = 0f;

        fxTransform.position = worldPos;
        fxTransform.rotation = quaternion;
        fxTransform.SetParent(root, true);
    }
}
