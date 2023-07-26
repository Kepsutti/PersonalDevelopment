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
    private string _gameButtonText;
    private ListView _gameSelectionList;

    private ScrollView _scrollView;
    private VisualElement _scrollViewElement;
    private float _snappingDuration = 0.4f;

    private void Start()
    {
        InitializeGameList();

        VisualElement[] buttonArray = _scrollView.Children().ToArray();
        foreach (VisualElement button in buttonArray)
        {
            (button as Button).clicked += () => ListButtonClicked(button);
        }
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

    }

    private void ListButtonClicked(VisualElement button)
    {
        ScrollToElement(button);
    }

    public void ScrollToElement(VisualElement element)
    {
        ScrollToTargetValue(GetElementCenterValue(element) - 0.5f * _scrollViewElement.layout.height);
    }

    float GetElementCenterValue(VisualElement element)
    {
        return element.layout.center.y;
    }

    void ScrollToTargetValue(float target)
    {
        Sequence s = DOTween.Sequence();
        s.Append(DOVirtual.Float(_scrollView.verticalScroller.value, target, _snappingDuration, v => _scrollView.verticalScroller.value = v)).SetEase(Ease.InOutQuad);
    }
}
