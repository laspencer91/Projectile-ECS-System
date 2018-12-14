using Unity.Entities;

namespace ECS.Components
{
    public struct DestroyComponent : IComponentData
    {
        public byte Destroy;
    }
}