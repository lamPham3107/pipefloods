using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiFade : SingletonMonoBehaviour<uiFade>
{
    [SerializeField] private CanvasGroup canvasGroup;

    float timer = 0;


    private void Update()
    {
        timer += Time.deltaTime;
        if (canvasGroup.alpha >= 0.9f && timer >= 2f)
        {
            timer = 0;
            FadeOut();
        }
    }

    public void FadeIn(float time = 0.75f)
    {
        timer = 0;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, time)
            .SetEase(Ease.Linear);
    }

    public void FadeOut(float time = 0.75f)
    {
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, time)
            .SetEase(Ease.Linear);
    }

    public void FadeInOut(System.Action c, float time = 0.75f)
    {
        StartCoroutine(ie_FadeInOut(c, time));
    }

    IEnumerator ie_FadeInOut(System.Action c, float time)
    {
        FadeIn(time * 0.75f);

        yield return Yielders.Get(time * 0.75f);

        if (c != null) c.Invoke();

        yield return Yielders.Get(0.2f);

        FadeOut(time);
    }
}
