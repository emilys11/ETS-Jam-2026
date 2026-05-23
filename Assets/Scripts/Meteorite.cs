using System;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [SerializeField] float meteoriteSize = 1.0f;
    [SerializeField] float maxCrashRadius = 3.0f;
    [SerializeField] float fallingSpeed = 40.0f;
    [SerializeField] float crashingSpeed = 1.0f;

    CircleCollider2D crashCollider;
    SpriteRenderer renderer;

    float minCrashRadius;
    Vector3 targetPos;
    bool isCrashing = false;
    float crashingTimePercentage = 0.0f;

    void Start()
    {
        crashCollider = GetComponent<CircleCollider2D>();
        minCrashRadius *= meteoriteSize;
        crashCollider.radius = minCrashRadius;
        crashCollider.enabled = false;

        renderer = GetComponent<SpriteRenderer>();

        gameObject.transform.localScale *= meteoriteSize;
    }

    void Update()
    {
        if (isCrashing)
        {
            Crash();
        }
        else
        {
            Move();
        }
    }

    public void Move()
    {
        float step = fallingSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
        {
            crashCollider.enabled = true;
            InitiateCrash();
        }
    }

    public void InitiateCrash()
    {
        isCrashing = true;
        renderer.enabled = false;
    }

    void Crash()
    {
        float step = Time.deltaTime * crashingSpeed;

        crashCollider.radius = Mathf.Lerp(minCrashRadius, maxCrashRadius, crashingTimePercentage);

        crashingTimePercentage += step;
        if (crashingTimePercentage >= 1.0f)
        {
            isCrashing = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log(collider.gameObject.name);
    }

    public Vector3 TargetPos { set => targetPos = value; }
}
