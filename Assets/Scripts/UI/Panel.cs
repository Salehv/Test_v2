using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class Panel : MonoBehaviour
    {
        public Animator animator;
        public Button[] escapes;

        internal void Appear()
        {
            foreach (var escape in escapes)
            {
                escape.interactable = true;
            }
        }

        internal void Disappear()
        {
            foreach (Button escape in escapes)
            {
                escape.interactable = false;
            }
        }
    }
}