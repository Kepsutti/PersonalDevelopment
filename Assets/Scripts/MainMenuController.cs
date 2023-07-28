using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject _startButtonParticles;

    private MainMenuTransitions _transitionController;
    private UIDocument _uiDocument;

    private Button _startButton;
    private Button _settingsButton;
    private Button _exitButton;
    private Button _backButton;

    private VisualElement _mainMenuWrapper;
    private VisualElement _settingsWrapper;
    private VisualElement _menuContent;
    public VisualElement backdrop;
    private VisualElement _backdropEffect;
    public VisualElement title;
    private VisualElement _blackScreen;

    private bool _ignoreButtonClicks;

    public string hideElementStyle = "hide-element";

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        backdrop = _uiDocument.rootVisualElement.Q<VisualElement>("Backdrop");
        _backdropEffect = _uiDocument.rootVisualElement.Q<VisualElement>("BackdropEffect");
        _blackScreen = _uiDocument.rootVisualElement.Q<VisualElement>("BlackScreen");
        _menuContent = _uiDocument.rootVisualElement.Q<VisualElement>("MenuContent");
        title = _uiDocument.rootVisualElement.Q<VisualElement>("Title");
        _startButton = _menuContent.Q<Button>("StartButton");
        _settingsButton = _menuContent.Q<Button>("SettingsButton");
        _exitButton = _menuContent.Q<Button>("ExitButton");
        _backButton = _menuContent.Q<Button>("BackButton");
        _mainMenuWrapper = _menuContent.Q<VisualElement>("Buttons");
        _settingsWrapper = _menuContent.Q<VisualElement>("Settings");

        _startButton.clicked += OnClickedStartButton;
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
        InitializeButtonText();
        _ignoreButtonClicks = false;
    }

    private void InitializeButtons()
    {
        VisualElement[] _buttonsArray = _mainMenuWrapper.Children().ToArray();
        UIHelpers.ToggleStyleClassInArray(_buttonsArray, hideElementStyle);

        VisualElement[] _settingsArray = _settingsWrapper.Children().ToArray();
        UIHelpers.ChangeTranslatePropertyInArray(_settingsArray, new Translate(0, Screen.height, 0));

        _mainMenuWrapper.style.display = DisplayStyle.Flex;
        _settingsWrapper.style.display = DisplayStyle.Flex;
        _settingsWrapper.BringToFront();
    }

    private void InitializeBackdropAndTitle()
    {
        _blackScreen.style.display = DisplayStyle.Flex;

        backdrop.AddToClassList(hideElementStyle);
        _backdropEffect.AddToClassList(hideElementStyle);
        title.AddToClassList(hideElementStyle);
        _blackScreen.AddToClassList(hideElementStyle);

        backdrop.style.scale = new Scale(new Vector2(0, 1));
        backdrop.style.translate = new Translate(-Screen.width, 0, 0);
        _backdropEffect.style.scale = new Scale(Vector2.one);
        _backdropEffect.style.translate = new Translate(-Screen.width, 0, 0);
        title.style.translate = new Translate(0, -25, 0);
    }

    private void InitializeButtonText()
    {
        //UI Toolkit does not support text size as percentage at the time of writing this, hence this function
        List<VisualElement> buttonList = _uiDocument.rootVisualElement.Query(className: "menu-button").ToList();
        UIHelpers.InitializeButtonTextSize(buttonList, Screen.height / 23);
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

    private void OnClickedStartButton()
    {
        if (_ignoreButtonClicks)
            return;

        ToggleButtonIgnore(true);
        _transitionController.StartButtonClicked(_startButton, _blackScreen);
        //SceneManager.LoadScene("Game");
    }

    private void OnClickedSettingsButton()
    {
        if (_ignoreButtonClicks)
            return;

        ToggleButtonIgnore(true);
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
