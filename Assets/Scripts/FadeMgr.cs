using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeMgr : MonoBehaviour
{
    public Image FadePanel;
    float time = 0.0f;
    float fadeTime = 1.0f;


    void Awake()
    {
        StartFadeIn();
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

 
    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.2f);
    }
}
