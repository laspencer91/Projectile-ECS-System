using ECS.Components;
using ECS.Statics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

// 1.5ms for ~1000 projectiles
// 0.35ms for 1000 projectiles - After ECS Optimization
// 3.8ms for 10000 projectiles - After ECS Optimization
namespace ProjectileSystem
{
    public class ProjectileBarrier : BarrierSystem { }
    
    [UpdateAfter(typeof(LifetimeManagementSystem))]
    public class ProjectileSimulationSystem : JobComponentSystem
    {
        public struct Data
        {
            public readonly int Length;
            public ComponentDataArray<Projectile> Projectile;
            
            [ReadOnly] public EntityArray Entities;
        }
        
        [Inject] private Data _data;
        
        [Inject] private ProjectileBarrier _jobBarrier;
        
        public static DestroyComponent DestroyComponentTrue = new DestroyComponent { Destroy = 1};
        
        /**
         * Main Update Method
         */
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int numOFProjectiles = _data.Length;
            int batchSize = 500;
            
            var raycastCommands = new NativeArray<RaycastCommand>(numOFProjectiles, Allocator.TempJob);
            var raycastHits     = new NativeArray<RaycastHit>(numOFProjectiles, Allocator.TempJob);
            
            var raycastCommandsJob = new PrepareRaycastCommands
            {
                DeltaTime   = Time.deltaTime,
                Commands    = raycastCommands,
                Projectiles = _data.Projectile
            };

            // SCHEDULE Job for creating raycast commands
            JobHandle raycastDependency = 
                raycastCommandsJob.Schedule(numOFProjectiles, batchSize);
            
            // SCHEDULE Raycast job
            JobHandle raycastJobHandle =
                RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, batchSize, raycastDependency);
            
            // Update Projectiles with new state
            var projectileStateUpdate = new ProjectileHitUpdateJob
            {
                Projectiles = _data.Projectile,
                RaycastHits = raycastHits,
                Entities  = _data.Entities,
                CommandBuffer = _jobBarrier.CreateCommandBuffer().ToConcurrent()
            };
            
            JobHandle stateUpdateJob = projectileStateUpdate.Schedule(numOFProjectiles, batchSize, raycastJobHandle);
            
            stateUpdateJob.Complete();
            
            raycastCommands.Dispose();
            raycastHits.Dispose();
            
            return stateUpdateJob;
        }
        
        /**
         * Hit update Job
         */
        [BurstCompile(Accuracy=Accuracy.High, Support=Support.Strict)]
        struct ProjectileHitUpdateJob : IJobParallelFor
        {
            [ReadOnly] public EntityCommandBuffer.Concurrent CommandBuffer;
            
            public ComponentDataArray<Projectile> Projectiles;
            public NativeArray<RaycastHit> RaycastHits;
            public EntityArray Entities;
            
            public void Execute(int i)
            {
                bool wasHit = RaycastHits[i].normal != Vector3.zero;

                if (wasHit)
                {
                    CommandBuffer.SetComponent(i, Entities[i], DestroyComponentTrue);
                    return;
                }

                Projectile proj = Projectiles[i];
                Projectiles[i] = proj;
            }
        }
        
        /**
         * Prepare Raycast Job
         */
        [BurstCompile(Accuracy=Accuracy.High, Support=Support.Strict)]
        struct PrepareRaycastCommands : IJobParallelFor
        {
            public float DeltaTime;
            public NativeArray<RaycastCommand>    Commands;
            public ComponentDataArray<Projectile> Projectiles;

            private const float Gravity = -9.81f;
            private const float TerminalVelocity = -80;

            public void Execute(int i)
            {
                Projectile proj = Projectiles[i];
                
                if (proj.Velocity.y > TerminalVelocity)
                    proj.Velocity.y += Gravity * DeltaTime;

                float3 position = Projectiles[i].Position;
                
                float3 nextPosition  = position + proj.Velocity * DeltaTime;
                float3 direction     = math.normalize(nextPosition - position);
                float  positionDelta = math.distance(nextPosition, position);

                Commands[i]  = new RaycastCommand(position, direction, positionDelta);

                proj.Position = nextPosition;
                
                Projectiles[i] = proj;
            }
        }
    }
}