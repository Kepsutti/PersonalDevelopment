using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSelectionController : MonoBehaviour
{
    [SerializeField]
    private VisualTreeAsset _gameButtonTemplate;

    private VisualElement _rootUI;
    private ListView _gameSelectionList;

    private ScrollView _scrollView;
    private VisualElement _scrollViewElement;
    private float _snappingDuration = 0.4f;
    private Button _upButton;
    private Button _downButton;
    private VisualElement _arrowButtonsWrapper;
    private Button _mainMenuButton;

    private bool _nowScrolling;
    private VisualElement _selectedGameButton;
    private VisualElement _blackScreen;

    private void Start()
    {
        InitializeGameList();
        StartCoroutine(InitializeScroller());
        InitializeBlackScreen();
        _nowScrolling = false;
        InitializeListButtons();
    }

    //****
    //
    //  I haven't yet decided if I should go with a ScrollView or a LisView so I
    //  have implementations for both for now.
    //
    //****

    private void InitializeGameList()
    {
        _rootUI = GetComponent<UIDocument>().rootVisualElement;

        //************ Listview*************
        List<string> GameList = new List<string> { "AA", "BB", "CC", "DDD", "E", "FF", "G", "HHH", "II" };

        _gameSelectionList = _rootUI.Q<ListView>("GameList");

        _gameSelectionList.makeItem = () =>
        {
            VisualElement newListEntry = _gameButtonTemplate.Instantiate();

            return newListEntry;
        };

        _gameSelectionList.bindItem = (item, index) =>
        {
            item.Q<Button>().text = GameList[index];
        };

        _gameSelectionList.itemsSource = GameList;

        //****************** SCROLLVIEW ******************
        _scrollView = _rootUI.Q<ScrollView>("GamesScrollView");
        _scrollViewElement = _scrollView.contentContainer.hierarchy.parent;
        _upButton = _rootUI.Q<Button>("UpButton");
        _downButton = _rootUI.Q<Button>("DownButton");
        _arrowButtonsWrapper = _upButton.parent;
        _arrowButtonsWrapper.style.translate = new Translate(0, Screen.height, 0);
        _mainMenuButton = _rootUI.Q<Button>("MainMenuButton");

        _arrowButtonsWrapper.AddToClassList("hide-element");
        _upButton.clicked += () => ArrowButtonClicked(true);
        _downButton.clicked += () => ArrowButtonClicked();
        _mainMenuButton.clicked += () => ReturnToMainMenu();
        _scrollView.RegisterCallback<WheelEvent>(e => { OnMouseWheel(e); e.StopPropagation(); }, TrickleDown.TrickleDown);
    }

    private void ArrowButtonClicked(bool scrollingUp = false)
    {
        if (_nowScrolling)
            return;

        int currentSelectedIndex = _selectedGameButton.parent.IndexOf(_selectedGameButton);
        if (scrollingUp && currentSelectedIndex > 0)
        {
            ArrowButtonClickedAnimation(_upButton);
            ScrollToElement(_scrollView.Children().ElementAt(currentSelectedIndex - 1));
        }
        else if (!scrollingUp && currentSelectedIndex < _selectedGameButton.parent.childCount -1)
        {
            ArrowButtonClickedAnimation(_downButton);
            ScrollToElement(_scrollView.Children().ElementAt(currentSelectedIndex + 1));
        }
    }

    private void OnMouseWheel(WheelEvent wheelEvent){
        if (wheelEvent.delta.y > 0)
        {
            ArrowButtonClicked();
        }
        if (wheelEvent.delta.y < 0)
        {
            ArrowButtonClicked(true);
        }
    }

    private void ArrowButtonClickedAnimation(Button button)
    {
        Vector3 buttonStartPos = button.transform.position;
        Vector3 buttonTweenPos = new Vector3(0, 20f, 0);

        if (button == _upButton)
        {
            buttonTweenPos = buttonTweenPos * -1;
        }

        DOTween.To(() => button.transform.position, x => button.transform.position = x, buttonStartPos + buttonTweenPos, 0.1f).SetLoops(2, LoopType.Yoyo);
    }

    private void ReturnToMainMenu()
    {
        StartCoroutine(BlackScreenFade(true));
    }

    private IEnumerator BlackScreenFade(bool fadeToBlack = false)
    {
        yield return 0;
        if (fadeToBlack)
        {
            _blackScreen.style.display = DisplayStyle.Flex;
            _blackScreen.style.opacity = StyleKeyword.Null;
        }
        else
        {
            _blackScreen.style.opacity = 0;
        }
    }

    private IEnumerator InitializeScroller()
    {
        yield return 0;
        _scrollView.verticalScroller.value = GetElementCenterValue(_scrollView.Children().Last()) - 0.5f * _scrollViewElement.layout.height;
        _scrollView.verticalScroller.value += _scrollViewElement.layout.height;
    }

    private void InitializeBlackScreen()
    {
        _blackScreen = _rootUI.Q<VisualElement>("BlackScreen");
        _blackScreen.RegisterCallback<TransitionEndEvent>(TransitionEnd);
        _blackScreen.style.display = DisplayStyle.Flex;
        StartCoroutine(BlackScreenFade());
    }

    private void TransitionEnd(TransitionEndEvent endEvent)
    {
        if (endEvent.target == _blackScreen && _blackScreen.style.opacity == 0)
        {
            StartCoroutine(ScrollStartAnimation());
            _blackScreen.style.display = DisplayStyle.None;
        }
        else if (endEvent.target == _blackScreen && _blackScreen.style.opacity == StyleKeyword.Null)
        {
            StartCoroutine(SceneController.SceneChangeHandler(0, 1));
        }
    }

    private void InitializeListButtons()
    {
        List<VisualElement> buttonList = _scrollView.Children().ToList();
        UIHelpers.InitializeButtonTextSize(buttonList, Screen.height / 12);

        foreach (VisualElement button in buttonList)
        {
            (button as Button).clicked += () => ListButtonClicked(button);
        }
    }

    private IEnumerator ScrollStartAnimation()
    {
        SetNowScrolling(true);

        yield return new WaitForSeconds(1);
        ScrollToElement(_scrollView.Children().First(), true);

        yield return new WaitUntil(() => !_nowScrolling);
        _arrowButtonsWrapper.ToggleInClassList("hide-element");
        _arrowButtonsWrapper.style.translate = StyleKeyword.Null;
    }

    private void ListButtonClicked(VisualElement button)
    {
        if (_nowScrolling)
            return;

        if (_selectedGameButton != button)
        {
            ScrollToElement(button);
        }
        else
        {
            Debug.Log("SELECTED");
        }
    }

    private void ScrollToElement(VisualElement element, bool isEntryScroll = false)
    {
        SetSelectedGameButton(element);
        ScrollToTargetValue(GetElementCenterValue(element) - 0.5f * _scrollViewElement.layout.height, isEntryScroll);
    }

    private void SetSelectedGameButton(VisualElement button)
    {
        _selectedGameButton = button;
    }

    private float GetElementCenterValue(VisualElement element)
    {
        return element.layout.center.y;
    }

    private void ScrollToTargetValue(float target, bool isEntryScroll)
    {
        float snappingDuration = _snappingDuration;
        if (isEntryScroll)
        {
            snappingDuration = 2f;
        }

        SetNowScrolling(true);
        Sequence s = DOTween.Sequence();
        s.Append(DOVirtual.Float(_scrollView.verticalScroller.value, target, snappingDuration, v => _scrollView.verticalScroller.value = v))
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => SetNowScrolling(false));
    }

    private void SetNowScrolling(bool value)
    {
        _nowScrolling = value;
    }
}
