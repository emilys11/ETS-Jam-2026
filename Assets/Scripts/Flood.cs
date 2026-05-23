using System.Collections;
using UnityEngine;

public class Flood : MonoBehaviour
{
    AnimationCurve SweepCurve;
    float m_sweepTime = 10f;


    public void Update()
    {
        if(transform.position.x == 100f) 
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        SweepCurve = AnimationCurve.Linear(0f, 100f, 10f, -100f);
        StartCoroutine(SweepCoroutine());
    }

    IEnumerator SweepCoroutine()
    {
        while (m_sweepTime > 0f)
        {
            m_sweepTime -= 0.01f;
            transform.position = new Vector3(SweepCurve.Evaluate(m_sweepTime), transform.position.y, transform.position.z);
            yield return null;
        }
    }
}
