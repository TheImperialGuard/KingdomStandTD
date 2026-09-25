using System;
using YG;

namespace Assets._Project.Develop.Runtime.Utilities.Adverts
{
    public class YGAdvertsService : IAdvertsService, IDisposable
    {
        public event Action RewardedAdvertFailed;

        public YGAdvertsService()
        {
            YG2.onErrorRewardedAdv += OnErrorReward;
        }

        public bool IsAvailable { get; } = true;

        public void OpenFullScreenAdvert()
        {
            YG2.InterstitialAdvShow();
        }

        public void OpenRewardedAdvert(Action callback)
        {
            YG2.RewardedAdvShow(string.Empty, callback);
        }

        public void Dispose()
        {
            YG2.onErrorRewardedAdv -= OnErrorReward;
        }

        private void OnErrorReward()
        {
            RewardedAdvertFailed?.Invoke();
        }
    }
}
