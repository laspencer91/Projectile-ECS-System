using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Components
{
    public struct Projectile : IComponentData
    {
        public float3 Position;
        public float3 Velocity;
        public float  Damage;
    }
}