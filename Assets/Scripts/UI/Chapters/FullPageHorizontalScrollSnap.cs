using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class FullPageHorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected RectTransform content;
    [SerializeField] protected float transitionSpeed;
    [SerializeField] protected float speedToChangePage;
    private List<RectTransform> _children;

    private float screenWidth;

    // Movement Control
    private bool _pointerDown;
    private bool _settled;
    private bool _lerping;
    private Vector2 _lerpTarget;
    private float _speed;
    private int _startedPage;


    /**
    private void Start()
    {
        Init();
    }
    /**/


    internal void Init()
    {
        if (GetComponentInParent<CanvasScaler>().uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
            screenWidth = Screen.width;
        else
            screenWidth = GetComponentInParent<CanvasScaler>().referenceResolution.x;

        _children = new List<RectTransform>();
        for (int i = 0; i < content.childCount; i++)
        {
            _children.Add(content.GetChild(i).transform as RectTransform);
        }

        SetContentLayout();

        SetContentSize();


        // Movement
        _settled = true;
        _pointerDown = false;
        _speed = 0;
    }


    private void SetContentSize()
    {
        int n = content.childCount;
        content.anchorMax = new Vector2(0, 1);
        content.anchorMin = new Vector2(0, 0);
        content.pivot = new Vector2(0, 0.5f);
        content.anchoredPosition = Vector2.zero;

        content.sizeDelta = new Vector2(n * screenWidth, 0);

        foreach (RectTransform child in _children)
        {
            child.sizeDelta = new Vector2(screenWidth, 0);
        }
    }


    private void SetContentLayout()
    {
        if (content.GetComponent<HorizontalLayoutGroup>() == null)
            content.gameObject.AddComponent<HorizontalLayoutGroup>();

        var layout = content.GetComponent<HorizontalLayoutGroup>();

        layout.childControlHeight = true;
        layout.childForceExpandHeight = true;
        layout.childAlignment = TextAnchor.MiddleLeft;
    }


    private void Update()
    {
        if (!_pointerDown)
        {
            if (!_settled && !_lerping)
            {
                _lerping = true;

                if (_speed > speedToChangePage || _speed < -speedToChangePage)
                {
                    _lerpTarget = _speed < 0 ? GetPagePosition(_startedPage + 1) : GetPagePosition(_startedPage - 1);
                }
                else
                    _lerpTarget = GetPagePosition(GetCurrentPage());

                return;
            }
        }

        if (_lerping)
        {
            content.anchoredPosition =
                Vector2.Lerp(content.anchoredPosition, _lerpTarget, transitionSpeed * Time.deltaTime);

            if ((content.anchoredPosition - _lerpTarget).magnitude < 0.1f)
            {
                content.anchoredPosition = _lerpTarget;
                _settled = true;
                _lerping = false;
            }
        }
    }


    internal void GoToPageDelayed(int page, float delay)
    {
        _lerpTarget = GetPagePosition(page);
        Invoke("_go", delay);
    }


    internal void GoToPage(int page)
    {
        _lerpTarget = GetPagePosition(page);
        _go();
    }


    public void SetPage(int lastChapter)
    {
        content.anchoredPosition = GetPagePosition(lastChapter);
    }


    private void _go()
    {
        _lerping = true;
    }


    #region Helpers

    private int GetCurrentPage()
    {
        float x = content.anchoredPosition.x;
        float width = screenWidth;

        var page = (int) (-(x - (width / 2)) / width);
        return page;
    }

    private Vector2 GetPagePosition(int page)
    {
        float width = screenWidth;
        if (page < 0)
            return Vector2.zero;
        if (page > content.childCount - 1)
            return new Vector2((-content.childCount + 1) * width, 0);
        return new Vector2(-page * width, 0);
    }

    #endregion


    #region interfaces

    public void OnBeginDrag(PointerEventData eventData)
    {
        _pointerDown = true;
        _startedPage = GetCurrentPage();
    }


    public void OnDrag(PointerEventData eventData)
    {
        _lerping = false;
        content.anchoredPosition += new Vector2(eventData.delta.x, 0);

        float min = -content.sizeDelta.x + screenWidth;
        if (content.anchoredPosition.x < min)
            content.anchoredPosition = new Vector2(min, 0);

        if (content.anchoredPosition.x > 0)
            content.anchoredPosition = Vector2.zero;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        GetCurrentPage();
        _pointerDown = false;
        _speed = eventData.delta.x;
        _settled = false;
    }

    #endregion
}