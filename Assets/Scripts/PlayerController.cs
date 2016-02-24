using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

/// <summary>
/// Controls functionality and settings of Player character apart from movement and input handling, which is described in PlayerMovement.cs script.
/// </summary>
public class PlayerController : MonoBehaviour, IKillableObject
{
    // Gun Control script
    private GunControl gunControl;
    // Player Movement script
    private PlayerMovement playerMovement;

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

    // UI Elements
    public Text UI_HP;
    public Text UI_Ammo;

    // Use this for initialization
    void Start()
    {
        gunControl = gameObject.GetComponent<GunControl>();
        if (gunControl == null)
            Debug.LogError("No GunControl script on Player!");
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("No PlayerMovement script on Player!");

        // Equip default pistol
        gunControl.Equip("Pistol");

        IsSpawned = true;

        // Set HP
        CurrentHP = MaxHP;
    }

    /// <summary>
    /// Handles left mouse button click actions depending on arguments passed.
    /// </summary>
    /// <param name="buildMode">True if BuildController.State is not CLOSED, false if it is CLOSED</param>
    public void HandleLeftMouseButtonClick(bool buildMode)
    {
        // If not currently in building mode then left click makes player shoot
        if (buildMode == false)
            gunControl.PlayerShoot(transform, Input.mousePosition);
    }

    /// <summary>
    /// Make the enemy receive damage equal to /dmg/ from any source and handle it. Returns 0 on success, -1 when failed to receive damage.
    /// </summary>
    /// <param name="dmg"></param>
    public void OnReceiveDamage(int dmg)
    {
        Debug.Log("Player " + name + " got hit for " + dmg + " damage!");

        // Apply damage directly to currentHP
        CurrentHP -= dmg;

        // Update UI HP bar
        UpdateUIHP(CurrentHP, MaxHP);
    }

    public void Dead()
    {
        if (IsAlive) return;
        Destroy(this);
        GameManager.Instance.GameOver();
    }

    public void OnStartReload()
    {
        Debug.Log("Reloading!");
        UpdateUIAmmo(reloading: true);
    }

    public void OnEndReload()
    {
        UpdateUIAmmo(gunControl.CurrentAmmo, gunControl.MaxAmmo);
    }

    public void UpdateUIAmmo(int currentAmmo, int maxAmmo)
    {
        UI_Ammo.text = "Ammo: " + currentAmmo + "/" + maxAmmo;
    }

    private void UpdateUIAmmo(bool reloading)
    {
        UI_Ammo.text = "Ammo: Reloading...";
    }

    private void UpdateUIHP(int currentHP, int maxHP)
    {
        UI_HP.text = "HP: " + currentHP + "/" + maxHP;
    }
}
