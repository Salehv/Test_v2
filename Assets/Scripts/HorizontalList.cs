using System;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalList : MonoBehaviour
{

    [SerializeField] protected RectOffset padding;
    public Vector2 elementSize;
    public float spacing;
    [SerializeField] [Range(0, 1)] protected float panelPosition;

    [SerializeField] [Range(0, 2)] protected float sizeStrength = 0.5f;

    [SerializeField] [Range(1, 5)] protected float removeEffectDistanceRate = 1.5f;

    internal void SetSize(float size)
    {
        elementSize = new Vector2(size, size);
    }

    private RectTransform[] children;

    internal int Count
    {
        get { return transform.childCount; }
    }



    private void AN_BackChild(int childID)
    {
        Animation anim = transform.GetChild(childID).GetComponent<Animation>();
        AnimationCurve curve;

        // create a new AnimationClip
        AnimationClip clip = new AnimationClip()
        {
            legacy = true
        };

        // Vector2 panel = (Vector2)transform.position - new Vector2(GetPanelWidth(transform.childCount) / 2, 0);
        Vector2 pos = GetChildPosition(childID) - new Vector2(GetPanelWidth(transform.childCount) / 2, 0);
        curve = AnimationCurve.EaseInOut(0, transform.GetChild(childID).localPosition.x, 0.1f, pos.x);
        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
        curve = AnimationCurve.EaseInOut(0, transform.GetChild(childID).localScale.x, 0.1f, 1);
        clip.SetCurve("", typeof(Transform), "localScale.x", curve);
        clip.SetCurve("", typeof(Transform), "localScale.y", curve);

        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);
    }

    private void AN_TileGoTo(Vector2 dest, int toChildID, int fromChildID)
    {
        Animation anim = transform.GetChild(fromChildID).GetComponent<Animation>();
        AnimationCurve curve;

        AnimationClip clip = new AnimationClip()
        {
            legacy = true
        };

        curve = AnimationCurve.EaseInOut(0, transform.GetChild(fromChildID).localPosition.x, 0.1f, dest.x);
        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
        
        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);
    }



    internal void RemoveChild(int childIndex)
    {
        Destroy(transform.GetChild(childIndex).gameObject);
    }

    internal void RebuildLayout()
    {
        int count = transform.childCount;

        RectTransform rt = transform as RectTransform;
        rt.anchorMin = new Vector2(0.5f, panelPosition);
        rt.anchorMax = new Vector2(0.5f, panelPosition);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(GetPanelWidth(count), GetPanelHeight());

        // Initilize list
        children = new RectTransform[count];
        for (int i = 0; i < count; i++)
            children[i] = transform.GetChild(i) as RectTransform;

        for(int i = 0; i < count; i++)
        {
            children[i].pivot = new Vector2(0.5f, 0.5f);
            children[i].sizeDelta = elementSize;
            children[i].anchorMin = new Vector2(0, 0.5f);
            children[i].anchorMax = new Vector2(0, 0.5f);
            children[i].anchoredPosition = GetChildPosition(i);
        }
    }

    internal void AddChild(int childIndex, GameObject g)
    {
        g.transform.SetParent(transform);
        g.transform.SetSiblingIndex(childIndex);
    }

    internal void ChangeChild(int childIndex, int xPosition, int code)
    {
        transform.GetChild(childIndex).GetComponent<EditorLetterHandler>().Init(xPosition, code);
        RebuildLayout();
    }

    private Vector2 GetChildPosition(int i)
    {
        return new Vector2(padding.left + elementSize.x / 2 + i * (elementSize.x + spacing), (padding.bottom - padding.top) / 2);
    }

    internal Vector2 GetChildPosition(int i, int num)
    {
        float diff = (GetPanelWidth(num) - GetPanelWidth(transform.childCount)) / 2;

        return new Vector2((padding.left + elementSize.x / 2 + i * (elementSize.x + spacing)) - diff, (padding.bottom - padding.top) / 2);
    }


    private float GetPanelHeight()
    {
        return elementSize.y + padding.top + padding.bottom;
    }

    private float GetPanelWidth(int n)
    {
        return n * elementSize.x + (n - 1) * spacing + padding.right + padding.left;
    }

    private void OnTransformChildrenChanged()
    {
        RebuildLayout();
    }

    private void OnValidate()
    {
        RebuildLayout();    
    }

    
    public RectTransform[] GetChildren()
    {
        return children;
    }
    
    
//    internal void CalculateRemovePosition(Vector2 pos, int childID)
//    {
//        float strength = spacing + elementSize.x / 2;
//
//        float effectThreshold = elementSize.x * removeEffectDistanceRate;
//
//        Vector2 location = (Vector2)transform.position + GetChildPosition(childID - 1) - new Vector2(elementSize.x / 2 + spacing / 2, 0);
//        float distance = (location - pos).magnitude;
//
//
//        float t = Mathf.Max((effectThreshold - distance), 0) / effectThreshold;
//        t = t * strength;
//
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            RectTransform rt = transform as RectTransform;
//            Vector2 childLocation = (Vector2)rt.position - new Vector2(rt.sizeDelta.x / 2, 0) + GetChildPosition(i);
//
//            if (i < childID)
//            {
//                children[i].anchoredPosition = GetChildPosition(i) + new Vector2(-t, 0);
//            }
//            else if (i >= childID)
//            {
//                children[i].anchoredPosition = GetChildPosition(i) + new Vector2(t, 0);
//            }
//        }
//
//    }

    private Vector2 GetAbsoluteChildPosition(int i, int count)
    {
        return GetChildPosition(i, transform.childCount + 1) - new Vector2(GetPanelWidth(count) / 2, 0);
    }

    public void AnimateAdd(int indexToAdd)
    {
        Debug.Log("Index To Add: " + indexToAdd);
        
        for (int i = 0; i < indexToAdd; i++)
        {
            Debug.Log(i + " TO " + i);
            AN_TileGoTo(GetAbsoluteChildPosition(i, transform.childCount + 1), i, i);        
        }

        for (int i = indexToAdd + 1; i < transform.childCount + 1; i++)
        {
            Debug.Log((i - 1) + " TO " + i);
            AN_TileGoTo(GetAbsoluteChildPosition(i - 1, transform.childCount + 1), i, i-1);
        }
    }

    public void ReArrange()
    {
        RebuildLayout();
    }
}