using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    // Точка над моделью: на неё нижним краем встаёт полоска здоровья в экранном слое интерфейса.
    // Высота точки подгоняется под модель в префабе каждого врага
    public class HealthBarPointRegistrator : MonoEntityRegistrator
    {
        public override void Register(Entity entity)
        {
            entity.AddHealthBarPoint(transform);
        }
    }
}
