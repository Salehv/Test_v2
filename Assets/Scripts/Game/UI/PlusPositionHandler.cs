using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlusPositionHandler : MonoBehaviour
{
    public HorizontalList reference;
    public int count;
    public GameObject plusPrefab;


    private GameObject[] currentPluses;

    public void Show(EditorHandler editor)
    {
        int num = reference.Count + 1;
        currentPluses = new GameObject[num];

        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        for (int i = 0; i < num; i++)
        {
            GameObject g = Instantiate(plusPrefab, transform);
            g.transform.localPosition = Vector3.zero;
            Vector2 pPos = reference.GetChildPosition(i, reference.Count) + new Vector2(
                               -(reference.elementSize.x / 2) - (reference.spacing / 2),
                               reference.elementSize.y / 2 - 20);

            g.GetComponent<PlusHandler>().Init(editor, i * 2, pPos);
            currentPluses[i] = g;
        }

        for (int i = 0; i < reference.Count + 1; i++)
        {
            currentPluses[i].SetActive(true);
        }

        //StartCoroutine(MexicoCity());
    }

    private IEnumerator MexicoCity()
    {
        for (int i = 0; i < reference.Count + 1; i++)
        {
            currentPluses[i].SetActive(true);
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void Resize()
    {
        RectTransform self = transform as RectTransform;
        self.sizeDelta = (reference.transform as RectTransform).sizeDelta;
        self.position = (reference.transform as RectTransform).position;
    }

    private void OnValidate()
    {
        Resize();
    }

    private void OnTransformChildrenChanged()
    {
        Resize();
    }

    public void Hide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetTrigger("hide");
        }
    }


    public void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy((transform.GetChild(i).gameObject));
        }
    }
}