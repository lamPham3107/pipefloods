using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RateDialog : MonoBehaviour
{
    public GameObject rateDialog;

    public static RateDialog instance;
    public System.Action onCloseRateDialog; 
    public Sprite Star_On_img;
    public Sprite Star_Off_img;
    private int selectedRating = 0;
    public Button[] stars;
    public Button submitButton;

    void Awake()
    {
        instance = this;
        for (int i = 0; i < stars.Length; i++)
        {
            int index = i; 
            stars[i].onClick.AddListener(() => OnStarClicked(index + 1));
        }
    }
    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.interactable = false;
        }
    }
    public void ShowRateDialog()
    {

        if (rateDialog != null && GameManager.isShowingRateDialog)
        {
            GameManager.instance.isRunning = false;
            Timer_origin.instance.Pause();

            ResetStars();
            rateDialog.SetActive(true);
        }
    }
    public void HideRateDialog()
    {

        rateDialog.SetActive(false);
        GameManager.isShowingRateDialog = false;
        Timer_origin.instance.Resume();
        GameManager.instance.isRunning = true;

        if (onCloseRateDialog != null)
            onCloseRateDialog.Invoke();

    }
    public void Submit()
    {

        // Handle the submit button click event here
        Debug.Log("Submit button clicked!");

        HideRateDialog();

    }

    void OnStarClicked(int rating)
    {
        selectedRating = rating;
        submitButton.interactable = true;
        UpdateStarUI(rating);
        Debug.Log("Selected Rating: " + rating);
    }

    void UpdateStarUI(int rating)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            Image starImage = stars[i].GetComponent<Image>();
            if (i < rating)
            {
                starImage.sprite = Star_On_img;
            }
            else
            {
                starImage.sprite = Star_Off_img;
            }
        }
    }
    void ResetStars()
    {
        selectedRating = 0;
        UpdateStarUI(0);
    }
}
