
using Unity.Entities;

public struct ChangeColor : IComponentData
{
    public float timeToChange;
    public float timeSinceLastChange;
}