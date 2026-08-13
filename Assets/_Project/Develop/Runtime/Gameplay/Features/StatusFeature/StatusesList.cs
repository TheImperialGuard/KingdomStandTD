using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature
{
    public class StatusesList
    {
        public event Action<Status> Added;
        public event Action<Status> Removed;

        private List<Status> _elements = new();

        public IReadOnlyList<Status> Elements => _elements;

        public virtual void AddElement(Status element)
        {
            Status status = _elements.FirstOrDefault(status => status.Type == element.Type);

            if (status == null)
            {
                status = element;
            }

            _elements.Add(status);

            Added?.Invoke(status);
        }

        public virtual void RemoveElement(Status element)
        {
            _elements.Remove(element);
            Removed?.Invoke(element);
        }
    }
}
