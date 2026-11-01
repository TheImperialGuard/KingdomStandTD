using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Endless
{
    // Шаг «Бесконечный режим» доски «Играть»: описание двумя абзацами, рекорд и «Играть». ◀ шага — у вью доски: навигация
    // между шагами остаётся за ней.
    public class EndlessStepView : MonoBehaviour
    {
        public event Action PlayClicked;

        [SerializeField] private TMP_Text _goal;
        [SerializeField] private TMP_Text _advice;
        [SerializeField] private TMP_Text _record;
        [SerializeField] private Button _playButton;

        private void OnEnable() => _playButton.onClick.AddListener(OnPlayButtonClicked);

        private void OnDisable() => _playButton.onClick.RemoveListener(OnPlayButtonClicked);

        public void SetDescription(string goal, string advice)
        {
            _goal.text = goal;
            _advice.text = advice;
        }

        public void SetRecord(string record) => _record.text = record;

        private void OnPlayButtonClicked() => PlayClicked?.Invoke();
    }
}
