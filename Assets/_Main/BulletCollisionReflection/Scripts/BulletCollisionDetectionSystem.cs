using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[Unity.Burst.BurstCompile]
public struct BulletVsBulletCollisionJob : IJob
{
    [ReadOnly] public NativeArray<Entity> BulletEntities;
    [ReadOnly] public NativeArray<LocalTransform> BulletTransforms;
    [ReadOnly] public NativeArray<BulletCollisionComponent> BulletCollisions;
    public ComponentLookup<BulletCollisionComponent> CollisionLookup;
    
    public void Execute()
    {
        for (int i = 0; i < BulletEntities.Length; i++)
        {
            for (int j = i + 1; j < BulletEntities.Length; j++)
            {
                if (CheckSphereCollision(
                    BulletTransforms[i].Position, BulletCollisions[i].radius,
                    BulletTransforms[j].Position, BulletCollisions[j].radius,
                    out float3 normal, out float3 contactPoint))
                {
                    // Update collision data for both bullets
                    var collision1 = CollisionLookup[BulletEntities[i]];
                    collision1.hasCollided = true;
                    collision1.collisionNormal = normal;
                    collision1.collisionPoint = contactPoint;
                    collision1.collidedWith = BulletEntities[j];
                    CollisionLookup[BulletEntities[i]] = collision1;
                    
                    var collision2 = CollisionLookup[BulletEntities[j]];
                    collision2.hasCollided = true;
                    collision2.collisionNormal = -normal;
                    collision2.collisionPoint = contactPoint;
                    collision2.collidedWith = BulletEntities[i];
                    CollisionLookup[BulletEntities[j]] = collision2;
                }
            }
        }
    }
    
    private bool CheckSphereCollision(float3 posA, float radiusA, float3 posB, float radiusB,
        out float3 normal, out float3 contactPoint)
    {
        float3 direction = posB - posA;
        float distance = math.length(direction);
        float combinedRadius = radiusA + radiusB;
        
        if (distance < combinedRadius && distance > 0.0f)
        {
            normal = math.normalize(direction);
            contactPoint = posA + normal * radiusA;
            return true;
        }
        
        normal = float3.zero;
        contactPoint = float3.zero;
        return false;
    }
}

public struct BulletVsWallCollisionJob : IJob
{
    [ReadOnly] public NativeArray<Entity> BulletEntities;
    [ReadOnly] public NativeArray<LocalTransform> BulletTransforms;
    [ReadOnly] public NativeArray<BulletCollisionComponent> BulletCollisions;
    [ReadOnly] public NativeArray<Entity> WallEntities;
    [ReadOnly] public NativeArray<LocalTransform> WallTransforms;
    public ComponentLookup<BulletCollisionComponent> CollisionLookup;
    
    public void Execute()
    {
        for (int i = 0; i < BulletEntities.Length; i++)
        {
            float3 bulletPos = BulletTransforms[i].Position;
            float bulletRadius = BulletCollisions[i].radius;
            
            for (int j = 0; j < WallEntities.Length; j++)
            {
                // Check collision with wall using Unity's built-in collider
                if (CheckWallCollision(bulletPos, bulletRadius, WallTransforms[j], 
                    out float3 normal, out float3 contactPoint))
                {
                    var collision = CollisionLookup[BulletEntities[i]];
                    collision.hasCollided = true;
                    collision.collisionNormal = normal;
                    collision.collisionPoint = contactPoint;
                    collision.collidedWith = WallEntities[j];
                    CollisionLookup[BulletEntities[i]] = collision;
                }
            }
        }
    }
    
    private bool CheckWallCollision(float3 bulletPos, float bulletRadius, LocalTransform wallTransform,
        out float3 normal, out float3 contactPoint)
    {
        // This is a simplified example using sphere vs box collision
        // In a real implementation, you'd query the actual collider shape
        
        float3 wallPos = wallTransform.Position;
        float3 wallScale = wallTransform.Scale;
        
        // Simple box collision check (assuming wall is a box)
        float3 closest = math.clamp(bulletPos, wallPos - wallScale * 0.5f, wallPos + wallScale * 0.5f);
        float3 direction = bulletPos - closest;
        float distance = math.length(direction);
        
        if (distance < bulletRadius && distance > 0.0f)
        {
            normal = math.normalize(direction);
            contactPoint = closest;
            return true;
        }
        
        normal = float3.zero;
        contactPoint = float3.zero;
        return false;
    }
}