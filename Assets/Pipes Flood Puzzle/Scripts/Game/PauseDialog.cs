using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseDialog : MonoBehaviour
{
    public GameObject pauseDialog;
    public static PauseDialog ins;
    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowPauseDialog()
    {
        if (pauseDialog != null)
        {
            pauseDialog.SetActive(true);
        }
    }
    public void HidePauseDialog()
    {
        if (pauseDialog != null)
        {
            pauseDialog.SetActive(false);
        }
    }
}
