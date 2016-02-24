/// <summary>
/// Class containing weapon specific data fields and functions
/// </summary>
public class Weapon : Item
{
    #region Data fields and properties
    // Data fields
    private int damage;
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    private int maxAmmo;
    public int MaxAmmo
    {
        get { return maxAmmo; }
        set { maxAmmo = value; }
    }

    private int currentAmmo;
    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set { currentAmmo = value; }
    }

    private float reloadTime;
    public float ReloadTime
    {
        get { return reloadTime; }
        set { reloadTime = value; }
    }

    private float attackRange;
    public float AttackRange
    {
        get { return attackRange; }
        set { attackRange = value; }
    }

    private bool melee;     // If true - melee weapon with no ammo and reload functionality
    public bool IsMelee
    {
        get { return melee; }
        set { melee = value; }
    }
    #endregion

    // Constructor
    /// <summary>
    /// Ranged weapon constructor
    /// </summary>
    /// <param name="_damage"></param>
    /// <param name="_maxAmmo"></param>
    /// <param name="_reloadTime"></param>
    /// <param name="_attackRange"></param>
    public Weapon(int _id, string _name, int _damage, int _maxAmmo, float _reloadTime, float _attackRange)
    {
        ID = _id;
        Name = _name;
        Damage = _damage;
        MaxAmmo = _maxAmmo;
        CurrentAmmo = MaxAmmo;
        ReloadTime = _reloadTime;
        AttackRange = _attackRange;
        IsMelee = false;
    }

    /// <summary>
    /// Melee weapon constructor
    /// </summary>
    /// <param name="_damage"></param>
    public Weapon(int _id, string _name, int _damage, float _reloadTime)
    {
        ID = _id;
        Name = _name;
        Damage = _damage;
        MaxAmmo = 1;
        CurrentAmmo = MaxAmmo;
        ReloadTime = _reloadTime;
        // Default melee attack range is 2
        AttackRange = 2;
        IsMelee = true;
    }

    // Empty constructor
    public Weapon() { }

    /// <summary>
    /// Equip this weapon by cloning this object
    /// </summary>
    /// <returns></returns>
    public Weapon Equip()
    {
        Weapon newWeapon = new Weapon();
        newWeapon = MemberwiseClone() as Weapon;
        newWeapon.Name = Name;
        return newWeapon;
    }
}
