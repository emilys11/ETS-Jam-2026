using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Volcano : MonoBehaviour
{
    [SerializeField] private GameObject deathZone;
    VisualEffect volcanoVFX;

    private float eruptCooldown=15f;
    private float eruptTimer = 0f;

    private void Start() 
    {
        volcanoVFX = GetComponent<VisualEffect>();
        volcanoVFX.Stop();
    }

    private void Update() 
    {
        eruptTimer += Time.deltaTime;
        if (eruptTimer >= eruptCooldown)
        {
            StartCoroutine(EruptCoroutine());
            eruptTimer = 0f;
        }
    }
    IEnumerator EruptCoroutine()
    {
        volcanoVFX.Play();
        deathZone.gameObject.SetActive(true);
        AudioHandler.Instance.PlayEffect(AudioHandler.Instance.volcanoEffect,"Volcanos");
        yield return new WaitForSeconds(5);
        Unerupt();
    }

    private void Unerupt() 
    {
        volcanoVFX.Stop();
        deathZone.gameObject.SetActive(false);
    }
}
