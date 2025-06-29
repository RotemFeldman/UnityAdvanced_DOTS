
using Unity.Entities;
using Unity.Mathematics;

public struct EntitySpawnerComponent : IComponentData
{
    public Entity PrefabEntity;
    public int SpawnCount;
    public float SpawnInterval;
    public float3 SpawnAreaSize;
    public float MinSpeed;
    public float MaxSpeed;
    public bool UseRandomYDirection;
    public float NextSpawnTime;
    public int SpawnedCount;
}