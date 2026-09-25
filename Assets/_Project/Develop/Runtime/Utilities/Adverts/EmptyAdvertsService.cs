using System;

namespace Assets._Project.Develop.Runtime.Utilities.Adverts
{
    public class EmptyAdvertsService : IAdvertsService
    {
        public event Action RewardedAdvertFailed
        {
            add
            {
            }

            remove
            {
            }
        }

        public bool IsAvailable => false;

        public void OpenFullScreenAdvert()
        {
        }

        public void OpenRewardedAdvert(Action callback)
        {
        }
    }
}
