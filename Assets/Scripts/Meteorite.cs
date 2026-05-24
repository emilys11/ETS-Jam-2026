using System;
using UnityEngine;
using UnityEngine.VFX;

public class Meteorite : MonoBehaviour
{
    [SerializeField] float meteoriteSize = 1.0f;
    [SerializeField] float maxCrashRadius = 1.0f;
    [SerializeField] float fallingSpeed = 40.0f;
    [SerializeField] float crashingSpeed = 5.0f;

    CircleCollider2D crashCollider;
    SpriteRenderer renderer;
    VisualEffect vfx;

    float minCrashRadius;
    Vector3 targetPos;
    bool isCrashing = false;
    float crashingTimePercentage = 0.0f;

    void Start()
    {
        crashCollider = GetComponent<CircleCollider2D>();
        vfx = GetComponent<VisualEffect>();

        vfx.Stop();
        minCrashRadius = crashCollider.radius;
        crashCollider.radius = minCrashRadius;
        crashCollider.enabled = false;

        renderer = GetComponent<SpriteRenderer>();

        transform.localScale *= meteoriteSize;
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
            vfx.Play();
            vfx.SetVector3("pos", gameObject.transform.position);
            crashCollider.enabled = true;
            InitiateCrash();
        } 
    }

    public void InitiateCrash()
    {
        isCrashing = true;
        renderer.enabled = false;

        if (AudioHandler.Instance != null && AudioHandler.Instance.meteorLanding != null)
            AudioHandler.Instance.PlayEffect(AudioHandler.Instance.meteorLanding, "Meteors");
    }

    void Crash()
    {
        float step = Time.deltaTime * crashingSpeed;

        crashCollider.radius = Mathf.Lerp(minCrashRadius, maxCrashRadius, crashingTimePercentage);

        crashingTimePercentage += step;
        if (crashingTimePercentage >= 1.0f)
        {
            isCrashing = false;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Dinosaur dinosaur = collider.GetComponent<Dinosaur>();
        dinosaur.Kill();
        if (AudioHandler.Instance != null && AudioHandler.Instance.crushedEffect != null)
            AudioHandler.Instance.PlayEffect(AudioHandler.Instance.crushedEffect, "CrushedDeath");
    }

    public Vector3 TargetPos { set => targetPos = value; }
}
