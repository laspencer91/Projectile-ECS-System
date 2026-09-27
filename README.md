# Projectile ECS System

An early **Unity ECS / C# projectile-simulation prototype** exploring data-oriented gameplay, the Unity Job System, Burst, and batched physics queries.

**Unity version:** 2018.3.0f2  
**Entities package:** 0.0.12-preview.21

> This project targets Unity's 2018-era experimental ECS APIs. It is preserved as historical systems work and is **not expected to compile unchanged in current Unity/DOTS versions** without migration.

## What it does

- Represents projectiles as ECS entities/components
- Simulates projectile velocity and gravity in parallel jobs
- Builds raycast commands in parallel and executes batched `RaycastCommand` queries
- Uses Burst-compiled `IJobParallelFor` jobs
- Tracks projectile lifetime and marks expired entities for destruction
- Uses command buffers for deferred entity mutation/removal
- Includes a test spawner capable of creating 100 projectiles per input frame

## Performance notes from the original implementation

Comments in the simulation code record approximately:

- ~1.5 ms for ~1,000 projectiles before ECS optimization
- ~0.35 ms for 1,000 projectiles after optimization
- ~3.8 ms for 10,000 projectiles after optimization

These are historical developer measurements from the project, not modern benchmark results.

## Why this repository is useful

This prototype documents early experimentation with data-oriented simulation and parallel gameplay workloads before modern Unity DOTS stabilized. The APIs are obsolete today, but the core engineering concerns remain relevant: structuring data for parallel work, batching physics queries, managing entity lifetime, and avoiding per-projectile MonoBehaviour overhead.
