using System;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalMergingList : MonoBehaviour
{
//    [SerializeField] protected RectOffset padding;
//    [SerializeField] protected Vector2 elementSize;
//    [SerializeField] protected float spacing;
//    [SerializeField] [Range(0, 1)] protected float panelPosition;
//
//    [SerializeField] [Range(1, 5)] protected float effectDistanceRate = 1.5f;
//    [SerializeField] [Range(0, 1)] protected float mergeEffectRate = 0.4f;
//    [SerializeField] [Range(0, 1)] protected float mergeAreaRate = 0.2f;
//    [SerializeField] [Range(0, 2)] protected float pushStrength = 0.5f;
//    [SerializeField] [Range(0, 2)] protected float sizeStrength = 0.5f;
//
//    [SerializeField] [Range(1, 5)] protected float removeEffectDistanceRate = 1.5f;
//
//    internal void SetSize(float size)
//    {
//        elementSize = new Vector2(size, size);
//    }
//
//    private RectTransform[] children;
//
//    internal int CurrentMergePosition { get; private set; }
//
//
//    private int[] scaleFlags;
//    private int[] moveFlags;
//
//    internal void InitMergeFlags()
//    {
//        scaleFlags = new int[transform.childCount];
//        moveFlags = new int[transform.childCount + 1];
//    }
//
//    internal void CalculateMergePosition(Vector2 pos, bool addable, bool mergable)
//    {
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            if (transform.GetChild(i).GetComponent<Animation>().isPlaying)
//                return;
//        }
//
//        float effectThreshold = (spacing / 2 + elementSize.x / 2) / 2.1f;
//
//        if (mergable)
//        {
//            for (int i = 0; i < transform.childCount; i++)
//            {
//                Vector2 location = transform.GetChild(i).position;
//
//                float distance = (location - pos).magnitude;
//
//                if (scaleFlags[i] == 1)
//                {
//                    if (distance > effectThreshold)
//                    {
//                        AN_ScaleChild(i, false);
//                        scaleFlags[i] = 0;
//                    }
//                }
//                else if (scaleFlags[i] == 0)
//                {
//                    if (distance < effectThreshold)
//                    {
//                        AN_ScaleChild(i, true);
//                        scaleFlags[i] = 1;
//                    }
//                }
//            }
//        }
//
//        Vector2 panel = (Vector2) transform.position - new Vector2(GetPanelWidth(transform.childCount) / 2, 0);
//
//
//        for (int i = 0; i < transform.childCount + 1; i++)
//        {
//            Vector2 location;
//            if (i != transform.childCount)
//                location = GetChildPosition(i) + panel - new Vector2(elementSize.x / 2 + spacing / 2, 0);
//            else
//                location = GetChildPosition(transform.childCount - 1) + panel +
//                           new Vector2(elementSize.x / 2 + spacing / 2, 0);
//            ;
//
//            float distance = (pos - location).magnitude;
//
//            if (moveFlags[i] == 1)
//            {
//                if (distance > effectThreshold)
//                {
//                    for (int j = 0; j < transform.childCount; j++)
//                        AN_BackChild(j);
//                    moveFlags[i] = 0;
//                }
//            }
//        }
//
//        for (int i = 0; i < transform.childCount + 1; i++)
//        {
//            Vector2 location;
//            if (i != transform.childCount)
//                location = GetChildPosition(i) + panel - new Vector2(elementSize.x / 2 + spacing / 2, 0);
//            else
//                location = GetChildPosition(transform.childCount - 1) + panel +
//                           new Vector2(elementSize.x / 2 + spacing / 2, 0);
//            ;
//
//            float distance = (pos - location).magnitude;
//
//            if (moveFlags[i] == 0 && addable)
//            {
//                if (distance < effectThreshold)
//                {
//                    if (i > 0)
//                        for (int j = i - 1; j >= 0; j--)
//                            AN_ShiftChild(j, false);
//
//                    if (i < transform.childCount)
//                        for (int j = i; j < transform.childCount; j++)
//                            AN_ShiftChild(j, true);
//
//                    moveFlags[i] = 1;
//                }
//            }
//        }
//
//        for (int i = 0; i < transform.childCount + 1; i++)
//        {
//            if (i != transform.childCount && scaleFlags[i] == 1)
//            {
//                CurrentMergePosition = i * 2 + 1;
//                return;
//            }
//            else if (moveFlags[i] == 1)
//            {
//                CurrentMergePosition = i * 2;
//                return;
//            }
//            else
//                CurrentMergePosition = -1;
//        }
//    }
//
//    /*
//    internal void CalculateMergePosition(Vector3 position, object levelDynamicsFlag)
//    {
//        throw new NotImplementedException();
//    }
//    */
//
//    internal int GetLength()
//    {
//        return transform.childCount;
//    }
//
//    private void AN_ScaleChild(int i, bool bigger)
//    {
//        Animation anim = transform.GetChild(i).GetComponent<Animation>();
//        AnimationCurve curve;
//
//        // create a new AnimationClip
//        AnimationClip clip = new AnimationClip()
//        {
//            legacy = true
//        };
//
//        if (bigger)
//            curve = AnimationCurve.EaseInOut(0, 1, 0.1f, 1 + sizeStrength);
//        else
//            curve = AnimationCurve.EaseInOut(0, 1 + sizeStrength, 0.1f, 1);
//
//        clip.SetCurve("", typeof(Transform), "localScale.y", curve);
//        clip.SetCurve("", typeof(Transform), "localScale.x", curve);
//
//        anim.AddClip(clip, clip.name);
//        anim.PlayQueued(clip.name);
//    }
//
//    private void AN_ShiftChild(int i, bool right)
//    {
//        Animation anim = transform.GetChild(i).GetComponent<Animation>();
//        AnimationCurve curve;
//
//        // create a new AnimationClip
//        AnimationClip clip = new AnimationClip()
//        {
//            legacy = true
//        };
//
//        if (right)
//            curve = AnimationCurve.EaseInOut(0, transform.GetChild(i).localPosition.x, 0.1f,
//                transform.GetChild(i).localPosition.x + elementSize.x / 2);
//        else
//            curve = AnimationCurve.EaseInOut(0, transform.GetChild(i).localPosition.x, 0.1f,
//                transform.GetChild(i).localPosition.x - elementSize.x / 2);
//
//        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
//
//        anim.AddClip(clip, clip.name);
//        anim.PlayQueued(clip.name);
//    }
//
//    private void AN_BackChild(int i)
//    {
//        Animation anim = transform.GetChild(i).GetComponent<Animation>();
//        AnimationCurve curve;
//
//        // create a new AnimationClip
//        AnimationClip clip = new AnimationClip()
//        {
//            legacy = true
//        };
//
//        // Vector2 panel = (Vector2)transform.position - new Vector2(GetPanelWidth(transform.childCount) / 2, 0);
//        Vector2 pos = GetChildPosition(i) - new Vector2(GetPanelWidth(transform.childCount) / 2, 0);
//        curve = AnimationCurve.EaseInOut(0, transform.GetChild(i).localPosition.x, 0.1f, pos.x);
//        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
//        curve = AnimationCurve.EaseInOut(0, transform.GetChild(i).localScale.x, 0.1f, 1);
//        clip.SetCurve("", typeof(Transform), "localScale.x", curve);
//        clip.SetCurve("", typeof(Transform), "localScale.y", curve);
//
//        anim.AddClip(clip, clip.name);
//        anim.Play(clip.name);
//    }
//
//    internal void AN_ReArrange()
//    {
//        for (int i = 0; i < transform.childCount; i++)
//            AN_BackChild(i);
//    }
//
//    internal void CalculateRemovePosition(Vector2 pos, int childID)
//    {
//        float strength = spacing + elementSize.x / 2;
//
//        float effectThreshold = elementSize.x * removeEffectDistanceRate;
//
//        Vector2 location = (Vector2) transform.position + GetChildPosition(childID - 1) -
//                           new Vector2(elementSize.x / 2 + spacing / 2, 0);
//        float distance = (location - pos).magnitude;
//
//
//        float t = Mathf.Max((effectThreshold - distance), 0) / effectThreshold;
//        t = t * strength;
//
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            RectTransform rt = transform as RectTransform;
//            Vector2 childLocation = (Vector2) rt.position - new Vector2(rt.sizeDelta.x / 2, 0) + GetChildPosition(i);
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
//    }
//
//
//    internal GameObject RemoveChild(int absPosition, Transform parent)
//    {
//        GameObject g = transform.GetChild(absPosition).gameObject;
//        g.transform.SetParent(parent);
//        return g;
//    }
//
//    internal void RebuildLayout()
//    {
//        int count = transform.childCount;
//
//        RectTransform rt = transform as RectTransform;
//        rt.anchorMin = new Vector2(0.5f, panelPosition);
//        rt.anchorMax = new Vector2(0.5f, panelPosition);
//        rt.anchoredPosition = new Vector2(0, 0);
//        rt.sizeDelta = new Vector2(GetPanelWidth(count), GetPanelHeight());
//
//        // Initilize list
//        children = new RectTransform[count];
//        for (int i = 0; i < count; i++)
//            children[i] = transform.GetChild(i) as RectTransform;
//
//        for (int i = 0; i < count; i++)
//        {
//            children[i].pivot = new Vector2(0.5f, 0.5f);
//            children[i].sizeDelta = elementSize;
//            children[i].anchorMin = new Vector2(0, 0.5f);
//            children[i].anchorMax = new Vector2(0, 0.5f);
//            children[i].anchoredPosition = GetChildPosition(i);
//        }
//    }
//
//    internal void AddChild(int childID, GameObject g)
//    {
//        // Debug.Log("[[Add " + childID + "]]");
//        g.transform.SetParent(transform);
//        g.transform.SetSiblingIndex(childID);
//    }
//
//    internal void ChangeChild(int childID, int xPosition, int code)
//    {
//        transform.GetChild(childID).GetComponent<EditorLetterHandler>().Init(xPosition, code);
//        RebuildLayout();
//    }
//
//    private Vector2 GetChildPosition(int i)
//    {
//        return new Vector2(padding.left + elementSize.x / 2 + i * (elementSize.x + spacing),
//            (padding.bottom - padding.top) / 2);
//    }
//
//    private Vector2 GetChildPosition(int i, int num)
//    {
//        float diff = (GetPanelWidth(num) - GetPanelWidth(transform.childCount)) / 2;
//
//        return new Vector2((padding.left + elementSize.x / 2 + i * (elementSize.x + spacing)) - diff,
//            (padding.bottom - padding.top) / 2);
//    }
//
//
//    private float GetPanelHeight()
//    {
//        return elementSize.y + padding.top + padding.bottom;
//    }
//
//    private float GetPanelWidth(int n)
//    {
//        return n * elementSize.x + (n - 1) * spacing + padding.right + padding.left;
//    }
//
//    internal void ReArrange()
//    {
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            transform.GetChild(i).GetComponent<EditorLetterHandler>().position = (transform.childCount - i) * 2 - 1;
//        }
//    }
//
//    private void OnTransformChildrenChanged()
//    {
//        RebuildLayout();
//    }
//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        RebuildLayout();
//    }
//#endif
//
//
//    public RectTransform[] GetChildren()
//    {
//        return children;
//    }
}