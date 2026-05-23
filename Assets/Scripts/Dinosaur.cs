using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Dinosaur : MonoBehaviour
{
    public static event Action<Vector3> OnDinoSpawnRequested;

    public static event Action<Vector3, int> OnDinoSoulDropped;


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
    [SerializeField] private int meetingsToReproduce    =3;
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
    private Rigidbody       _rb;


        private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        
        _rb.constraints = RigidbodyConstraints.FreezeRotation
                        | RigidbodyConstraints.FreezePositionY;

        _currentHealth = maxHealth;
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
        if(_onReproductionCooldown) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, meetingRadius);
        foreach(Collider hit in hits)
        {
            if(hit.gameObject == gameObject) continue;

            Dinosaur other = hit.GetComponent<Dinosaur>();
            if(other == null || other.IsDead) continue;

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

        OnDinoSpawnRequested?.Invoke(spawnPos);

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

        OnDinoSoulDropped?.Invoke(transform.position, soulValue);

        StartCoroutine(DeathCleanup());
    }
    private IEnumerator DeathCleanup()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }


    //private IEnumerator AgeRoutine()
    //{
    //    yield return new WaitForSeconds(lifespan);
    //    if (_state != DinoState.Dead) Die();
    //}

    public bool IsDead => _state == DinoState.Dead;
    public int  Health => _currentHealth;
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
