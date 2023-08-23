using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static IEnumerator SceneChangeHandler(int sceneIndex, int waitTimeInSeconds)
    {
        yield return new WaitForSeconds(waitTimeInSeconds);
        SceneChangeHandler(sceneIndex);
    }

    public static void SceneChangeHandler(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
