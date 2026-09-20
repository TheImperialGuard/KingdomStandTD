using Assets._Project.Develop.Runtime.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class LevelCameraBoundsService : IInitializable
    {
        private readonly Level _level;
        private readonly BoundedOrthoCamera _camera;

        public LevelCameraBoundsService(Level level, BoundedOrthoCamera camera)
        {
            _level = level;
            _camera = camera;
        }

        public void Initialize()
        {
            if (_level.HasFieldBounds == false)
                return;

            Bounds fieldBounds = _level.FieldBounds;

            Vector2 min = new Vector2(fieldBounds.min.x, fieldBounds.min.z);
            Vector2 max = new Vector2(fieldBounds.max.x, fieldBounds.max.z);

            _camera.SetBounds(min, max);
        }
    }
}
