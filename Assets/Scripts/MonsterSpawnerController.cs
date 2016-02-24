using UnityEngine;

public class MonsterSpawnerController : MonoBehaviour
{
    [SerializeField]
    private GameObject monsterTemplate;
    private Transform monstersHierarchy;
    [SerializeField]
    private BoxCollider spawnCheckCollider;
    [SerializeField]
    private LayerMask collisionMask;
    private Vector3 colliderCenter, colliderHalfExtents;
    private float lastSpawnTime = 0;
    [SerializeField]
    private float spawnInterval;
    private bool IsSpawnQueued;

    void Start()
    {
        monstersHierarchy = GameObject.Find("Monsters").GetComponent<Transform>();

        // Get collider dimensions and disable it
        colliderCenter = spawnCheckCollider.transform.position + spawnCheckCollider.center;
        colliderHalfExtents = spawnCheckCollider.bounds.extents / 2;
        spawnCheckCollider.enabled = false;
    }

    void Update()
    {
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            IsSpawnQueued = true;
            lastSpawnTime = Time.time;
        }

        if (IsSpawnQueued)
            SpawnMonster();
    }

    public void SpawnMonster()
    {
        // Check if there is no collision on spawn collider
        Collider[] cols = Physics.OverlapBox(colliderCenter, colliderHalfExtents, Quaternion.identity, collisionMask);

        // If there is collision with spawner return
        if (cols.Length > 0)
        {
            // DEBUG
            //Debug.Log("Can't spawn " + cols[0].name + " in the way");
            return;
        }

        // Perform overlapbox check like above for the area around the set collider:
        //        X3
        //     X1 O X2
        //        X4
        // O - starting area to check Physics.OverlapBox(colliderCenter, colliderHalfExtents)
        // X1 - next position to check Physics.OverlapBox(colliderCenter + new Vector3(-colliderHalfExtents.x * 2, 0, 0), colliderHalfExtents)
        // X2 - next position to check Physics.OverlapBox(colliderCenter + new Vector3(colliderHalfExtents.x * 2, 0, 0), colliderHalfExtents)
        // and so on... only after checking proper area and area surrounding spawner give up and check in next frame
        // There also has to be collision check with ground below if ther is space to spawn the enemy

        GameObject spawnedMonster = Instantiate(monsterTemplate, transform.position, transform.rotation) as GameObject;
        spawnedMonster.name = monsterTemplate.name + "_" + Time.frameCount;
        spawnedMonster.transform.SetParent(monstersHierarchy);

        IsSpawnQueued = false;
    }
}
