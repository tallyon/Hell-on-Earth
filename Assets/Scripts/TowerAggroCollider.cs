using UnityEngine;
using Assets.Scripts;
using System.Collections.Generic;

public class TowerAggroCollider : MonoBehaviour
{
    private List<GameObject> targetsDetected;
    [SerializeField]
    private LayerMask enemyLayerMask;
    private SphereCollider sphereCollider;
    private Vector3 colliderCenter;
    private float colliderRadius;
    private TowerController towerController;

    void Start()
    {
        towerController = transform.GetComponentInParent<TowerController>();
        sphereCollider = GetComponent<SphereCollider>();

        // Get collider dimensions and disable it
        colliderCenter = sphereCollider.transform.position + sphereCollider.center;
        colliderRadius = sphereCollider.radius;
        sphereCollider.enabled = false;
    }

    void Update()
    {
        // If there is no target on tower find one
        if (towerController.Target == null)
        {
            // Find suitable targets and store them in list of game objects
            targetsDetected = FindTargetsInsideSphere(colliderCenter, colliderRadius, enemyLayerMask);

            // Calculate closest enemy currently in the list
            GameObject closestEnemy = CalculateClosestEnemy(targetsDetected);

            // Set target of TowerController to closestEnemy
            if (closestEnemy != null)
                towerController.Target = closestEnemy;
        }
    }

    /// <summary>
    /// Traverses /enemiesDetected/ list of GameObjects to find nearest one and returns it as GameObject.
    /// </summary>
    /// <param name="targetsDetected">List of GameObjects holding enemies that are in sight.</param>
    /// <returns>Closest enemy in sight as GameObject.</returns>
    private GameObject CalculateClosestEnemy(List<GameObject> targetsDetected)
    {
        // If list of enemies is empty return null
        if (targetsDetected == null || targetsDetected.Count == 0)
            return null;

        GameObject closestEnemy = targetsDetected[0];
        float smallestDistance = Vector3.Distance(closestEnemy.transform.position, transform.position);

        for (int i = 1; i < targetsDetected.Count; i++)
        {
            // If enemy is not spawned yet ignore it
            if (targetsDetected[i].GetComponent<IKillableObject>() == null || targetsDetected[i].GetComponent<IKillableObject>().IsSpawned == false)
                continue;
            else
            {
                // Calculate enemy's distance to tower and if it's the closest enemy so far store it in closestEnemy
                float dist = Vector3.Distance(targetsDetected[i].transform.position, transform.position);
                if (dist < smallestDistance)
                {
                    smallestDistance = dist;
                    closestEnemy = targetsDetected[i];
                }
            }
        }

        return closestEnemy;
    }

    /// <summary>
    /// Finds all suitable targets defined by layer /enemyMask/ and return them in list of game objects
    /// </summary>
    /// <param name="position">Position of sphere's center</param>
    /// <param name="radius">Radius of the sphere</param>
    /// <param name="enemyMask">LayerMask containing all valid targets for the tower</param>
    /// <returns>List of GameObjects of suitable targets</returns>
    private List<GameObject> FindTargetsInsideSphere(Vector3 position, float radius, LayerMask enemyMask)
    {
        Collider[] targets = Physics.OverlapSphere(position, radius, enemyMask);
        if (targets.Length > 0)
        {
            // Insert all found targets in the list and return it
            List<GameObject> targetsList = new List<GameObject>(targets.Length);
            foreach (Collider target in targets)
                targetsList.Add(target.gameObject);

            return targetsList;
        }
        else
            return null;
    }
}
