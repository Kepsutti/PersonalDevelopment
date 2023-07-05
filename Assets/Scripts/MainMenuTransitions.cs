using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuTransitions : MonoBehaviour
{
    public MainMenuController mainMenuController;

    private VisualElement _finalTransitionElement;
    private bool _revealBackdropAnimationOnce = false;

    public VisualElement mainMenuWrapper;

    private string buttonEntryAnimationStyle = "button-entry-animation";
    private string menuButtonStyle = "menu-button";
    private string menuButtonTransitionStyle = "menu-button-transition";

    public void BackdropEntryTransition(VisualElement backdrop, VisualElement backdropEffect)
    {
        backdrop.ToggleInClassList(mainMenuController.hideElementStyle);
        backdrop.style.translate = new Translate(0, 0, 0);
        backdrop.style.scale = new Scale(new Vector2(1, 1));

        backdropEffect.ToggleInClassList(mainMenuController.hideElementStyle);
        backdropEffect.style.translate = new Translate(0, 0, 0);
        backdropEffect.style.scale = new Scale(new Vector2(0, 1));

        mainMenuController.title.ToggleInClassList(mainMenuController.hideElementStyle);
        mainMenuController.title.style.translate = new Translate(0, 0, 0);
    }

    public void TransitionEndHandler(TransitionEndEvent endEvent)
    {
        if (endEvent.target == mainMenuController.backdrop && !_revealBackdropAnimationOnce)
        {
            _revealBackdropAnimationOnce = true;
            StartCoroutine(ButtonsEntryAnimation());
            //AnimateMenuButtons2();
        }

        if (endEvent.target is Button)
        {
            Button targ = (Button)endEvent.target;
            if (targ.ClassListContains(buttonEntryAnimationStyle))
            {
                targ.RemoveFromClassList(buttonEntryAnimationStyle);

                if (targ == _finalTransitionElement)
                {
                    targ.parent.BringToFront();
                    _finalTransitionElement = null;
                }
                return;
            }

            if (_finalTransitionElement == null)
            {
                return;
            }
            if (targ.parent == _finalTransitionElement.parent)
            {
                targ.ToggleInClassList(menuButtonTransitionStyle);
                targ.ToggleInClassList(menuButtonStyle);

                if (targ == _finalTransitionElement)
                {
                    targ.parent.BringToFront();
                    _finalTransitionElement = null;
                    mainMenuController.ToggleButtonIgnore(false);
                }
            }
        }
    }

    public void AnimateMenuButtons2()
    {
        VisualElement[] _buttonsArray = mainMenuWrapper.Children().ToArray();
        _finalTransitionElement = _buttonsArray[_buttonsArray.Length - 1];

        UIHelpers.ToggleStyleClassInArray(_buttonsArray, mainMenuController.hideElementStyle, 0.1f, this);
    }

    public IEnumerator ButtonsEntryAnimation()
    {
        VisualElement[] _buttonsArray = mainMenuWrapper.Children().ToArray();
        _finalTransitionElement = _buttonsArray[_buttonsArray.Length - 1];

        for (int i = 0; i < _buttonsArray.Length; i++)
        {
            _buttonsArray[i].ToggleInClassList(mainMenuController.hideElementStyle);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator ButtonsMenuTransition(VisualElement buttonWrapper, bool revealButtons = false)
    {
        VisualElement[] _buttonsArray = buttonWrapper.Children().ToArray();

        int newTranslateValue = Screen.height;
        int firstAnimatableButtonIndex = _buttonsArray.Length - 1;

        if (revealButtons)
        {
            newTranslateValue = 0;
            firstAnimatableButtonIndex = 0;
        }
        else
        {
            mainMenuController.ToggleButtonIgnore(true);

            for (int i = 0; i < _buttonsArray.Length; i++)
            {
                _buttonsArray[i].ToggleInClassList(menuButtonTransitionStyle);
                _buttonsArray[i].ToggleInClassList(menuButtonStyle);
            }
        }
        //First transitioning button's transition breaks without yield
        yield return 0;

        if (revealButtons)
        {
            _finalTransitionElement = _buttonsArray[_buttonsArray.Length - 1];

            for (int i = firstAnimatableButtonIndex; i < _buttonsArray.Length; i++)
            {
                _buttonsArray[i].style.translate = new Translate(0, newTranslateValue, 0);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            for (int i = firstAnimatableButtonIndex; i > -1; i--)
            {
                _buttonsArray[i].style.translate = new Translate(0, newTranslateValue, 0);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
