using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FireworksParticleSystem
{
    public class FireworkSpawnerAuthoring : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public int particleCount = 50;
        public float maxSpeed = 10f;
        public Color color = Color.red;
        public float spawnInterval = 2f;

        [Header("Particle Size Settings")]
        [Range(0.1f, 5f)]
        public float minParticleSize = 0.5f;
        [Range(0.1f, 5f)]
        public float maxParticleSize = 1.5f;

        public class Baker : Baker<FireworkSpawnerAuthoring>
        {
            public override void Bake(FireworkSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // Only add the spawner component, not the settings
                AddComponent(entity, new FireworkSpawner
                {
                    SpawnPosition = authoring.transform.position,
                    ParticleCount = authoring.particleCount,
                    MaxSpeed = authoring.maxSpeed,
                    Color = new float4(authoring.color.r, authoring.color.g, authoring.color.b, authoring.color.a),
                    SpawnInterval = authoring.spawnInterval,
                    NextSpawnTime = 0f,
                    MinSize = authoring.minParticleSize,
                    MaxSize = authoring.maxParticleSize
                });
            }
        }
    }
}