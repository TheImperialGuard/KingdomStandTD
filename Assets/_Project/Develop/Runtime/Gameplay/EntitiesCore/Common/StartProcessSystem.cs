using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common
{
    public abstract class StartProcessSystem : IInitializableSystem, IDisposable
    {
        protected ReactiveEvent StartProcessRequest;
        protected ReactiveEvent StartProcessEvent;

        protected ReactiveVariable<bool> InProcessProcess;

        protected ICompositeCondition CanStartProcess;

        protected IDisposable StartProcessRequestDisposable;

        public abstract void OnInit(Entity entity);

        public void Dispose()
        {
            StartProcessRequestDisposable.Dispose();
        }

        protected void OnStartProcessRequest()
        {
            if (CanStartProcess.Evaluate())
            {
                InProcessProcess.Value = true;
                StartProcessEvent.Invoke();
            }
        }
    }
}
