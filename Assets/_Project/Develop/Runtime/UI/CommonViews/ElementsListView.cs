using Assets._Project.Develop.Runtime.UI.Core.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.CommonViews
{
    public class ElementsListView<TElement> : MonoBehaviour, IView where TElement : MonoBehaviour, IView
    {
        [SerializeField] private Transform _parent;

        private List<TElement> _elements = new();

        public IReadOnlyList<TElement> Elements => _elements;

        public void AddElement(TElement element)
        {
            element.transform.SetParent(_parent);
            _elements.Add(element);
        }

        public void RemoveElement(TElement element)
        {
            element.transform.SetParent(null);
            _elements.Remove(element);
        }
    }
}
