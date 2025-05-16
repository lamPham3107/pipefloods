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
    public Fade fadeController;
    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;

        // Dừng lần lượt tại 20, 50, 90 (fake progress)
        while (fakeProgress < 0.2f)
        {
            fakeProgress += Time.deltaTime * 0.2f; // tốc độ tăng có thể điều chỉnh
            UpdateUI(fakeProgress);
            yield return null;
        }
        UpdateUI(0.2f);
        yield return new WaitForSeconds(1f);

        while (fakeProgress < 0.5f)
        {
            fakeProgress += Time.deltaTime * 0.2f;
            UpdateUI(fakeProgress);
            yield return null;
        }
        UpdateUI(0.5f);
        yield return new WaitForSeconds(2f);

        while (fakeProgress < 0.9f)
        {
            fakeProgress += Time.deltaTime * 0.2f;
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
        yield return new WaitUntil(() => !AdmobManager.ins.isAOAShowing);
        // Sau khi AOA đóng → bắt đầu fade-in
        if(Fade.Instance != null)
        {
            yield return StartCoroutine(Fade.Instance.FadeIn());
        }
        operation.allowSceneActivation = true;


    }

    void UpdateUI(float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (progressBar != null)
            progressBar.value = progress;

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(progress * 100f) + "/100";
    }


}
