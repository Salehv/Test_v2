using UnityEngine;

namespace UI
{
    public class TransitionEnd : MonoBehaviour
    {
        public void TransitionEnded()
        {
            TransitionHandler.instance.TransitionEnded();
        }
    }
}