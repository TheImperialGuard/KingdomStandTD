using System;

namespace Assets._Project.Develop.Runtime.Utilities.Adverts
{
    public interface IAdvertsService
    {
        event Action RewardedAdvertFailed;

        bool IsAvailable { get; }

        void OpenFullScreenAdvert();
        void OpenRewardedAdvert(Action callback);
    }
}
