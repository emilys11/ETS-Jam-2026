using UnityEngine;
using UnityEngine.Pool;

public class DynoSoul : MonoBehaviour
{
    private IObjectPool<DynoSoul> pool;

    public void SetPool(IObjectPool<DynoSoul> pool)
    {
        this.pool = pool;
    }

    public void Release()
    {
        pool?.Release(this);
    }
}