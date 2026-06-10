using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
public class SceneLoader
{
    [Inject] private readonly LoadingBar m_LoadingBar;

    public async void LoadTargetScene(string sceneName)
    {
        await LoadSceneAsync(sceneName);

    }
    private async UniTask LoadSceneAsync(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
    }
}
