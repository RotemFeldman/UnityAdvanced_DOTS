using Unity.Entities;
using UnityEngine;

public class WallAuthoring : MonoBehaviour
{
    [Header("Wall Settings")]
    public bool useCustomCollision = false;
    public float collisionRadius = 1f; // Only used if useCustomCollision is true
    
    class Baker : Baker<WallAuthoring>
    {
        public override void Bake(WallAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new WallTag());
            AddComponent(entity, new BulletCollisionComponent
            {
                radius = authoring.collisionRadius,
                hasCollided = false,
                collisionNormal = Unity.Mathematics.float3.zero,
                collisionPoint = Unity.Mathematics.float3.zero,
                collidedWith = Entity.Null
            });
        }
    }
}