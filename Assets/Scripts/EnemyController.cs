using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

/// <summary>
/// Controls the basic behaviour of the enemy.
/// </summary>
public class EnemyController : MonoBehaviour, IKillableObject
{
    #region Data fields and properties
    private GunControl gunControl;
    private NavMeshAgent navAgent;
    public bool IsSpawned { get; set; }

    // Animator
    [SerializeField]
    private Animator anim;
    private string[] attackAnimations = { "Attack1", "Attack2", "Attack3" };

    [SerializeField]
    private GameObject HPUI_prefab;
    private GameObject HPUI;
    [SerializeField]
    // Position of HPBar, set by empty game object under the Enemy game object
    private Transform HPBar_pos = null;
    private Text HPBar_text = null;

    // Health
    private int currentHP;
    public int CurrentHP
    {
        get { return currentHP; }
        // Call Dead() when HP goes below 0
        set
        {
            if (value > 0)
                currentHP = value;
            else
            {
                currentHP = 0;
                Dead();
            }
        }
    }

    [SerializeField]
    private int maxHP;
    public int MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }
    public bool IsAlive
    {
        get { return CurrentHP > 0; }
        set { return; }
    }

    // Attack range
    [SerializeField]
    private float attackRange;

    // Target
    [SerializeField]
    private GameObject target = null;
    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }
    #endregion

    void Start()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        navAgent.stoppingDistance = attackRange;
        gunControl = gameObject.GetComponent<GunControl>();

        // Disable rigidbody
        GetComponent<Rigidbody>().Sleep();

        // Instantiate HP Bar and make it a child of Canvas
        HPUI = Instantiate(HPUI_prefab);
        HPUI.transform.SetParent(FindObjectOfType<Canvas>().transform);
        HPUI.name = "ID_" + name + "_HP";
        HPBar_text = HPUI.transform.FindChild("HPValue").GetComponent<Text>();
        HPUI.transform.position = Camera.main.WorldToScreenPoint(HPBar_pos.position);

        // Equip default melee weapon
        gunControl.Equip("Claw");
        gunControl.GunRange = attackRange;
        
        // Początkowa animacja spawnu zapewnia potworowi nieśmiertelność za pomocą flagi IsSpawned ustawionej na false
        IsSpawned = false;

        // Set HP
        CurrentHP = MaxHP;

        UpdateUI();
    }

    void Update()
    {
        // Update Navigation
        if (IsSpawned)
        {
            // If there is some target set head for him
            if (Target != null)
            {
                // If target is in range then attack it
                if (CheckIfTargetIsInRange(Target, attackRange))
                {
                    StopNavigationMovement();
                    Hit(Target);
                }
                else
                {
                    // Set nav agent destination to target's position
                    navAgent.Resume();
                    navAgent.SetDestination(Target.transform.position);
                }
            }
            else
            {
                Vector3 playerPos = GameManager.Instance.Player.transform.position;
                // If there was no target set then go towards the enemy but continue to constantly check for valid target (achieved by having Target == null)
                navAgent.Resume();
                navAgent.SetDestination(playerPos);
                Target = GameManager.Instance.Player;
            }
        }

        // Set position of a HP Bar above the enemy
        HPUI.transform.position = Camera.main.WorldToScreenPoint(HPBar_pos.position);
    }

    /// <summary>
    /// Set attack range of the enemy to /value/.
    /// </summary>
    /// <param name="value">Attack range</param>
    private void SetAttackRange(float value)
    {
        attackRange = value;
        navAgent.stoppingDistance = attackRange;
    }

    /// <summary>
    /// Make the enemy receive damage equal to /dmg/ from any source and handle it. Returns 0 on success, -1 when failed to receive damage.
    /// </summary>
    /// <param name="dmg"></param>
    public void OnReceiveDamage(int dmg)
    {
        // If enemy is not spawned it can't be damaged
        if (!IsSpawned)
            return;

        // DEBUG
        //Debug.Log("Enemy " + name + " hit for " + dmg + " damage.");
        CurrentHP -= dmg;

        // Fire animation trigger
        anim.SetTrigger("GotHit");

        UpdateUI();
    }

    public void Dead()
    {
        // Make sure that current hp > 0
        if (IsAlive) return;

        // DEBUG
        Debug.Log(name + " died!");
        // Destroy HP in UI
        Destroy(HPUI);
        // Destroy enemy on hit
        Destroy(gameObject);
    }

    private void StopNavigationMovement()
    {
        navAgent.Stop();
        // Set animation bool Walk to false
        anim.SetBool("Walk", false);
    }

    /// <summary>
    /// Calls IEntityController.ReceiveDamage() on /target/.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private void Hit(GameObject target)
    {
        // Shoot from Enemy's position to target's position
        int shotSuccessful = gunControl.Shoot(transform, Target.transform.position + new Vector3(0, GetComponent<CapsuleCollider>().bounds.extents.y, 0));

        // Fire animation
        if (shotSuccessful == 1)
            anim.SetTrigger(attackAnimations[Random.Range(0, attackAnimations.Length)]);
    }

    /// <summary>
    /// Check if /target/ is within /attackRange/ distance from the Enemy, if so return true, otherwise return false.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackRange"></param>
    /// <returns></returns>
    private bool CheckIfTargetIsInRange(GameObject target, float attackRange)
    {
        Vector3 raycastDestination = target.transform.position - transform.position + new Vector3(0, GetComponent<CapsuleCollider>().bounds.extents.y, 0);
        bool inRange = Physics.Raycast(transform.position, raycastDestination, attackRange, 1 << target.layer);

        if (inRange)
        {
            // Target is in range - set animation boolean so it stops walking animation
            StopNavigationMovement();
            return true;
        }
        else
        {
            // Target is not in range - set animation boolean so it continues the walking animation
            anim.SetBool("Walk", true);
            return false;
        }
    }

    /// <summary>
    /// Updates all UI elements related to the enemy object
    /// </summary>
    private void UpdateUI()
    {
        HPBar_text.text = CurrentHP + "/" + MaxHP;
    }

    private void OnCollisionEnter(Collision col)
    {
        // Collision with Player
        if (col.gameObject.tag == "Player")
        {
            //Debug.Log(name + " collided with Player!");
        }

        // Collision with Building
        else if (col.gameObject.tag == "Building")
        {
            //Debug.Log(name + " collided with Building " + col.gameObject.name);
        }
    }

    public void SpawnComplete()
    {
        // DEBUG
        //Debug.Log("Spawn complete");
        IsSpawned = true;
    }
}
