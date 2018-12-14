using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ProjectileSystem
{
    [UpdateBefore(typeof(ProjectileSimulationSystem))]
    public class EntityRemovalSystem : JobComponentSystem
    {
        struct DestroyGroup
        {
            public readonly int Length;
            public ComponentDataArray<DestroyComponent> ToDestroy;
            [ReadOnly] public EntityArray Entities;
        }
        
        [Inject] private DestroyGroup _destroyGroup;

        [Inject] private EndFrameBarrier _jobBarrier;
        
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            DestroyJob lifetimeJob = new DestroyJob
            {
                Entities  = _destroyGroup.Entities,
                ToDestroy = _destroyGroup.ToDestroy,
                CommandBuffer = _jobBarrier.CreateCommandBuffer().ToConcurrent()
            };

            var jobHandle = lifetimeJob.Schedule(_destroyGroup.Length, 500, inputDeps);

            jobHandle.Complete();
            
            return jobHandle;
        }

        [BurstCompile(Accuracy = Accuracy.High, Support = Support.Strict)]
        struct DestroyJob : IJobParallelFor
        {
            [ReadOnly] public EntityCommandBuffer.Concurrent CommandBuffer;

            public float DeltaTime;
            public EntityArray Entities;
            public ComponentDataArray<DestroyComponent> ToDestroy;

            public void Execute(int i)
            {
                if (ToDestroy[i].Destroy == 1)
                    CommandBuffer.DestroyEntity(i, Entities[i]);
            }
        }
    }
}