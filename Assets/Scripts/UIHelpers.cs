using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIHelpers
{
    public static void ToggleStyleClassInArray(VisualElement[] array, string className)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].AddToClassList(className);
        }
    }

    public static void ToggleStyleClassInArray(VisualElement[] array, string className, float waitTime, MonoBehaviour instance)
    {
        instance.StartCoroutine(ToggleStyleClassInArray(array, className, waitTime));
    }

    public static IEnumerator ToggleStyleClassInArray(VisualElement[] array, string className, float waitTime)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].AddToClassList(className);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
