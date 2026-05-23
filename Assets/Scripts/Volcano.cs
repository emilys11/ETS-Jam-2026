using System;
using System.Collections;
using UnityEngine;

public class Volcano : MonoBehaviour
{
    [SerializeField] private GameObject deathZone;

    private float eruptCooldown=15f;
    private float eruptTimer = 0f;

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
        deathZone.gameObject.SetActive(true);
        AudioHandler.Instance.PlayEffect(AudioHandler.Instance.volcanoEffect,"Volcanos");
        yield return new WaitForSeconds(5);
        Unerupt();
    }

    private void Unerupt() 
    {
        deathZone.gameObject.SetActive(false);
    }
}
