using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;


public partial struct ChangeColorSystem : ISystem
{
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random(1234);
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (entity,material) in SystemAPI.Query<RefRW<ChangeColor>, RefRW<URPMaterialPropertyBaseColor>>())
        {
            entity.ValueRW.timeSinceLastChange += deltaTime;
            if (entity.ValueRW.timeSinceLastChange > entity.ValueRO.timeToChange)
            {
                float4 newColor;
                newColor.x = random.NextFloat(0f, 1f);
                newColor.y = random.NextFloat(0f, 1f);
                newColor.z = random.NextFloat(0f, 1f);
                newColor.w = 1f;

                material.ValueRW.Value = newColor;
                entity.ValueRW.timeSinceLastChange = 0f;
            }
        }
    }

    
}
