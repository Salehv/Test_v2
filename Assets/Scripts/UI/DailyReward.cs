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
        public GameObject timer;
        private Text timer_text;
        private float remainingTime;

        private void Start()
        {
            timer_text = timer.GetComponent<Text>();
        }


        internal void SetAsLocked()
        {
            background.color = Color.gray;
            btnCollect.SetActive(false);
            collectedImage.SetActive(false);
            timer.SetActive(false);
        }


        internal void SetAsAvailable()
        {
            background.color = Color.white;
            btnCollect.SetActive(true);
            collectedImage.SetActive(false);
            timer.SetActive(false);
        }


        internal void SetAsCollected()
        {
            background.color = Color.white;
            btnCollect.SetActive(false);
            collectedImage.SetActive(true);
            timer.SetActive(false);
        }


        internal void SetAsUnavailable(int remain)
        {
            background.color = Color.gray;
            btnCollect.SetActive(false);
            collectedImage.SetActive(false);
            timer.SetActive(true);
            remainingTime = remain;
        }

        public void rewardClicked()
        {
            DailyRewardHandler.instance.GetReward(this);
        }

        private void Update()
        {
            if(timer.activeSelf)
            {
                timer_text.text = Utilities.GetFarsiDailyTime((int) remainingTime);
                remainingTime -= Time.deltaTime;
                if(remainingTime < 0)
                    DailyRewardHandler.instance.Init();
            }
        }
    }
}