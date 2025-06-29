using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EntitySpawnerAuthoring : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject PrefabToSpawn;
    public int SpawnCount = 10;
    public float SpawnInterval = 1f;
    public float3 SpawnAreaSize = new float3(10f, 0f, 10f);
    
    [Header("Random Speed Settings")]
    public float MinSpeed = 0.5f;
    public float MaxSpeed = 3f;
    
    [Header("Random Direction Settings")]
    public bool UseRandomYDirection = false;
    
    class Baker : Baker<EntitySpawnerAuthoring>
    {
        public override void Bake(EntitySpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitySpawnerComponent
            {
                PrefabEntity = GetEntity(authoring.PrefabToSpawn, TransformUsageFlags.Dynamic),
                SpawnCount = authoring.SpawnCount,
                SpawnInterval = authoring.SpawnInterval,
                SpawnAreaSize = authoring.SpawnAreaSize,
                MinSpeed = authoring.MinSpeed,
                MaxSpeed = authoring.MaxSpeed,
                UseRandomYDirection = authoring.UseRandomYDirection,
                NextSpawnTime = 0f,
                SpawnedCount = 0
            });
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, SpawnAreaSize);
    }
}