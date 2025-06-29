using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(BulletReflectionSystem))]
public partial class BulletCollisionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Check collisions using Unity's built-in physics
        Entities
            .WithoutBurst()
            .ForEach((Entity bulletEntity, ref LocalTransform transform, ref BulletComponent bullet, 
                     ref BulletCollisionComponent collision) =>
            {
                float3 bulletPos = transform.Position;
                float bulletRadius = collision.radius;
                
                // Use Unity's OverlapSphere to detect collisions
                Collider[] hitColliders = Physics.OverlapSphere(bulletPos, bulletRadius);
                
                foreach (var hitCollider in hitColliders)
                {
                    // Skip if it's the bullet's own collider or another bullet
                    if (hitCollider.gameObject.name.Contains("Bullet")) continue;
                    
                    // Check if it's a wall
                    WallAuthoring wallComponent = hitCollider.GetComponent<WallAuthoring>();
                    if (wallComponent != null)
                    {
                        // Calculate collision normal
                        Vector3 closestPoint = hitCollider.ClosestPoint(bulletPos);
                        float3 direction = bulletPos - (float3)closestPoint;
                        
                        // If we're inside the collider, use a default normal
                        if (math.length(direction) < 0.001f)
                        {
                            direction = new float3(0, 1, 0); // Default up direction
                        }
                        
                        float3 normal = math.normalize(direction);
                        
                        collision.hasCollided = true;
                        collision.collisionNormal = normal;
                        collision.collisionPoint = closestPoint;
                        collision.collidedWith = Entity.Null;
                        
                        Debug.Log($"Bullet hit wall! Normal: {normal}, Point: {closestPoint}");
                        
                        break; // Only process first collision
                    }
                }
            }).Run();
    }
}