using UnityEngine;
using UnityEngine.Video;

namespace Initialize
{
    public class SplashScreenHandler : MonoBehaviour
    {
        void Start()
        {
            GetComponent<VideoPlayer>().loopPointReached += SplashScreenEnd;
        }

        private void SplashScreenEnd(VideoPlayer p)
        {
            GameLoader.instance.SplashEnded();
        }
    }
}