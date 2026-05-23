using System;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [SerializeField] float meteoriteSize = 1.0f;
    [SerializeField] float maxCrashRadius = 3.0f;
    [SerializeField] float fallingSpeed = 40.0f;
    [SerializeField] float crashingSpeed = 1.0f;

    SphereCollider crashCollider;
    MeshRenderer meshRenderer;

    float minCrashRadius;
    Vector3 initialPos;
    Vector3 targetPos;
    float totalDistanceToTravel;
    bool isCrashing = false;
    float crashingTimePercentage = 0.0f;

    void Start()
    {
        crashCollider = GetComponent<SphereCollider>();
        crashCollider.radius = minCrashRadius;

        meshRenderer = GetComponent<MeshRenderer>();

        initialPos = transform.position;
        totalDistanceToTravel = Vector3.Distance(new Vector3(targetPos.x, 0.0f, targetPos.z), new Vector3(initialPos.x, 0.0f, initialPos.z));
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

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPos.x, transform.position.y, targetPos.z), step);
        float lerpedPosY = Mathf.Lerp(
            initialPos.y,
            targetPos.y,
            Vector3.Distance(
                new Vector3(initialPos.x, 0.0f, initialPos.z),
                new Vector3(transform.position.x, 0.0f, transform.position.z)
            ) / totalDistanceToTravel
        );
        transform.position = new Vector3(transform.position.x, lerpedPosY, transform.position.z);
        if (Vector3.Distance(transform.position, targetPos) <= 0.01f)
        {
            InitiateCrash();
        }
    }

    public void InitiateCrash()
    {
        isCrashing = true;
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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (!isCrashing)
        {
            if (other.gameObject.layer == LayerMask.GetMask("Terrain"))
            {
                InitiateCrash();
                meshRenderer.enabled = false;
            }
        }
    }

    public Vector3 TargetPos { set => targetPos = value; }
}
