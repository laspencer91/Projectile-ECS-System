using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Statics
{
    public class EcsEntityManager
    {
        public static EntityArchetype Projectile;

        public static EntityManager entityManager;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            entityManager = World.Active.GetOrCreateManager<EntityManager>();

            Projectile = entityManager.CreateArchetype(typeof(Projectile));
        }

        public static void CreateProjectile(Vector3 startPosition, Vector3 velocity, float damage)
        {
            Entity projectile = entityManager.CreateEntity(Projectile);
            
            entityManager.SetComponentData(projectile, new Projectile
            {
                StartPosition   = new float3(startPosition.x, startPosition.y, startPosition.z),
                CurrentPosition = new float3(startPosition.x, startPosition.y, startPosition.z),
                Velocity        = new float3(velocity.x, velocity.y, velocity.z),
                Damage          = damage
            });
        }

        public static void RemoveEntity(Entity entity)
        {
            entityManager.DestroyEntity(entity);
        }
    }
}