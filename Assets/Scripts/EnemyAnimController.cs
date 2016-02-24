using UnityEngine;

public class EnemyAnimController : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyScript;

    public void SpawnComplete()
    {
        enemyScript.SpawnComplete();
    }
}
