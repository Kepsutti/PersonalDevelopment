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
            array[i].ToggleInClassList(className);
        }
    }

    public static IEnumerator ToggleStyleClassInArray(VisualElement[] array, string className, float waitTime)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].ToggleInClassList(className);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
