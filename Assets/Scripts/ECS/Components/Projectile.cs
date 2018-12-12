using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct Projectile : IComponentData
    {
        public float3 StartPosition;
        public float3 CurrentPosition;
        public float3 Velocity;
        public float  TimeAlive;
        public float  Damage;
        public TBool  Destroy;
    }
}