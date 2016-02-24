using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class TowerController : MonoBehaviour, IKillableObject
{
    #region Data fields and properties
    private GunControl gunControl;
    [SerializeField]
    private GameObject HPUI_prefab;
    private GameObject HPUI;
    [SerializeField]
    // Position of HPBar, set by empty game object under the Tower game object
    private Transform HPBar_pos = null;
    private Text HPBar_text = null;

    // HP and alive check
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
    public bool IsSpawned { get; set; }

    // Current target
    public GameObject Target { get; set; }
    #endregion

    // Use this for initialization
    void Start()
    {
        gunControl = gameObject.GetComponent<GunControl>();
        Target = null;
        IsSpawned = true;

        // Instantiate HP Bar and make it a child of Canvas
        HPUI = Instantiate(HPUI_prefab);
        HPUI.transform.SetParent(FindObjectOfType<Canvas>().transform);
        HPUI.name = "ID_" + name + "_HP";
        HPBar_text = HPUI.transform.FindChild("HPValue").GetComponent<Text>();

        // Equip default tower weapon
        gunControl.Equip("TowerArrow");

        // Set HP
        CurrentHP = MaxHP;

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        // If there is Target to shoot set him as current target
        if (Target != null)
            Hit(Target);

        // Set position of a HP Bar above the tower
        HPUI.transform.position = Camera.main.WorldToScreenPoint(HPBar_pos.position);
    }

    /// <summary>
    /// Tower recieves damage equal to /dmg/ from any source and handles consequences. Returns 0 on success, -1 when failed to receive damage.
    /// </summary>
    /// <param name="dmg"></param>
    /// <returns></returns>
    public void OnReceiveDamage(int dmg)
    {
        Debug.Log("Tower " + name + " hit for " + dmg + " damage.");

        CurrentHP -= dmg;
        UpdateUI();
    }

    public void Dead()
    {
        if (IsAlive) return;
        // Destroy HPBar in UI
        Destroy(HPUI);
        // Destroy enemy on hit
        Destroy(gameObject);
    }

    /// <summary>
    /// Calls IEntityController.ReceiveDamage() on /target/. Returns 0 if successfull, -1 when failed.
    /// </summary>
    /// <param name="target"></param>
    private void Hit(GameObject target)
    {
        // Shoot from turret's position to target's position
        gunControl.Shoot(transform, target.transform.position);
    }

    /// <summary>
    /// Updates all UI elements related to the enemy object
    /// </summary>
    private void UpdateUI()
    {
        HPBar_text.text = CurrentHP + "/" + MaxHP;
    }
}
