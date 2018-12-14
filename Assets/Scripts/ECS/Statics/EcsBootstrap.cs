using System;
using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ECS.Statics
{
    public class EcsBootstrap
    {
        public static EntityArchetype Projectile;
        
        public static EntityManager EntityManager;

        public static MeshInstanceRenderer ProjectileRenderer;
        
        /** Create Archetypes Before Scene Load **/
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            EntityManager = World.Active.GetOrCreateManager<EntityManager>();

            CreateArchetypes();
        }

        /** Load Prototype Of The Scene **/
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeAfterSceneLoad()
        {
            LoadPrototypes();
        }

        private static void CreateArchetypes()
        {
            Projectile = EntityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Projectile), typeof(LifetimeComponent), typeof(DestroyComponent));
        }
        
        private static void LoadPrototypes()
        {
            var renderer = GetRendererFromPrototype("ProjectileRenderPrototype");
            
            if (renderer.HasValue) ProjectileRenderer = renderer.Value;
        }
        
        private static MeshInstanceRenderer? GetRendererFromPrototype(string protoName)
        {
            var proto = GameObject.Find(protoName);
            
            if (proto == null) return null;
            
            var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
            Object.Destroy(proto);
            
            return result;
        }
        
        public static void CreateProjectile(Vector3 startPosition, Vector3 velocity, Quaternion rotation, float lifetime, float damage)
        {
            Entity projectile = EntityManager.CreateEntity(Projectile);
            
            EntityManager.SetComponentData(projectile, new Projectile
            {
                Velocity  = new float3(velocity.x, velocity.y, velocity.z),
                Damage    = damage,
                Position  = new float3(startPosition.x, startPosition.y, startPosition.z)
            });
            
            EntityManager.SetComponentData(projectile, new Rotation { Value = rotation});
            EntityManager.SetComponentData(projectile, new LifetimeComponent { MaxTimeAlive = lifetime});
        }
    }
}