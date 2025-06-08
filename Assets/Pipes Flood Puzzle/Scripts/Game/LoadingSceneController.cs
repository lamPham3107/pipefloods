using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public string sceneToLoad = "Main";
    public Slider progressBar;
    public TMP_Text progressText;
    private float loadingSpeed = 0.2f;
    private const string LoginCountKey = "login_count";

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    private void Update()
    {
        if(loadingSpeed < 0.4f && AdmobManager.ins != null && AdmobManager.ins.isAOAShowing)
        {
            loadingSpeed = 0.4f;
        }
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;

        // Dừng lần lượt tại  90 (fake progress)
        while (fakeProgress < 0.9f)
        {
            fakeProgress += Time.deltaTime * loadingSpeed; // tốc độ tăng có thể điều chỉnh
            UpdateUI(fakeProgress);
            yield return null;
        }

        UpdateUI(0.9f);
        yield return new WaitForSeconds(1f);

        // Đợi scene thật sự load xong (progress >= 0.9)
        while (operation.progress < 0.9f)
            yield return null;

        UpdateUI(1f); // Show 100/100
        yield return new WaitForSeconds(0.5f); // Optional

        AdsManager.ins.ShowAOA();
        CheckLogin();
        uiFade.ins.FadeInOut(() =>
        {
            operation.allowSceneActivation = true;
            AdsManager.ins.ShowBanner();
            
        }, 0.75f);
    }

    void UpdateUI(float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (progressBar != null)
            progressBar.value = progress;

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(progress * 100f) + "/100";
    }
    public void CheckLogin()
    {
        // lay so lan dang nhap , mac dinh la 0 neu chua dang nhap
        int count = PlayerPrefs.GetInt(LoginCountKey, 0);
        int maxSavedMission = PlayerPrefs.GetInt("HighestMissionID", -1); // -1 là giá trị mặc định nếu chưa có
        int maxSavedLevel = PlayerPrefs.GetInt("HighestLevelID", -1);
        count++;
        PlayerPrefs.SetInt(LoginCountKey, count);
        PlayerPrefs.Save();
        FirebaseEvent.ins.E_openGame(count,maxSavedMission,maxSavedLevel);
        Debug.Log("Highest Mission ID: " + maxSavedMission);
        Debug.Log("Highest Level ID: " + maxSavedLevel);
        Debug.Log("Login Count: " + count);
    }

}
