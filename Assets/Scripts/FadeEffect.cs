using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public static FadeEffect instance;
    public Image fadePanel;
    private float fadeDuration = 2f;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = fadePanel.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, targetAlpha);
    }

}
