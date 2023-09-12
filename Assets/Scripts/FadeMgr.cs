using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeMgr : MonoBehaviour
{
    public Image FadePanel;
    float time = 0.0f;
    float F_time = 1.0f;

    public static FadeMgr Inst = null;

    void Awake()
    {
        Inst = this;
    }

    public void Fade()
    {
        StartCoroutine(FadeFlow());
    }

    IEnumerator FadeFlow()
    {
        time = 0.0f;

        FadePanel.gameObject.SetActive(true);
        Color alpha = FadePanel.color;
        while(alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            FadePanel.color = alpha;
            yield return null;
        }

        time = 0.0f;

        yield return new WaitForSeconds(1f);
        while (alpha.a >0.0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            FadePanel.color = alpha;
            yield return null;
        }
        FadePanel.gameObject.SetActive(false);
        yield return null;
    }
}
