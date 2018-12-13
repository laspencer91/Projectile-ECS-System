using ECS.Components;
using ECS.Statics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// 1.5ms for ~1000 projectiles

namespace ProjectileSystem
{
    public class DestroyBarrier : BarrierSystem { }
    
    public class ProjectileSimulationSystem : JobComponentSystem
    {
        float _gravity = -9.81f;

        [BurstCompile]
        struct ProjectileHitUpdateJob : IJobParallelFor
        {
            public ComponentDataArray<Projectile> Projectiles;
            public NativeArray<RaycastHit> RaycastHits;
            public EntityArray Entities;
            [ReadOnly] public EntityCommandBuffer CommandBuffer;
            public float DeltaTime;
            
            public void Execute(int i)
            {
                bool wasHit = RaycastHits[i].normal != Vector3.zero;
                
                Projectile proj = Projectiles[i];

                if (wasHit)
                {
                    CommandBuffer.DestroyEntity(Entities[i]);
                    return;
                }

                //proj.Destroy = wasHit;
                proj.TimeAlive += DeltaTime;
                Projectiles[i] = proj;
            }
        }
        
        
        [BurstCompile(Accuracy=Accuracy.High, Support=Support.Strict)]
        struct PrepareRaycastCommands : IJobParallelFor
        {
            public float DeltaTime;
            public float Gravity;
            
            public NativeArray<RaycastCommand> Commands;
            public ComponentDataArray<Projectile> Projectiles;
            
            public void Execute(int i)
            {
                Projectile proj = Projectiles[i];
                proj.Velocity.y += Gravity * DeltaTime;
                
                float3 nextPosition  = proj.CurrentPosition + proj.Velocity * DeltaTime;
                float3 direction     = math.normalize(nextPosition - proj.CurrentPosition);
                float  positionDelta = math.distance(nextPosition, proj.CurrentPosition);

                Commands[i] = new RaycastCommand(proj.CurrentPosition, direction, positionDelta);
                
                proj.CurrentPosition = nextPosition;
                
                Projectiles[i] = proj;
            }
        }

       
        public struct Data
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            public ComponentDataArray<Projectile> Projectile;
        }
        
        [Inject] private Data _data;
        
        [Inject] private DestroyBarrier _destroyBarrier;
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int amountOFProjectiles = _data.Length;
            
            var raycastCommands = new NativeArray<RaycastCommand>(amountOFProjectiles, Allocator.TempJob);
            var raycastHits     = new NativeArray<RaycastHit>(amountOFProjectiles, Allocator.TempJob);
            
            var raycastCommandsJob = new PrepareRaycastCommands
            {
                DeltaTime   = Time.deltaTime,
                Gravity     = _gravity,
                Commands    = raycastCommands,
                Projectiles = _data.Projectile
            };

            // SCHEDULE Job for creating raycast commands
            var raycastDependency = raycastCommandsJob.Schedule(amountOFProjectiles, 500);
            
            
            // SCHEDULE Raycast job
            JobHandle raycastJobHandle =
                RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 500, raycastDependency);
            
            // Update Projectiles with new state
            var projectileStateUpdate = new ProjectileHitUpdateJob
            {
                Projectiles = _data.Projectile,
                RaycastHits = raycastHits,
                DeltaTime = Time.deltaTime,
                Entities  = _data.Entities,
                CommandBuffer = _destroyBarrier.CreateCommandBuffer()
            };
            
            JobHandle stateUpdateJob = projectileStateUpdate.Schedule(amountOFProjectiles, 500, raycastJobHandle);
            
            stateUpdateJob.Complete();
            
            raycastCommands.Dispose();
            raycastHits.Dispose();
            
            return stateUpdateJob;
        }
    }
}