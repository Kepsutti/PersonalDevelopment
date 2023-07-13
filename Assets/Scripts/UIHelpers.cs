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

    public static void ChangeTranslatePropertyInArray(VisualElement[] array, Translate newTranslate)
    {
        Translate zero = new Translate(0, 0, 0);
        if (newTranslate == zero)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].style.translate = StyleKeyword.Null;
            }
        }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].style.translate = newTranslate;
            }
        }
    }

    public static IEnumerator ChangeTranslatePropertyInArray(VisualElement[] array, Translate newTranslate, float waitTime)
    {
        Translate zero = new Translate(0, 0, 0);
        if (newTranslate == zero)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].style.translate = StyleKeyword.Null;
                yield return new WaitForSeconds(waitTime);
            }
        }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].style.translate = newTranslate;
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
