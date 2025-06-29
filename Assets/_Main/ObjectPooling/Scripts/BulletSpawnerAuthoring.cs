using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BulletSpawnerAuthoring : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float bulletSpeed = 10f;
    public float bulletLifetime = 10f;
    public int maxBounces = 5;
    public float bulletRadius = 0.1f;
    public float reflectionDamping = 0.9f;
    
    [Header("Visual")]
    public GameObject bulletVisualPrefab;
    
    [Header("Spawning")]
    public KeyCode spawnKey = KeyCode.Space;
    public Transform[] spawnPoints;
    
    private EntityManager entityManager;
    
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnBullet();
        }
    }
    
    void SpawnBullet()
    {
        if (spawnPoints.Length == 0 || bulletVisualPrefab == null) return;
        
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        float3 randomDirection = new float3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        );
        randomDirection = math.normalize(randomDirection);
        
        // Create bullet entity
        Entity bullet = entityManager.CreateEntity(
            typeof(LocalTransform),
            typeof(BulletComponent),
            typeof(BulletCollisionComponent),
            typeof(ReflectionComponent),
            typeof(BulletVisualGameObject)
        );
        
        // Set position and components
        entityManager.SetComponentData(bullet, LocalTransform.FromPosition(spawnPoint.position));
        
        entityManager.SetComponentData(bullet, new BulletComponent
        {
            velocity = randomDirection * bulletSpeed,
            speed = bulletSpeed,
            lifetime = bulletLifetime,
            maxLifetime = bulletLifetime,
            bounceCount = 0,
            maxBounces = maxBounces
        });
        
        entityManager.SetComponentData(bullet, new BulletCollisionComponent
        {
            radius = bulletRadius,
            hasCollided = false,
            collisionNormal = float3.zero,
            collisionPoint = float3.zero,
            collidedWith = Entity.Null
        });
        
        entityManager.SetComponentData(bullet, new ReflectionComponent
        {
            reflectionDamping = reflectionDamping,
            canReflectOffBullets = true,
            canReflectOffWalls = true
        });
        
        // Create and link visual GameObject
        GameObject visualGO = Instantiate(bulletVisualPrefab, spawnPoint.position, Quaternion.identity);
        entityManager.SetComponentData(bullet, new BulletVisualGameObject 
        { 
            visualGameObject = visualGO 
        });
    }
}