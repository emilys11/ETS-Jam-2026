using System.Collections;
using UnityEngine;

public class MegaDinosaur : Dinosaur
{
    [Header("Mega stats")]
    [SerializeField] private int    megaMaxHealth           = 15;
    [SerializeField] private float  auraRadius              = 8f; //yes twin he got aura twin so cool twin
    [SerializeField] private float  auraCooldownReduction   = 50f;//0.5f;
    [SerializeField] private float  reproductionPenalty     = 5f;

    [Header("Migration")]
    [SerializeField] private float migrationDelay           = 15f; //seconds after spawn to trigger it
    [SerializeField] private float migrationDuration        = 20f;

    private System.Collections.Generic.List<Dinosaur> _dinosInAura = new(); //aura so cool it makes dinos cool man, truly insane aura man

    
    protected override void Awake()
    {
        base.Awake();

        maxHealth = megaMaxHealth;
        transform.localScale = new Vector3(3f, 3f, 3f);
    }

    protected override void Update()
    {
        base.Update();
        if(IsDead) return;
        
        UpdateAura(); //aura so tuff it needs to be updated to know about it man
    }

    private void UpdateAura()
    {
        _dinosInAura.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, auraRadius);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Dinosaur other = hit.GetComponent<Dinosaur>();
            if(other == null || other.IsDead || other is MegaDinosaur) continue;


            _dinosInAura.Add(other);
            other.ReduceCooldowns(auraCooldownReduction * Time.deltaTime);
            if(_isMigrating)
                other.StartFlocking(this, _migrationTimeRemaining);
        }
        
        //Debug.Log($"Mega aura: {_dinosInAura.Count} dinos affected");
    }


    public void InitiateMigration()
    {
        //random dir
        Vector2 rand = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 direction = new Vector3(rand.x, 0f, rand.y);

    
        StartMigration(direction, migrationDuration);

        Debug.Log($"Mega {gameObject.name} has initiated migration towards {direction}");
    }


#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, auraRadius);
    }
#endif
}
