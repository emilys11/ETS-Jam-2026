using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Volcano : MonoBehaviour
{
    [SerializeField] private GameObject deathZone;
    VisualEffect volcanoVFX;

    float lifetime = 30f;
    float lifetimeTimer = 0f;

    private float eruptCooldown=6f;
    private float eruptTime = 3f;
    private float eruptTimer = 0f;

    private void Start() 
    {
        lifetimeTimer = GameManager.Instance.GetgameTime;
        volcanoVFX = GetComponent<VisualEffect>();
        volcanoVFX.Stop();
    }

    private void Update() 
    {
        if(GameManager.Instance.GetgameTime - lifetimeTimer > lifetime) 
        {
            Destroy(gameObject);
        }

        eruptTimer += Time.deltaTime;
        if (eruptTimer >= eruptCooldown)
        {
            StartCoroutine(EruptCoroutine());
            eruptTimer = 0f;
        }
    }
    IEnumerator EruptCoroutine()
    {
        AudioHandler.Instance.PlayEffect(AudioHandler.Instance.volcanoEffect, "Volcanos");
        volcanoVFX.Play();
        deathZone.gameObject.SetActive(true);
        yield return new WaitForSeconds(eruptTime);
        Unerupt();
    }

    private void Unerupt() 
    {
        volcanoVFX.Stop();
        deathZone.gameObject.SetActive(false);
    }
}
