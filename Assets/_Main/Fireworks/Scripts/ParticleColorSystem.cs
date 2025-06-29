using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace FireworksParticleSystem
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct ParticleColorSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (particleData, materialColor) in 
                     SystemAPI.Query<RefRO<ParticleData>, RefRW<MaterialColor>>())
            {
                // Convert float4 color to Color and apply it
                materialColor.ValueRW.Value = new float4(
                    particleData.ValueRO.Color.x,
                    particleData.ValueRO.Color.y,
                    particleData.ValueRO.Color.z,
                    particleData.ValueRO.Color.w
                );
            }
        }
    }
}