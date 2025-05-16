using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFadeIn : MonoBehaviour
{

    void Start()
    {
        if(Fade.Instance != null)
        {
            Fade.Instance.FadeIn();
        }
    }


}
