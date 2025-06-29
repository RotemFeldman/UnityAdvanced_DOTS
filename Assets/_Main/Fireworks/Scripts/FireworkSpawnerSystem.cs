using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FireworksParticleSystem
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct FireworkSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ParticleSettings>();
            state.RequireForUpdate<ParticlePrefabReference>();
            Debug.Log("Prefab-based Spawner System created");
        }

        public void OnUpdate(ref SystemState state)
        {
            var settings = SystemAPI.GetSingleton<ParticleSettings>();
            var prefabRef = SystemAPI.GetSingleton<ParticlePrefabReference>();
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            var currentTime = (float)SystemAPI.Time.ElapsedTime;

            foreach (var spawner in SystemAPI.Query<RefRW<FireworkSpawner>>())
            {
                if (currentTime >= spawner.ValueRO.NextSpawnTime)
                {
                    Debug.Log($"Spawning {spawner.ValueRO.ParticleCount} prefab particles");
                    
                    SpawnPrefabParticles(ref ecb, spawner.ValueRO, settings, prefabRef.Prefab);
                    spawner.ValueRW.NextSpawnTime = currentTime + spawner.ValueRO.SpawnInterval;
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private void SpawnPrefabParticles(ref EntityCommandBuffer ecb, FireworkSpawner spawner, 
                                 ParticleSettings settings, Entity prefab)
        {
            var random = new Unity.Mathematics.Random((uint)(UnityEngine.Time.time * 1000f));

            for (int i = 0; i < spawner.ParticleCount; i++)
            {
                // Instantiate the prefab
                var particleEntity = ecb.Instantiate(prefab);

                // Generate random direction and properties
                float horizontalAngle = random.NextFloat(0f, 2f * math.PI);
                float verticalAngle = random.NextFloat(math.radians(60f), math.radians(90f));

                float3 randomDirection = new float3(
                    math.cos(horizontalAngle) * math.sin(verticalAngle),
                    math.cos(verticalAngle),
                    math.sin(horizontalAngle) * math.sin(verticalAngle)
                );

                float speed = random.NextFloat(0.5f, 1f) * spawner.MaxSpeed;
                float3 initialVelocity = randomDirection * speed;
                float initialSize = random.NextFloat(spawner.MinSize, spawner.MaxSize);

                // Add particle data
                ecb.AddComponent(particleEntity, new ParticleData
                {
                    InitialVelocity = initialVelocity,
                    CurrentVelocity = initialVelocity,
                    InitialSize = initialSize,
                    CurrentSize = initialSize,
                    MinSize = spawner.MinSize,
                    MaxSize = spawner.MaxSize,
                    LifeTime = 0f,
                    MaxLifeTime = settings.MaxLifeTime,
                    Color = spawner.Color
                });

                // Set the material color to override the prefab's color
                ecb.SetComponent(particleEntity, new URPMaterialPropertyBaseColor
                {
                    Value = spawner.Color
                });

                // Set initial transform
                ecb.SetComponent(particleEntity, LocalTransform.FromPositionRotationScale(
                    spawner.SpawnPosition, 
                    quaternion.identity, 
                    initialSize
                ));
            }
        }
    }
}