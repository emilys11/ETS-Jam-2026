using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
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
    [SerializeField] private int maxHealth              =3;
    [SerializeField] private int soulValue              =1; //in a good world it would be 0, >:(


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
    private Rigidbody2D       _rb;
    private SpriteRenderer _sr;


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


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
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

    private void Update()
    {
        if (_state == DinoState.Dead) return;

        _stateTimer -= Time.deltaTime;
        UpdateMeetingCooldowns();
        HandleCurrentState();
        CheckNearbyDinos();
    }


    private void HandleCurrentState()
    {
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
        Vector3 dir = _wanderTarget - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;

        dir.Normalize();
        _rb.linearVelocity = dir * moveSpeed;

        if (_rb.linearVelocityX > 0f) 
        {
            _sr.flipX = true;
        }
        else if (_rb.linearVelocityX < 0f)
        {
            _sr.flipX = false;
        }
    }

    private Vector3 PickRandomWanderTarget()
    {
        Vector2 rand = UnityEngine.Random.insideUnitCircle * wanderRadius;
        return transform.position + new Vector3(rand.x, rand.y, 0f);
    }


    private void CheckNearbyDinos()
    {
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

            //if(_meetingCount >= meetingsToReproduce)
                //TriggerReproduction();
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

        GameManager.Instance.IncrementDinosKilled();
        Debug.Log(GameManager.Instance.GetDinosKilled());

        AudioHandler.Instance.PlayEffect(AudioHandler.Instance.deathEffect, "Deaths");

        StartCoroutine(DeathCleanup());
        Destroy(gameObject);
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

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Volcano") || collision.gameObject.CompareTag("Flood"))
        {
            TakeDamage(maxHealth);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
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
