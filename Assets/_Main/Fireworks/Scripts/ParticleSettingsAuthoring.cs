using Unity.Entities;
using UnityEngine;

namespace FireworksParticleSystem
{
    public class ParticleSettingsAuthoring : MonoBehaviour
    {
        [Header("Particle Physics")]
        public float gravity = -9.81f;
        public float maxLifeTime = 3f;
        
        [Header("Size Animation")]
        [Range(0f, 2f)]
        public float sizeDecayRate = 0.5f;
        public bool useGrowThenShrink = false;
        [Range(0.1f, 0.9f)]
        public float growthPhaseRatio = 0.3f;

        [Header("Particle Prefab")]
        public GameObject particlePrefab;

        public class Baker : Baker<ParticleSettingsAuthoring>
        {
            public override void Bake(ParticleSettingsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new ParticleSettings
                {
                    Gravity = authoring.gravity,
                    MaxLifeTime = authoring.maxLifeTime,
                    SizeDecayRate = authoring.sizeDecayRate,
                    UseGrowThenShrink = authoring.useGrowThenShrink,
                    GrowthPhaseRatio = authoring.growthPhaseRatio
                });

                AddComponent(entity, new ParticlePrefabReference
                {
                    Prefab = GetEntity(authoring.particlePrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}