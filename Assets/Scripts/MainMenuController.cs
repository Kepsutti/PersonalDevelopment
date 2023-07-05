using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private MainMenuTransitions _transitionController;
    private UIDocument _uiDocument;

    private Button _playButton;
    private Button _settingsButton;
    private Button _exitButton;
    private Button _backButton;

    private VisualElement _mainMenuWrapper;
    private VisualElement _settingsWrapper;
    private VisualElement _menuContent;
    public VisualElement backdrop;
    private VisualElement _backdropEffect;
    public VisualElement title;

    private bool _ignoreButtonClicks;

    public string hideElementStyle = "hide-element";

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        backdrop = _uiDocument.rootVisualElement.Q<VisualElement>("Backdrop");
        _backdropEffect = _uiDocument.rootVisualElement.Q<VisualElement>("BackdropEffect");
        _menuContent = _uiDocument.rootVisualElement.Q<VisualElement>("MenuContent");
        title = _uiDocument.rootVisualElement.Q<VisualElement>("Title");
        _playButton = _menuContent.Q<Button>("PlayButton");
        _settingsButton = _menuContent.Q<Button>("SettingsButton");
        _exitButton = _menuContent.Q<Button>("ExitButton");
        _backButton = _menuContent.Q<Button>("BackButton");
        _mainMenuWrapper = _menuContent.Q<VisualElement>("Buttons");
        _settingsWrapper = _menuContent.Q<VisualElement>("Settings");

        _playButton.clicked += OnClickedPlayButton;
        _settingsButton.clicked += OnClickedSettingsButton;
        _exitButton.clicked += OnClickedExitButton;
        _backButton.clicked += OnClickedBackButton;

        backdrop.RegisterCallback<TransitionEndEvent>(TransitionEnd);
        _menuContent.RegisterCallback<TransitionEndEvent>(TransitionEnd);

        _transitionController = GetComponent<MainMenuTransitions>();
        _transitionController.mainMenuController = this;
        _transitionController.mainMenuWrapper = _mainMenuWrapper;

        Initialize();
    }

    private void Initialize()
    {
        InitializeButtons();
        InitializeBackdropAndTitle();
        _ignoreButtonClicks = false;
    }

    private void InitializeButtons()
    {
        VisualElement[] _buttonsArray = _mainMenuWrapper.Children().ToArray();
        UIHelpers.AddStyleClassToArray(_buttonsArray, hideElementStyle);

        VisualElement[] _settingsArray = _settingsWrapper.Children().ToArray();
        for (int i = 0; i < _settingsArray.Length; i++)
        {
            _settingsArray[i].style.translate = new Translate(0, Screen.height, 0);
        }

        _mainMenuWrapper.style.display = DisplayStyle.Flex;
        _settingsWrapper.style.display = DisplayStyle.Flex;
        _settingsWrapper.BringToFront();
    }

    private void InitializeBackdropAndTitle()
    {
        backdrop.AddToClassList(hideElementStyle);
        _backdropEffect.AddToClassList(hideElementStyle);
        title.AddToClassList(hideElementStyle);

        backdrop.style.scale = new Scale(new Vector2(0, 1));
        backdrop.style.translate = new Translate(-Screen.width, 0, 0);
        _backdropEffect.style.scale = new Scale(Vector2.one);
        _backdropEffect.style.translate = new Translate(-Screen.width, 0, 0);
        title.style.translate = new Translate(0, -25, 0);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        _transitionController.BackdropEntryTransition(backdrop, _backdropEffect);
    }

    private void TransitionEnd(TransitionEndEvent endEvent)
    {
        _transitionController.TransitionEndHandler(endEvent);
    }

    private void OnClickedPlayButton()
    {
        if (_ignoreButtonClicks)
            return;

        Debug.Log("Clicked play");
        //SceneManager.LoadScene("Game");
    }

    private void OnClickedSettingsButton()
    {
        if (_ignoreButtonClicks)
            return;

        StartCoroutine(SettingsButtonCoroutineListener());
    }

    private IEnumerator SettingsButtonCoroutineListener()
    {
        yield return StartCoroutine(_transitionController.ButtonsMenuTransition(_mainMenuWrapper));
        StartCoroutine(_transitionController.ButtonsMenuTransition(_settingsWrapper, true));
    }

    private void OnClickedExitButton()
    {
        if (_ignoreButtonClicks)
            return;

        Application.Quit();
    }

    private void OnClickedBackButton()
    {
        if (_ignoreButtonClicks)
            return;

        StartCoroutine(BackButtonCoroutineListener());
    }

    private IEnumerator BackButtonCoroutineListener()
    {
        yield return StartCoroutine(_transitionController.ButtonsMenuTransition(_settingsWrapper));
        StartCoroutine(_transitionController.ButtonsMenuTransition(_mainMenuWrapper, true));
    }

    public void ToggleButtonIgnore(bool toggle)
    {
        _ignoreButtonClicks = toggle;
    }
}
