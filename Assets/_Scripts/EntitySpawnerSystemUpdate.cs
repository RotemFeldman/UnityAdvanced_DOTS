using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EntitySpawnerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var currentTime = (float)SystemAPI.Time.ElapsedTime;
        var random = Unity.Mathematics.Random.CreateFromIndex((uint)(currentTime * 1000));
        
        foreach (var (spawner, transform, entity) in SystemAPI.Query<RefRW<EntitySpawnerComponent>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            ref var spawnerData = ref spawner.ValueRW;
            
            // Check if it's time to spawn
            if (currentTime < spawnerData.NextSpawnTime) continue;
            
            // Spawn the specified count of entities
            for (int i = 0; i < spawnerData.SpawnCount; i++)
            {
                // Spawn new entity
                var spawnedEntity = ecb.Instantiate(spawnerData.PrefabEntity);
                
                // Random position within spawn area
                float3 randomOffset = new float3(
                    random.NextFloat(-spawnerData.SpawnAreaSize.x / 2f, spawnerData.SpawnAreaSize.x / 2f),
                    random.NextFloat(-spawnerData.SpawnAreaSize.y / 2f, spawnerData.SpawnAreaSize.y / 2f),
                    random.NextFloat(-spawnerData.SpawnAreaSize.z / 2f, spawnerData.SpawnAreaSize.z / 2f)
                );
                
                float3 spawnPosition = transform.ValueRO.Position + randomOffset;
                
                // Set position
                ecb.SetComponent(spawnedEntity, LocalTransform.FromPosition(spawnPosition));
                
                // Random direction
                float3 randomDirection;
                if (spawnerData.UseRandomYDirection)
                {
                    randomDirection = math.normalize(new float3(
                        random.NextFloat(-1f, 1f),
                        random.NextFloat(-1f, 1f),
                        random.NextFloat(-1f, 1f)
                    ));
                }
                else
                {
                    randomDirection = math.normalize(new float3(
                        random.NextFloat(-1f, 1f),
                        0f,
                        random.NextFloat(-1f, 1f)
                    ));
                }
                
                // Random speed
                float randomSpeed = random.NextFloat(spawnerData.MinSpeed, spawnerData.MaxSpeed);
                
                // Set or override the MoveForward component
                ecb.SetComponent(spawnedEntity, new MoveForward
                {
                    Direction = randomDirection,
                    Speed = randomSpeed
                });
                
                // Update random for next entity
                random = Unity.Mathematics.Random.CreateFromIndex((uint)(random.NextUInt() + i));
            }
            
            // Update next spawn time
            spawnerData.NextSpawnTime = currentTime + spawnerData.SpawnInterval;
            spawnerData.SpawnedCount += spawnerData.SpawnCount;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}