using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Dinosaur : MonoBehaviour
{
    public static event Action<Vector3, Dinosaur> OnDinoSpawnRequested;

    [Header("Mouvement")]
    [SerializeField] protected float moveSpeed        = 3f;
    [SerializeField] protected float wanderRadius     = 8f;
    [SerializeField] protected float arrivalThreshold = 0.4f;

    [Header("Idle")]
    [SerializeField] protected float idleTimeMin      = 1.5f;
    [SerializeField] protected float idleTimeMax      = 4f;

    [Header("Wander")]
    [SerializeField] protected float wanderTimeMin    = 3f;
    [SerializeField] protected float wanderTimeMax    = 8f;

    [Header("Reproduction")]
    [SerializeField] protected int meetingsToReproduce    = 4;
    [SerializeField] protected float meetingRadius        = 1.5f;
    [SerializeField] protected float meetingCooldown      = 5f; 
    [SerializeField] protected float reproductionCooldown = 20f; 

    [Header("Health")]
    [SerializeField] public int maxHealth              =3;
    [SerializeField] private int soulValue              =50; //in a good world it would be 0, >:(

    [Header("Migration")]
    [SerializeField] protected float migrationSpeed       = 8f;
    [SerializeField] protected float migrationVariation   = 15f;
    
    [Header("Flocking")]
    [SerializeField] protected float flockFollowDistance  = 6f;
    [SerializeField] protected float flockSeparationRadius= 1.8f;
    
    [Header("Visuals")]
    [SerializeField] private Sprite[] dinoSprites = new Sprite[4];

    protected MegaDinosaur _flockLeader;
    protected bool         _isMigrating;
    protected Vector3      _migrationDirection;
    protected Vector3      _obstacleAvoidance;
    protected Vector3      _baseMigrationDirection;
    protected float        _migrationTimeRemaining;
    public Vector3         MigrationDirection => _migrationDirection;

    protected enum DinoState { Idle, Wandering, Dead }

    protected DinoState       _state;
    protected int             _currentHealth;
    protected float           _stateTimer;
    protected Vector3         _wanderTarget;

    private int             _meetingCount;
    private bool            _onReproductionCooldown;
    private Dictionary<Dinosaur, float> _meetingCooldowns = new(); 

    protected Rigidbody2D     _rb;
    protected SpriteRenderer  _sr;
    protected Animator _animator;

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

    
    private float _stuckTimer = 0f;
    
    
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        if (dinoSprites.Length > 0)
            _sr.sprite = dinoSprites[UnityEngine.Random.Range(0, dinoSprites.Length)];

        _currentHealth = maxHealth;

        _baseMeetingsToReproduce  = meetingsToReproduce;
        _baseReproductionCooldown = reproductionCooldown;
        _baseMeetingCooldown      = meetingCooldown;

        _onReproductionCooldown   = true;
        StartCoroutine(ReproductionCooldownRoutine());
    }

    protected virtual void Start()
    {
        EnterIdle();
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
                if(_stateTimer <= 0f) {
                    EnterWander();
                }
                _animator.SetBool("isWandering", false);
                break;

            case DinoState.Wandering:
                MoveTowardTarget();
                bool arrived = Vector3.Distance(transform.position, _wanderTarget) <= arrivalThreshold;
                if (arrived || _stateTimer <= 0f) EnterIdle();
                _animator.SetBool("isWandering", true);
                break;        
        }
    }

    private void EnterIdle()
    {
        _state = DinoState.Idle;
        _stateTimer = UnityEngine.Random.Range(idleTimeMin, idleTimeMax);
        _rb.linearVelocity = Vector2.zero; // linearVelocity Unity 6
    }

    protected void EnterWander()
    {
        _state = DinoState.Wandering;
        _stateTimer = UnityEngine.Random.Range(wanderTimeMin, wanderTimeMax);
        _wanderTarget = PickRandomWanderTarget();
        _stuckTimer = 0f;
    }

    private void MoveTowardTarget()
    {
        Vector3 dir;
        // _animator.SetBool("isWandering", true);
        if (_isMigrating && _flockLeader != null) {
            dir = ComputeFlockSteering();
        }
        else if (_isMigrating) {
            dir = _migrationDirection;
        }
        else {/*
            dir = _wanderTarget - transform.position;
            dir.z = 0f; // On annule Z en 2D !
            if (dir.sqrMagnitude < 0.01f) return;
            dir.Normalize();*/
             // COMPORTEMENT WANDER (Petits dinos)
            dir = _wanderTarget - transform.position;
            dir.z = 0f; 
            if (dir.sqrMagnitude < 0.01f) return;
            dir.Normalize();

            // On applique la glissade douce ici !
            dir += _obstacleAvoidance * 1.5f;
            dir.z = 0f;
            if (dir.sqrMagnitude > 0.01f) dir.Normalize();

            // La force de glissade s'estompe quand on s'éloigne du mur
            _obstacleAvoidance = Vector3.Lerp(_obstacleAvoidance, Vector3.zero, Time.deltaTime * 5f);
        }/*
        dir += _obstacleAvoidance * 1.5f; // On dévie la direction loin du mur
        dir.z = 0f;
        if (dir.sqrMagnitude > 0.01f) dir.Normalize();

        // On dissipe l'esquive progressivement pour qu'il reprenne sa route après le mur
        _obstacleAvoidance = Vector3.Lerp(_obstacleAvoidance, Vector3.zero, Time.deltaTime * 5f);
        */

        _rb.linearVelocity = dir * (_isMigrating ? migrationSpeed : moveSpeed);

        // Flip du sprite
        if (_rb.linearVelocity.x > 0f) _sr.flipX = true;
        else if (_rb.linearVelocity.x < 0f) _sr.flipX = false;
    }

    private Vector3 PickRandomWanderTarget()
    {
        /*
        Vector2 rand = UnityEngine.Random.insideUnitCircle * wanderRadius;
        return transform.position + new Vector3(rand.x, rand.y, 0f); // X et Y pour la 2D
    */

    float randomX = UnityEngine.Random.Range(0f, 60f); 
    float randomY = UnityEngine.Random.Range(0f, 35f); 
    
    return new Vector3(randomX, randomY, 0f);
    
    }

    private void CheckNearbyDinos()
    {
        if(_isMigrating || _onReproductionCooldown) return;

        // PHYSICS 2D ICI !
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meetingRadius);
        foreach(Collider2D hit in hits)
        {
            if(hit.gameObject == gameObject) continue;

            Dinosaur other = hit.GetComponent<Dinosaur>();
            if(other == null || other.IsDead || other == _parent || _children.Contains(other)) continue;
            if(_meetingCooldowns.ContainsKey(other)) continue;

            _meetingCooldowns[other] = meetingCooldown;
            _meetingCount++;

            if (_meetingCount >= meetingsToReproduce) TriggerReproduction();
        }
    }

    private void UpdateMeetingCooldowns()
    {
        var toRemove = new List<Dinosaur>();
        foreach (var key in new List<Dinosaur>(_meetingCooldowns.Keys))
        {
            _meetingCooldowns[key] -= Time.deltaTime;
            if (_meetingCooldowns[key] <= 0f) toRemove.Add(key);
        }
        foreach (Dinosaur key in toRemove) _meetingCooldowns.Remove(key);
    }

    private void TriggerReproduction()
    {
        _meetingCount = 0;
        _onReproductionCooldown = true;

        Vector3 spawnPos = transform.position + new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f), // X et Y !
            0f
        );

        OnDinoSpawnRequested?.Invoke(spawnPos, this);
        StartCoroutine(ReproductionCooldownRoutine());
    }

    private IEnumerator ReproductionCooldownRoutine()
    {
        yield return new WaitForSeconds(reproductionCooldown);
        _onReproductionCooldown = false;
    }

    public void ReduceCooldowns(float amount)
    {
        foreach(var key in new List<Dinosaur>(_meetingCooldowns.Keys))
            _meetingCooldowns[key] = Mathf.Max(0f, _meetingCooldowns[key] - amount);
    }

    // ========== MIGRATION & FLOCKING (Adapté 2D) ==========

    public void StartMigration(Vector3 direction, float duration)
    {
        if (_isMigrating) return; 

        float variation = UnityEngine.Random.Range(-migrationVariation, migrationVariation);
        // Rotation sur l'axe Z pour la 2D !
        _migrationDirection = Quaternion.Euler(0f, 0f, variation) * direction;
        _migrationDirection.Normalize();

        _baseMigrationDirection = _migrationDirection;

        _isMigrating = true;
        StartCoroutine(MigrationRoutine(duration));
    }

    private IEnumerator MigrationRoutine(float duration)
    {
        _migrationTimeRemaining = duration;
        Vector3 originalDirection = _migrationDirection;
        float driftVelocity = 0f;

        while (_migrationTimeRemaining > 0f)
        {
            driftVelocity += UnityEngine.Random.Range(-15f, 15f) * Time.deltaTime;
            driftVelocity  = Mathf.Clamp(driftVelocity, -20f, 20f);

            // Vector3.forward = axe Z en 2D !
            float deviation = Vector3.SignedAngle(originalDirection, _migrationDirection, Vector3.forward);
            if (Mathf.Abs(deviation) > 45f)
                driftVelocity -= Mathf.Sign(deviation) * 15f;

            _migrationDirection = Quaternion.Euler(0f, 0f, driftVelocity * Time.deltaTime) * _migrationDirection;
            _migrationDirection.Normalize();

            _migrationTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        _isMigrating = false;
        EnterWander();
    }

    public void StartFlocking(MegaDinosaur leader, float duration)
    {
        if (_isMigrating) return;
        _flockLeader = leader;
        _isMigrating = true;
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
        toLeader.z = 0f;
        float dist = toLeader.magnitude;

        Vector3 sep = Vector3.zero;
        int sepCount = 0;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, flockSeparationRadius);
        foreach (Collider2D c in nearby)
        {
            if (c.gameObject == gameObject) continue;
            if (!c.TryGetComponent<Dinosaur>(out _)) continue;
            Vector3 away = transform.position - c.transform.position;
            away.z = 0f;
            if (away.sqrMagnitude > 0f)
                sep += away.normalized / Mathf.Max(away.magnitude, 0.1f);
            sepCount++;
        }
        if (sepCount > 0) sep /= sepCount;

        Vector3 steering;
        if (dist > flockFollowDistance)
            steering = toLeader.normalized * 0.8f + sep.normalized * 0.2f;
        else
            steering = _flockLeader.MigrationDirection * 0.6f + sep.normalized * 0.4f;

        steering.z = 0f;
        return steering.sqrMagnitude > 0.01f ? steering.normalized : _flockLeader.MigrationDirection;
    }

    // ========== SANTE & DEGATS ==========

    public void TakeDamage(int amount = 1)
    {
        if (_state == DinoState.Dead) return;
        _currentHealth -= amount;
        if (_currentHealth <= 0) Die();
    }

    public void Kill() => Die();

   private void Die()
    {
        if(_state == DinoState.Dead) return;

        _state = DinoState.Dead;
        _rb.linearVelocity = Vector2.zero;

        // On appelle les événements statiques directement !
        DynoSoulsEvents.DinoKill(transform.position); 
        DynoSoulsEvents.GainCoins(soulValue);

        if(GameManager.Instance != null)
            GameManager.Instance.IncrementDinosKilled();

        StartCoroutine(DeathCleanup());
        Destroy(gameObject, 0.1f);
    }
    
    private IEnumerator DeathCleanup()
    {
        gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
    }

    public void StartBabyBoom(float duration)
    {
        if (_inBabyBoom) return;
        StartCoroutine(BabyBoomRoutine(duration));
    }

    private IEnumerator BabyBoomRoutine(float duration)
    {
        _inBabyBoom = true;
        meetingsToReproduce = 1;
        reproductionCooldown = 3f;
        meetingCooldown = 1f;

        yield return new WaitForSeconds(duration);

        meetingsToReproduce = _baseMeetingsToReproduce;
        reproductionCooldown = _baseReproductionCooldown;
        meetingCooldown = _baseMeetingCooldown;
        _inBabyBoom = false;
    }

    public bool IsDead => _state == DinoState.Dead;
    public int  Health => _currentHealth;

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Volcano") || collision.gameObject.CompareTag("Flood"))
        {
            TakeDamage(maxHealth);
            if (AudioHandler.Instance != null && AudioHandler.Instance.charredEffect != null)
                AudioHandler.Instance.PlayEffect(AudioHandler.Instance.charredEffect, "CharredDeath");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // On ignore les collisions entre dinos
        if (collision.gameObject.TryGetComponent<Dinosaur>(out _)) return;

        Vector3 normal = collision.GetContact(0).normal;
        normal.z = 0f;

        if (_isMigrating)
        {
            // MEGA DINO ET MIGRATION : Rebond net façon DVD
            if (Vector3.Dot(_migrationDirection, normal) < 0f)
            {
                _migrationDirection = Vector3.Reflect(_migrationDirection, normal).normalized;
                _baseMigrationDirection = _migrationDirection; // Met à jour la coroutine
            }
        }
        else if (_state == DinoState.Wandering)
        {
            // PETITS DINOS : On charge la glissade !
            _obstacleAvoidance = normal; 
        }
    }
/*
    private void OnCollisionStay2D(Collision2D collision)
    {
        // On ne rebondit pas sur les autres dinos (le flocking gère déjà ça)
        if (collision.gameObject.TryGetComponent<Dinosaur>(out _)) return;

        // On récupère "l'angle" du mur qu'on est en train de toucher
        Vector2 normal = collision.GetContact(0).normal;
        _obstacleAvoidance = new Vector3(normal.x, normal.y, 0f);

        // Comportement intelligent pour le Wander (les petits dinos)
        if (!_isMigrating && _state == DinoState.Wandering)
        {
            // Si le dino essaie d'avancer droit dans le mur (angle opposé), il annule son trajet
            Vector3 intentDir = (_wanderTarget - transform.position).normalized;
            if (Vector3.Dot(intentDir, _obstacleAvoidance) < -0.8f)
            {
                EnterIdle(); // Au lieu de frotter le mur, il s'arrête et choisira un meilleur chemin !
            }
        }
    }
*/
#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, _wanderTarget);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, meetingRadius); 

        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, wanderRadius);  
    }
#endif
}