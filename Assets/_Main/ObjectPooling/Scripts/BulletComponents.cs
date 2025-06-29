using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BulletComponent : IComponentData
{
    public float3 velocity;
    public float speed;
    public float lifetime;
    public float maxLifetime;
    public int bounceCount;
    public int maxBounces;
}

public struct BulletCollisionComponent : IComponentData
{
    public bool hasCollided;
    public float3 collisionNormal;
    public float3 collisionPoint;
    public Entity collidedWith;
    public float radius; // Keep radius for custom collision detection
}

public struct WallTag : IComponentData
{
    // Tag component for walls
}

public struct ReflectionComponent : IComponentData
{
    public float reflectionDamping;
    public bool canReflectOffBullets;
    public bool canReflectOffWalls;
}

public class BulletVisualGameObject : IComponentData
{
    public GameObject visualGameObject;
}