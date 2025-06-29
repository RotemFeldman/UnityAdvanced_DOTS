using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class BulletVisualUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Update visual GameObject positions to match entity positions
        Entities
            .WithoutBurst() // Required because we're accessing GameObjects
            .ForEach((Entity entity, in LocalTransform transform, in BulletComponent bullet) =>
            {
                if (EntityManager.HasComponent<BulletVisualGameObject>(entity))
                {
                    var visualComp = EntityManager.GetComponentData<BulletVisualGameObject>(entity);
                    if (visualComp.visualGameObject != null)
                    {
                        // Update position
                        visualComp.visualGameObject.transform.position = transform.Position;
                        
                        // Optional: Rotate to face movement direction
                        if (math.lengthsq(bullet.velocity) > 0.001f)
                        {
                            visualComp.visualGameObject.transform.rotation = 
                                Quaternion.LookRotation(bullet.velocity, Vector3.up);
                        }
                    }
                }
            }).Run();
    }
}