using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Команды редактора для списка окна: копировать, вставить, дублировать и удалить — по умолчанию Ctrl+C,
    // Ctrl+V, Ctrl+D и Del. Команду получает список в фокусе. Пока фокус в текстовом поле внутри списка,
    // команды остаются полю — копирование и вставка текста.
    public static class CollectionViewCommands
    {
        private const string COPY_COMMAND = "Copy";
        private const string PASTE_COMMAND = "Paste";
        private const string DUPLICATE_COMMAND = "Duplicate";
        private const string DELETE_COMMAND = "Delete";
        private const string SOFT_DELETE_COMMAND = "SoftDelete";

        public static void Register(VisualElement view, Action copy, Action paste, Action duplicate, Action delete)
        {
            Dictionary<string, Action> actions = new()
            {
                { COPY_COMMAND, copy },
                { PASTE_COMMAND, paste },
                { DUPLICATE_COMMAND, duplicate },
                { DELETE_COMMAND, delete },
                { SOFT_DELETE_COMMAND, delete },
            };

            // Подтверждение на проверке говорит редактору, что команду выполнит этот список.
            view.RegisterCallback<ValidateCommandEvent>(commandEvent =>
            {
                if (TryFindAction(view, actions, commandEvent, out _))
                    commandEvent.StopPropagation();
            });

            view.RegisterCallback<ExecuteCommandEvent>(commandEvent =>
            {
                if (TryFindAction(view, actions, commandEvent, out Action action) == false)
                    return;

                commandEvent.StopPropagation();
                action();
            });
        }

        private static bool TryFindAction<TEvent>(
            VisualElement view,
            IReadOnlyDictionary<string, Action> actions,
            TEvent commandEvent,
            out Action action)
            where TEvent : EventBase, ICommandEvent
        {
            action = null;

            if (CollectionViewIndices.IsInTextField(commandEvent.target, view))
                return false;

            return actions.TryGetValue(commandEvent.commandName, out action);
        }
    }
}
