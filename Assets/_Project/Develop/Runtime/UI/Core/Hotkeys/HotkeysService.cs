using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.UI.Core.Hotkeys
{
    // Превращает опрос клавиш интерфейса в события, на которые подписываются презентеры.
    // Тикается из Update бутстрапа сцены.
    public class HotkeysService
    {
        private readonly IInputService _inputService;
        private readonly ReactiveEvent _closeRequested = new();
        private readonly ReactiveEvent _previousRequested = new();
        private readonly ReactiveEvent _nextRequested = new();

        public HotkeysService(IInputService inputService)
        {
            _inputService = inputService;
        }

        public IReadOnlyEvent CloseRequested => _closeRequested;

        public IReadOnlyEvent PreviousRequested => _previousRequested;

        public IReadOnlyEvent NextRequested => _nextRequested;

        public void Update()
        {
            if (_inputService.CloseRequested)
                _closeRequested.Invoke();

            if (_inputService.PreviousRequested)
                _previousRequested.Invoke();

            if (_inputService.NextRequested)
                _nextRequested.Invoke();
        }
    }
}
