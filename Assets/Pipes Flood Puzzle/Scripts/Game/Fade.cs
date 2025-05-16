using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public static Fade Instance;
    public Image fadeImage;
    public float fadeDuration = 1f;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void setAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("Fadein");
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            float alpha =  t/ fadeDuration;
            Debug.Log(alpha);
            setAlpha(alpha);
            yield return null;
        }
        setAlpha(0);
    }
    public IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            setAlpha(alpha);
            yield return null;
        }
        setAlpha(1);
        
    }

}
