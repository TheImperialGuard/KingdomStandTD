using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Вид раздела книги: цвета заголовков и буквицы, значок и подпись заглушки картинки.
    [Serializable]
    public class EncyclopediaSectionStyle
    {
        [field: SerializeField] public EncyclopediaSections Section { get; private set; }
        [field: SerializeField] public Color AccentColor { get; private set; }
        [field: SerializeField] public Color AccentDarkColor { get; private set; }
        [field: SerializeField] public Sprite PlaceholderIcon { get; private set; }
        [field: SerializeField] public string PlaceholderCaption { get; private set; }
    }
}
