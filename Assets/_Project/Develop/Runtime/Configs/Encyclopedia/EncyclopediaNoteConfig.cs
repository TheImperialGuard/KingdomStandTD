using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Encyclopedia
{
    // Заметка раздела «Обучение»: картинка на левой странице, текст — на правой. Абзацы текста разделяются переводом строки.
    [CreateAssetMenu(menuName = "Configs/Encyclopedia/EncyclopediaNoteConfig", fileName = "EncyclopediaNoteConfig")]
    public class EncyclopediaNoteConfig : ScriptableObject
    {
        [field: SerializeField] public Sprite Picture { get; private set; }
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField, TextArea(4, 12)] public string Text { get; private set; }
    }
}
