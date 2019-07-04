using System.Collections;
using System.Collections.Generic;
using TheGame;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace App
{
    public class DailyRewardHandler : MonoBehaviour
    {
        internal static DailyRewardHandler instance;
        public Transform rewardsParent;
        private DailyReward[] rewards;
        private int collectedRewards;

        private void Awake()
        {
            instance = this;
        }

        internal void Init()
        {
            if (!PlayerPrefs.HasKey("reward_collected"))
            {
                PlayerPrefs.SetInt("reward_collected", -1);
                PlayerPrefs.Save();
            }

            collectedRewards = PlayerPrefs.GetInt("reward_collected");
            print($"[DailyReward] CollectedReward:{collectedRewards}");

            if (!TimeManager.instance.HasRealTimer("last_reward_collected"))
                TimeManager.instance.SetRealTimer("last_reward_collected");

            int timePassed = TimeManager.instance.GetCurrentRealTime("last_reward_collected");
            print($"[DailyReward] TimePassed:{timePassed}");

            InitRewardList(collectedRewards, timePassed);
        }

        private void InitRewardList(int collected, int timePassed)
        {
            SetRewardReferences();

            if (collected == -1)
            {
                rewards[0].SetAsAvailable();

                for (int i = 1; i < rewards.Length; i++)
                {
                    rewards[i].SetAsLocked();
                }

                return;
            }

            for (int i = 0; i <= collected; i++)
            {
                rewards[i].SetAsCollected();
            }

            if (collected == rewards.Length - 1)
                return;


            if (timePassed > 24 * 60 * 60)
                rewards[collected + 1].SetAsAvailable();
            else
                rewards[collected + 1].SetAsUnavailable();


            for (int i = collected + 2; i < rewards.Length; i++)
            {
                rewards[i].SetAsLocked();
            }
        }

        private void SetRewardReferences()
        {
            rewards = new DailyReward[rewardsParent.childCount];
            for (int i = 0; i < rewardsParent.childCount; i++)
            {
                rewards[i] = rewardsParent.GetChild(i).GetComponent<DailyReward>();
            }
        }

        public void GetReward(DailyReward dailyReward)
        {
            if (dailyReward.type == DailyRewardType.COIN)
                GameManager.instance.AddCoins(dailyReward.amount);
            else if (dailyReward.type == DailyRewardType.GEM)
                ApplicationManager.instance.AddGems(dailyReward.amount);
            else
                return;

            int collected = PlayerPrefs.GetInt("reward_collected");
            PlayerPrefs.SetInt("reward_collected", collected + 1);
            PlayerPrefs.Save();

            TimeManager.instance.SetRealTimer("last_reward_collected");

            Init();
        }
    }


    [System.Serializable]
    public enum DailyRewardType
    {
        COIN,
        GEM
    }
}