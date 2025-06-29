using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BulletReflectionSystem))]
public partial class BulletMovementSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    
    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        
        // Clean up visual GameObjects before destroying entities
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in BulletComponent bullet) =>
            {
                if (bullet.lifetime <= 0.0f || bullet.bounceCount >= bullet.maxBounces)
                {
                    // Destroy visual GameObject
                    if (EntityManager.HasComponent<BulletVisualGameObject>(entity))
                    {
                        var visualComp = EntityManager.GetComponentData<BulletVisualGameObject>(entity);
                        if (visualComp.visualGameObject != null)
                        {
                            GameObject.Destroy(visualComp.visualGameObject);
                        }
                    }
                }
            }).Run();
        
        // Move bullets and update lifetime
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref LocalTransform transform, ref BulletComponent bullet) =>
            {
                // Move bullet
                transform.Position += bullet.velocity * deltaTime;
                bullet.lifetime -= deltaTime;
                
                // Mark for destruction if expired
                if (bullet.lifetime <= 0.0f || bullet.bounceCount >= bullet.maxBounces)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        // Reset collision flags AFTER reflection has been processed
        Entities
            .ForEach((ref BulletCollisionComponent collision) =>
            {
                collision.hasCollided = false;
            }).ScheduleParallel();
            
        m_EndSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}