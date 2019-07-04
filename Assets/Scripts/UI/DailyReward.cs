using App;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DailyReward : MonoBehaviour
    {
        public DailyRewardType type;
        public int amount;
        public Image background;
        public GameObject collectedImage;
        public GameObject btnCollect;

        internal void SetAsLocked()
        {
            background.color = Color.gray;
            btnCollect.SetActive(false);
            collectedImage.SetActive(false);
        }


        internal void SetAsAvailable()
        {
            background.color = Color.white;
            btnCollect.SetActive(true);
            collectedImage.SetActive(false);
        }


        internal void SetAsCollected()
        {
            background.color = Color.white;
            btnCollect.SetActive(false);
            collectedImage.SetActive(true);
        }


        internal void SetAsUnavailable()
        {
            background.color = Color.gray;
            btnCollect.SetActive(false);
            collectedImage.SetActive(false);
        }

        public void rewardClicked()
        {
            DailyRewardHandler.instance.GetReward(this);
        }
    }
}