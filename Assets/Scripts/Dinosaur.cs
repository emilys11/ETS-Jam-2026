using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Dinosaur : MonoBehaviour
{
    public static event Action<Vector3, Dinosaur> OnDinoSpawnRequested;

    //public static event Action<Vector3, int> OnDinoSoulDropped;


    [Header("Mouvement")]
    [SerializeField] private float moveSpeed            =3f;
    [SerializeField] private float wanderRadius         =8f;
    [SerializeField] private float arrivalThreshold     =0.4f;


    [Header("Idle")]
    [SerializeField] private float idleTimeMin          =1.5f;
    [SerializeField] private float idleTimeMax          =4f;


    [Header("Wander")]
    [SerializeField] private float wanderTimeMin        =3f;
    [SerializeField] private float wanderTimeMax        =8f;


    [Header("Reproduction")]
    [SerializeField] private int meetingsToReproduce    =8;
    [SerializeField] private float meetingRadius        =1.5f;
    [SerializeField] private float meetingCooldown      =5f; //per dino
    [SerializeField] private float reproductionCooldown =20f; //after having a baby


    [Header("Health")]
    [SerializeField] protected int maxHealth            =3;
    [SerializeField] private int soulValue              =1; //in a good world it would be 0, >:(

    
    [Header("Migration")]
    [SerializeField] private float migrationSpeed       =8f;
    [SerializeField] private float migrationVariation   =15f;
    [Header("Flocking")]
    [SerializeField] private float flockFollowDistance  =6f;
    [SerializeField] private float flockSeparationRadius =1.8f;
    protected MegaDinosaur _flockLeader;
    protected bool      _isMigrating;
    protected Vector3   _migrationDirection;
    protected float _migrationTimeRemaining;
    public Vector3 MigrationDirection => _migrationDirection;

    //Lifespan / oldness later ??

    private enum DinoState{ Idle, Wandering, Dead}


    private DinoState       _state;
    private int             _currentHealth;
    private float           _stateTimer;
    private Vector3         _wanderTarget;

    private int             _meetingCount;
    private bool            _onReproductionCooldown;
    private Dictionary<Dinosaur, float> _meetingCooldowns = new(); // instance ID is the key, val is time left
                    //was int, float
    private Rigidbody       _rb;


    private int   _baseMeetingsToReproduce;
    private float _baseReproductionCooldown;
    private float _baseMeetingCooldown;
    private bool  _inBabyBoom;
    private Dinosaur _parent; //lets not nuke the arcade
    private List<Dinosaur> _children = new(); //again man 
    public void SetParent(Dinosaur parent)
    {
        _parent = parent;
    }
    public void AddChild(Dinosaur child)
    {
        _children.Add(child);
    }


    


    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        
        _rb.constraints = RigidbodyConstraints.FreezeRotation
                        | RigidbodyConstraints.FreezePositionY;

        _currentHealth = maxHealth;

        _baseMeetingsToReproduce  = meetingsToReproduce;
        _baseReproductionCooldown = reproductionCooldown;
        _baseMeetingCooldown      = meetingCooldown;

        _onReproductionCooldown   = true;
        StartCoroutine(ReproductionCooldownRoutine());
    }

    private void Start()
    {
        EnterIdle();

        //if (lifespan > 0f)
        //    StartCoroutine(AgeRoutine());
        //    return;
    }

    protected virtual void Update()
    {
        if (_state == DinoState.Dead) return;

        _stateTimer -= Time.deltaTime;
        UpdateMeetingCooldowns();
        HandleCurrentState();
        CheckNearbyDinos();
    }


    private void HandleCurrentState()
    {
        if (_isMigrating)
        {
            MoveTowardTarget();
            return;
        }

        switch(_state)
        {
            case DinoState.Idle:
                if(_stateTimer <= 0f)
                    EnterWander();
                break;

            case DinoState.Wandering:
                MoveTowardTarget();
                bool arrived = Vector3.Distance(transform.position, _wanderTarget) <= arrivalThreshold;
                if (arrived || _stateTimer <= 0f)
                    EnterIdle();
                break;        
        }
    }

    private void EnterIdle()
    {
        _state              =DinoState.Idle;
        _stateTimer         =UnityEngine.Random.Range(idleTimeMin, idleTimeMax);
        _rb.linearVelocity  =Vector3.zero;
    }
    private void EnterWander()
    {
        _state              =DinoState.Wandering;
        _stateTimer         =UnityEngine.Random.Range(wanderTimeMin, wanderTimeMax);
        _wanderTarget       =PickRandomWanderTarget();
    }

    private void MoveTowardTarget()
    {
        Vector3 dir;
        if (_isMigrating && _flockLeader != null)
        {
            dir = ComputeFlockSteering();
        }
        else if (_isMigrating)
        { //for mega
            dir  = _migrationDirection;
        }
        else
        {
            dir = _wanderTarget - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;

            dir.Normalize();
        }
        
        
        _rb.linearVelocity = dir * (_isMigrating ? migrationSpeed : moveSpeed);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 8f
        );
    }

    private Vector3 PickRandomWanderTarget()
    {
        Vector2 rand = UnityEngine.Random.insideUnitCircle * wanderRadius;
        return transform.position + new Vector3(rand.x, 0f, rand.y);
    }


    private void CheckNearbyDinos()
    {
        if(_isMigrating) return;
        if(_onReproductionCooldown) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, meetingRadius);
        foreach(Collider hit in hits)
        {
            if(hit.gameObject == gameObject) continue;

            Dinosaur other = hit.GetComponent<Dinosaur>();
            if(other == null || other.IsDead) continue;
            if(other == _parent) continue;
            if(_children.Contains(other)) continue;

            if(_meetingCooldowns.ContainsKey(other)) continue;

            _meetingCooldowns[other] = meetingCooldown;
            _meetingCount++;

            if(_meetingCount >= meetingsToReproduce)
                TriggerReproduction();
        }
    }
    private void UpdateMeetingCooldowns()
    {
        var toRemove = new List<Dinosaur>();

        foreach (var key in new List<Dinosaur>(_meetingCooldowns.Keys))
        {
            _meetingCooldowns[key] -= Time.deltaTime;
            if (_meetingCooldowns[key] <= 0f)
                toRemove.Add(key);
        }

        foreach (Dinosaur key in toRemove)
            _meetingCooldowns.Remove(key);
    }

    private void TriggerReproduction()
    {
        Debug.Log($"{gameObject.name} wants to spawn a baby");
        _meetingCount = 0;
        _onReproductionCooldown = true;

        Vector3 spawnPos = transform.position + new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            0f,
            UnityEngine.Random.Range(-1f,1f)
            );

        OnDinoSpawnRequested?.Invoke(spawnPos, this);

        StartCoroutine(ReproductionCooldownRoutine());
    }
    private IEnumerator ReproductionCooldownRoutine()
    {
        yield return new WaitForSeconds(reproductionCooldown);
        _onReproductionCooldown = false;
    }

   
   public void TakeDamage(int amount = 1)//called by plyr
    {
         if (_state == DinoState.Dead) return;
            _currentHealth -= amount;
        if (_currentHealth <= 0) Die();
    }

    public void Kill() => Die();

    private void Die()
    {
        if(_state == DinoState.Dead) return;

        _state              =DinoState.Dead;
        _rb.linearVelocity  =Vector3.zero;

        DynoSoulsEvents.DinoKill(transform.position); //animation of coins
        DynoSoulsEvents.GainCoins(soulValue);

        StartCoroutine(DeathCleanup());
    }
    private IEnumerator DeathCleanup()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }

    public void StartBabyBoom(float duration)
    {
        if (_inBabyBoom) return;
        StartCoroutine(BabyBoomRoutine(duration));
    }

    private IEnumerator BabyBoomRoutine(float duration)
    {
        _inBabyBoom           = true;
        meetingsToReproduce   = 1;
        reproductionCooldown  = 3f;
        meetingCooldown       = 1f;

        yield return new WaitForSeconds(duration);

        meetingsToReproduce   = _baseMeetingsToReproduce;
        reproductionCooldown  = _baseReproductionCooldown;
        meetingCooldown       = _baseMeetingCooldown;
        _inBabyBoom           = false;
    }

    public void ReduceCooldowns(float amount)
    {
        foreach(var key in new List<Dinosaur>(_meetingCooldowns.Keys))
        {
            _meetingCooldowns[key] = Mathf.Max(0f, _meetingCooldowns[key] - amount);
            //Debug.Log($"{gameObject.name} cooldown avec {key.name}: {_meetingCooldowns[key]}");
        }

        //Debug.Log($"{gameObject.name} cooldowns reduced by {amount}");
    }

    public void StartMigration(Vector3 direction, float duration)
    {
        if (_isMigrating) return; // already in a flock

        // little variations
        float variation = UnityEngine.Random.Range(-migrationVariation, migrationVariation);
        _migrationDirection = Quaternion.Euler(0f, variation, 0f) * direction;
        _migrationDirection.Normalize();

        _isMigrating = true;
        StartCoroutine(MigrationRoutine(duration));
    }

    private IEnumerator MigrationRoutine(float duration)
    {
        _migrationTimeRemaining = duration;
        float driftAngle = 0f;
        float driftTarget = UnityEngine.Random.Range(-30f, 30f); // où il veut aller

        while (_migrationTimeRemaining > 0f)
        {
            // Drift qui glisse vers sa cible, puis la cible change lentement
            driftTarget += UnityEngine.Random.Range(-8f, 8f) * Time.deltaTime;
            driftTarget  = Mathf.Clamp(driftTarget, -40f, 40f);
            driftAngle   = Mathf.Lerp(driftAngle, driftTarget, Time.deltaTime * 0.5f);

            _migrationDirection = Quaternion.Euler(0f, driftAngle * Time.deltaTime, 0f) * _migrationDirection;
            _migrationDirection.Normalize();

            _migrationTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        _isMigrating = false;
        EnterWander();
    }
/*
    private IEnumerator MigrationRoutine(float duration)
    {
        _migrationTimeRemaining = duration;
        while (_migrationTimeRemaining > 0f)
        {
            _migrationTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        _isMigrating = false;
        EnterWander();
    }
*/

    public void StartFlocking(MegaDinosaur leader, float duration)
    {
        if (_isMigrating) return;
        _flockLeader            = leader;
        _isMigrating            = true;
        _migrationTimeRemaining = duration;
        StartCoroutine(FlockingRoutine());
    }

    private IEnumerator FlockingRoutine()
    {
        while (_migrationTimeRemaining > 0f)
        {
            if (_flockLeader == null || _flockLeader.IsDead) break;
            _migrationTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        _flockLeader = null;
        _isMigrating = false;
        EnterWander();
    }

    private Vector3 ComputeFlockSteering()
    {
        Vector3 toLeader = _flockLeader.transform.position - transform.position;
        toLeader.y = 0f;
        float dist = toLeader.magnitude;

        // Séparation : éviter de stacker sur les autres dinos
        Vector3 sep = Vector3.zero;
        int sepCount = 0;
        Collider[] nearby = Physics.OverlapSphere(transform.position, flockSeparationRadius);
        foreach (Collider c in nearby)
        {
            if (c.gameObject == gameObject) continue;
            if (!c.TryGetComponent<Dinosaur>(out _)) continue;
            Vector3 away = transform.position - c.transform.position;
            away.y = 0f;
            if (away.sqrMagnitude > 0f)
                sep += away.normalized / Mathf.Max(away.magnitude, 0.1f);
            sepCount++;
        }
        if (sepCount > 0) sep /= sepCount;

        Vector3 steering;
        if (dist > flockFollowDistance)
            // Trop loin du leader : le rattraper (cohésion forte)
            steering = toLeader.normalized * 0.8f + sep.normalized * 0.2f;
        else
            // En formation : suivre la direction du leader + éviter les voisins
            steering = _flockLeader.MigrationDirection * 0.6f + sep.normalized * 0.4f;

        steering.y = 0f;
        return steering.sqrMagnitude > 0.01f ? steering.normalized : _flockLeader.MigrationDirection;
    }


    //private IEnumerator AgeRoutine()
    //{
    //    yield return new WaitForSeconds(lifespan);
    //    if (_state != DinoState.Dead) Die();
    //}

    public bool IsDead => _state == DinoState.Dead;
    public int  Health => _currentHealth;

    [ContextMenu("Test Kill")]
    private void TestKill() => Kill();

    [ContextMenu("Test Damage")]  
    private void TestDamage() => TakeDamage(1);

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
         Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, _wanderTarget);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, meetingRadius);  // zone de rencontre

        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, wanderRadius);   // zone de wander
    }
#endif
}
