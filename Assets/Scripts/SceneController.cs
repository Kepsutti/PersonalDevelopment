using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static IEnumerator SceneChangeByID(int sceneIndex, int waitTimeInSeconds)
    {
        yield return new WaitForSeconds(waitTimeInSeconds);
        SceneChangeByID(sceneIndex);
    }

    public static void SceneChangeByID(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public static void SceneChangeByString(string sceneName)
    {
        if (sceneName == null)
            return;

        if (EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path.Contains("/" + sceneName + ".unity")))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("Scene name " + sceneName + " not found");
        }
    }
}
