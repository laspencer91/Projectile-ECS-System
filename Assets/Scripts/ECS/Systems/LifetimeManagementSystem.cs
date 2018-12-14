using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ProjectileSystem
{
    public class LifetimeBarrier : BarrierSystem { }
    
    [UpdateBefore(typeof(ProjectileSimulationSystem))]
    public class LifetimeManagementSystem : JobComponentSystem
    {  
        struct LifeTimeGroup
        {
            public readonly int Length;
            public ComponentDataArray<LifetimeComponent> Lifetimes;
            [ReadOnly] public EntityArray Entities;
        }
        
        [Inject] private LifeTimeGroup lifetimeGroup;

        [Inject] private LifetimeBarrier _jobBarrier;

        public static DestroyComponent DestroyComponentTrue = new DestroyComponent { Destroy = 1};
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            LifetimeJob lifetimeJob = new LifetimeJob
            {
                DeltaTime = Time.deltaTime,
                Entities  = lifetimeGroup.Entities,
                Lifetimes = lifetimeGroup.Lifetimes,
                CommandBuffer = _jobBarrier.CreateCommandBuffer().ToConcurrent()
            };

            var jobHandle = lifetimeJob.Schedule(lifetimeGroup.Length, 250, inputDeps);

            jobHandle.Complete();
            
            return jobHandle;
        }

        [BurstCompile(Accuracy = Accuracy.High, Support = Support.Strict)]
        struct LifetimeJob : IJobParallelFor
        {
            [ReadOnly] public EntityCommandBuffer.Concurrent CommandBuffer;

            public float DeltaTime;
            public EntityArray Entities;
            public ComponentDataArray<LifetimeComponent> Lifetimes;

            public void Execute(int i)
            {
                LifetimeComponent lifetime = Lifetimes[i];

                lifetime.TimeAlive += DeltaTime;

                if (lifetime.TimeAlive > lifetime.MaxTimeAlive)
                {
                    CommandBuffer.SetComponent(i, Entities[i], DestroyComponentTrue);
                    return;
                }

                Lifetimes[i] = lifetime;
            }
        }
    }
}