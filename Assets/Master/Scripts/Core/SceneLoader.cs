using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
public class SceneLoader
{

    private const float MinLoadingTime = 1.5f;

    [Inject] private readonly LoadingBar m_LoadingBar;

    public async UniTask LoadSceneWithLoadingBar(string sceneName)
    {
        m_LoadingBar.Show();
        m_LoadingBar.SetProgress(0f);

        float startTime = Time.time;

        await LoadSceneAsync(sceneName, startTime);

        m_LoadingBar.Hide();
    }
    public async UniTask LoadSceneWithoutLoadingBar(string sceneName)
    {
        if (m_LoadingBar != null)
        {
            m_LoadingBar.ForceHide();
        }

        await SceneManager.LoadSceneAsync(sceneName);
    }
    private async UniTask LoadSceneAsync(string sceneName, float startTime)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        // Wait until the scene is 90% loaded
        while (operation.progress < 0.9f || (Time.time - startTime) < MinLoadingTime)
        {
            // Calculate progress based on load status and elapsed time
            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01((Time.time - startTime) / MinLoadingTime);

            // Sync the loading bar
            m_LoadingBar.SetProgress(Mathf.Min(loadProgress, timeProgress));

            await UniTask.Yield();
        }

        // Ensure the bar reaches 100%
        m_LoadingBar.SetProgress(1f);
        await UniTask.Delay(300);

        operation.allowSceneActivation = true;
    }
}
