using Unity.Entities;
using UnityEngine;

namespace _Scripts
{
    public class EntityColorAuthoring : MonoBehaviour
    {
        [Header("Params")] 
        public float min;
        public float max;
        
        class Baker : Baker<EntityColorAuthoring>
        {
            public override void Bake(EntityColorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ChangeColor
                {
                    timeToChange = Random.Range(authoring.min, authoring.max),
                    timeSinceLastChange = 0f
                });
            }
        }
    }
}