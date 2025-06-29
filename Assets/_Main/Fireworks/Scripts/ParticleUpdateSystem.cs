using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FireworksParticleSystem
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ParticleUpdateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ParticleSettings>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var settings = SystemAPI.GetSingleton<ParticleSettings>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

            foreach (var (particle, transform, entity) in 
                     SystemAPI.Query<RefRW<ParticleData>, RefRW<LocalTransform>>().WithEntityAccess())
            {
                // Update lifetime
                particle.ValueRW.LifeTime += deltaTime;

                // Check if particle should be destroyed
                if (particle.ValueRO.LifeTime >= particle.ValueRO.MaxLifeTime)
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }

                // Apply gravity
                particle.ValueRW.CurrentVelocity.y += settings.Gravity * deltaTime;

                // Update position
                transform.ValueRW.Position += particle.ValueRO.CurrentVelocity * deltaTime;

                // Calculate size based on settings
                float newSize = CalculateParticleSize(particle.ValueRO, settings);
                particle.ValueRW.CurrentSize = newSize;
                
                // Update transform scale
                transform.ValueRW.Scale = newSize;
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private float CalculateParticleSize(ParticleData particle, ParticleSettings settings)
        {
            float lifeRatio = particle.LifeTime / particle.MaxLifeTime;
            
            if (settings.UseGrowThenShrink)
            {
                // Grow-then-shrink behavior
                if (lifeRatio <= settings.GrowthPhaseRatio)
                {
                    // Growth phase: start small, grow to initial size
                    float growthProgress = lifeRatio / settings.GrowthPhaseRatio;
                    float startSize = particle.InitialSize * 0.1f; // Start at 10% of initial size
                    return math.lerp(startSize, particle.InitialSize, growthProgress);
                }
                else
                {
                    // Shrink phase: shrink from initial size
                    float shrinkProgress = (lifeRatio - settings.GrowthPhaseRatio) / (1f - settings.GrowthPhaseRatio);
                    float endSize = particle.InitialSize * 0.1f; // End at 10% of initial size
                    return math.lerp(particle.InitialSize, endSize, shrinkProgress * settings.SizeDecayRate);
                }
            }
            else
            {
                // Traditional decay: start at initial size, shrink over time
                return particle.InitialSize * (1f - lifeRatio * settings.SizeDecayRate);
            }
        }
    }
}