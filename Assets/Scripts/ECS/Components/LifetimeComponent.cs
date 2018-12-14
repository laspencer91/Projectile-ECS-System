using Unity.Entities;

namespace ECS.Components
{
    public struct LifetimeComponent : IComponentData
    {
        public float TimeAlive;
        public float MaxTimeAlive;
    }
}