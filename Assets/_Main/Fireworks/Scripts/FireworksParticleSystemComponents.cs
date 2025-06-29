using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FireworksParticleSystem
{
    public struct FireworkSpawner : IComponentData
    {
        public float3 SpawnPosition;
        public int ParticleCount;
        public float MaxSpeed;
        public float4 Color;
        public float SpawnInterval;
        public float NextSpawnTime;
        public float MinSize;
        public float MaxSize;
    }

    public struct ParticleData : IComponentData
    {
        public float3 InitialVelocity;
        public float3 CurrentVelocity;
        public float InitialSize;
        public float CurrentSize;
        public float MinSize;
        public float MaxSize;
        public float LifeTime;
        public float MaxLifeTime;
        public float4 Color;
    }

    public struct ParticleSettings : IComponentData
    {
        public float Gravity;
        public float MaxLifeTime;
        public float SizeDecayRate;
        public bool UseGrowThenShrink;
        public float GrowthPhaseRatio; // 0.0 to 1.0 - how much of lifetime is growth phase
    }
    
    public struct ParticlePrefabReference : IComponentData
    {
        public Entity Prefab;
    }

}