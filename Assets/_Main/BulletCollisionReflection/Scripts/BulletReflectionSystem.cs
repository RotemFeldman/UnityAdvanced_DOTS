using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BulletCollisionSystem))]
[UpdateBefore(typeof(BulletMovementSystem))]
public partial class BulletReflectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref LocalTransform transform, ref BulletComponent bullet, 
                     ref BulletCollisionComponent collision, in ReflectionComponent reflection) =>
            {
                if (!collision.hasCollided) return;
                
                // Calculate reflected velocity using: R = V - 2(V·N)N
                float dotProduct = math.dot(bullet.velocity, collision.collisionNormal);
                float3 reflectedVelocity = bullet.velocity - 2.0f * dotProduct * collision.collisionNormal;
                
                // Apply damping
                reflectedVelocity *= reflection.reflectionDamping;
                
                // Update bullet properties
                bullet.velocity = reflectedVelocity;
                bullet.speed = math.length(reflectedVelocity);
                bullet.bounceCount++;
                
                // Move bullet slightly away from collision point to prevent sticking
                transform.Position = collision.collisionPoint + collision.collisionNormal * 0.1f;
                
            }).ScheduleParallel();
    }
}