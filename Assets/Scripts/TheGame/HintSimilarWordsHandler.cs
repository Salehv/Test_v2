using UnityEngine;
using UnityEngine.UI;

namespace TheGame
{
    public class HintSimilarWordsHandler : MonoBehaviour {
        [SerializeField] private GameObject similarWordPrefab;
        [SerializeField] private Transform swParent;
        [SerializeField] private ToggleGroup tg;

        internal void Clear()
        {
            for (int i = 0; i < swParent.childCount; i++)
                Destroy(swParent.GetChild(i).gameObject);

        }


        private string[] shownSimilar;
        internal void SetSimilarWords(string[] sWords)
        {
            shownSimilar = new string[sWords.Length];
            
            int i = 0;
            foreach (var word in sWords)
            {
                GameObject g = Instantiate(similarWordPrefab, swParent);
                g.GetComponentInChildren<Toggle>().group = tg;
                g.GetComponentInChildren<Text>().text = word;
                
                shownSimilar[i] = Utilities.GetNormalizedFarsi(word);

                g.name = $"{i++:00}";
            }
        }

        internal string GetSelected()
        {
            var ts = tg.GetComponentsInChildren<Toggle>();

            foreach (var t in ts)
            {
                if (t.isOn)
                {
                    int selected = int.Parse(t.transform.parent.name);
                    return shownSimilar[selected];
                }
            }

            return "";
        }
        
        
        
    }
}
