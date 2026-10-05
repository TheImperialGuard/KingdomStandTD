using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Списки окна показывают элементы по номерам. Если число элементов изменилось, список перестраивается,
    // иначе только обновляет видимые строки — так не сбрасываются прокрутка и фокус. Строки и ячейки хранят
    // свой номер в userData: по нему контекстное меню узнаёт строку под курсором.
    public static class CollectionViewIndices
    {
        private static readonly string _textFieldClass = TextInputBaseField<string>.ussClassName;

        public static void Sync(BaseVerticalCollectionView view, List<int> indices, int count)
        {
            if (indices.Count == count)
            {
                // Перетаскивание строки переставляет номера в самом списке — возвращаем их по порядку.
                for (int i = 0; i < count; i++)
                    indices[i] = i;

                view.RefreshItems();
                return;
            }

            indices.Clear();

            for (int i = 0; i < count; i++)
                indices.Add(i);

            view.Rebuild();
        }

        public static void Select(BaseVerticalCollectionView view, int index)
        {
            if (index == LevelAddress.NO_INDEX)
            {
                view.SetSelectionWithoutNotify(Array.Empty<int>());
                return;
            }

            view.SetSelectionWithoutNotify(new[] { index });
            view.ScrollToItem(index);
        }

        // Номер строки, по которой кликнули; NO_INDEX — клик мимо строк.
        public static int FindClickedIndex(EventBase clickEvent, VisualElement view)
        {
            VisualElement indexedElement = FindInside(clickEvent?.target, view, element => element.userData is int);

            return indexedElement != null ? (int)indexedElement.userData : LevelAddress.NO_INDEX;
        }

        // У текстового поля свои копирование, вставка и контекстное меню — список их не перехватывает.
        public static bool IsInTextField(IEventHandler target, VisualElement view)
            => FindInside(target, view, element => element.ClassListContains(_textFieldClass)) != null;

        // Ближайший к цели события элемент внутри view, подходящий под условие, — сама цель или её предок.
        private static VisualElement FindInside(IEventHandler target, VisualElement view, Func<VisualElement, bool> isSuitable)
        {
            VisualElement element = target as VisualElement;

            while (element != null && element != view)
            {
                if (isSuitable(element))
                    return element;

                element = element.parent;
            }

            return null;
        }
    }
}
