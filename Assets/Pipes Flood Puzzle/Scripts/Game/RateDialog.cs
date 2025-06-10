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
    private const string RateDialogShownKey = "RateDialogShown";
    private const string RateUsButtonDisabledKey = "RateUsButtonDisabled";

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
        //// Nếu đã từng hiện thì không hiện lại
        //if (PlayerPrefs.GetInt(RateDialogShownKey, 0) == 1)
        //{
        //    return;
        //}

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
        // Đánh dấu là đã show
        PlayerPrefs.SetInt("RateDialogShown", 1);
        PlayerPrefs.Save();
        rateDialog.SetActive(false);
        GameManager.isShowingRateDialog = false;
        GameManager.instance.isRunning = true;
        Timer_origin.instance.Run();

        if (onCloseRateDialog != null)
            onCloseRateDialog.Invoke();

    }
    public void Submit()
    {

        // Nếu >= 4 sao thì mở link đánh giá
        if (selectedRating >= 4)
        {
            UIEvents.instance.OpenLink();
        }
        PlayerPrefs.SetInt("RateUsButtonDisabled", 1);
        PlayerPrefs.Save();
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
