using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MegaDinosaur : Dinosaur
{
    [Header("Mega stats")]
    [SerializeField] private int megaMaxHealth = 15;
    [SerializeField] private float auraRadius = 8f;
    [SerializeField] private float auraCooldownReduction = 50f;

    [Header("Migration")]
    [SerializeField] private float migrationDuration = 20f;

    private List<Dinosaur> _dinosInAura = new();

    // --- POOL EXTRA VARIABLE ---
    private IObjectPool<MegaDinosaur> _megaOriginPool;

    
    public void ConfigurePool(IObjectPool<MegaDinosaur> pool)
    {
        _megaOriginPool = pool;
    }

    protected override void Awake()
    {
        
        base.Awake();

        MaxHealth = megaMaxHealth;
        _currentHealth = MaxHealth;
        transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
    }

    
    protected override void OnEnable()
    {
        base.OnEnable();
        _dinosInAura.Clear();
    }

    protected override void Update()
    {
        base.Update();
        if (IsDead) return;
        UpdateAura();
    }

    private void UpdateAura()
    {
        _dinosInAura.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, auraRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            Dinosaur other = hit.GetComponent<Dinosaur>();
            if (other == null || other.IsDead || other is MegaDinosaur) continue;

            _dinosInAura.Add(other);
            other.ReduceCooldowns(auraCooldownReduction * Time.deltaTime);
            if (_isMigrating)
                other.StartFlocking(this, _migrationTimeRemaining);
        }
    }

    public void InitiateMigration()
    {
        Vector2 rand = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 direction = new Vector3(rand.x, rand.y, 0f);
        StartMigration(direction, migrationDuration);
        Debug.Log($"Mega {gameObject.name} migre vers {direction}");
    }

    
    protected override void OnDisable()
    {
        base.OnDisable();

        if (_state == DinoState.Dead)
        {
            if (_megaOriginPool != null)
            {
                _megaOriginPool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, auraRadius);
    }
#endif
}