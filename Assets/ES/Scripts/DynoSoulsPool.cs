using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class DynoSoulsPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public DynoSoul DSPrefab;

    private IObjectPool<DynoSoul> objectPool;

    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defaultCapacity = 500;
    [SerializeField] private int maxSize = 100000;

    [Header("Floating DS Settings")]
    public float yOffset = 1.2f;
    public float floatDuration = 1.5f;
    public float yEnd = 2f;

    void Awake()
    {
        objectPool = new ObjectPool<DynoSoul>(
            CreateDS,
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyPooledObject,
            collectionCheck,
            defaultCapacity,
            maxSize
        );
    }

    void OnEnable()
    {
        DynoSoulsEvents.OnDinoKill += TriggerFloatingDS;
    }

    void OnDisable()
    {
        DynoSoulsEvents.OnDinoKill -= TriggerFloatingDS;
    }

    private DynoSoul CreateDS()
    {
        return Instantiate(DSPrefab);
    }

    private void OnGetFromPool(DynoSoul obj)
    {
        obj.gameObject.SetActive(true);
        obj.SetPool(objectPool); 
    }

    private void OnReleaseToPool(DynoSoul obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(DynoSoul obj)
    {
        Destroy(obj.gameObject);
    }

    public void TriggerFloatingDS(Vector3 v)
    {
        StartCoroutine(FloatingDS(v));
    }

    public IEnumerator FloatingDS(Vector3 v)
    {
        float elapsed = 0f;

        DynoSoul ds = objectPool.Get();

        Vector3 startPosition = new Vector3(v.x, v.y + yOffset, v.z);
        Vector3 endPosition = startPosition + new Vector3(0f, yEnd, 0f);

        ds.transform.position = startPosition;

        while (elapsed < floatDuration)
        {
            ds.transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                elapsed / floatDuration
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        ds.transform.position = endPosition;

        objectPool.Release(ds);
    }
}