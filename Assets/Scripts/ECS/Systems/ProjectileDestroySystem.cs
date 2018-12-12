
using ECS.Components;
using Unity.Collections;
using Unity.Entities;

namespace ProjectileSystem
{
    public class ProjectileDestroySystem : ComponentSystem
    {
        struct DestroyedProjectiles
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<Projectile> Projectiles;
        }

        [Inject] private DestroyedProjectiles _toDestroy;
        
        protected override void OnUpdate()
        {
            for (int i = 0; i < _toDestroy.Length; i++)
            {
                if (!_toDestroy.Projectiles[i].Destroy) return;
                
                PostUpdateCommands.DestroyEntity(_toDestroy.Entities[i]);
            }
        }
    }
}