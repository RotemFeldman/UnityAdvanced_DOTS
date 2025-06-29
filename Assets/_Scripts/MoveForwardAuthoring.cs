using Unity.Entities;
using UnityEngine;

public class MoveForwardAuthoring : MonoBehaviour
{
    public Vector3 Direction = Vector3.forward;
    public float Speed = 1f;

    class Baker : Baker<MoveForwardAuthoring>
    {
        public override void Bake(MoveForwardAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveForward
            {
                Direction = authoring.Direction,
                Speed = authoring.Speed
            });
        }
    }
}