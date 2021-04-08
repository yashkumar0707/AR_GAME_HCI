﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : Singleton<SceneLoader>
{
    private string sceneNameToBeLoaded;
    public void LoadScene(string _sceneName)
    {
        sceneNameToBeLoaded = _sceneName;
        StartCoroutine(InitializeSceneLoading());
    }
    IEnumerator InitializeSceneLoading()
    {
        yield return SceneManager.LoadSceneAsync("Scene_Loading");

        StartCoroutine(LoadActualyScene());
        
    }

    IEnumerator LoadActualyScene()
    {
        var asyncSceneLoading = SceneManager.LoadSceneAsync(sceneNameToBeLoaded);
        asyncSceneLoading.allowSceneActivation = false;
        while(!asyncSceneLoading.isDone)
        {
            Debug.Log(asyncSceneLoading.progress);
            if(asyncSceneLoading.progress>=0.9f)
            {
                asyncSceneLoading.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
