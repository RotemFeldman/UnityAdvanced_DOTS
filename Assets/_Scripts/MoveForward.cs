using Unity.Entities;
using Unity.Mathematics;

public struct MoveForward : IComponentData
{
    public float3 Direction;
    public float Speed;
}
