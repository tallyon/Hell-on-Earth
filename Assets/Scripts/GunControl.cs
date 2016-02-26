using Assets.Scripts;
using UnityEngine;

/// <summary>
/// Controls the functionality and settings of a weapon attached to the character.
/// </summary>
public class GunControl : MonoBehaviour
{
    #region Data fields and properties
    // Layer masks set to ignore objects when shooting
    [SerializeField]
    private LayerMask shotLayerMask;
    // Mask of floor tiles
    [SerializeField]
    private LayerMask floorTileMask;

    //  Debug shot material
    [SerializeField]
    private Material shotMaterial = null;

    // Current gun
    private Weapon gun;

    //  Gun, current and max ammo, damage of current gun
    public string currentGun
    {
        get { return gun.Name; }
    }
    public int GunId
    {
        get { return gun.ID; }
    }
    public float ReloadTime
    {
        get { return gun.ReloadTime; }
    }
    public int MaxAmmo
    {
        get { return gun.MaxAmmo; }
    }
    public int CurrentAmmo
    {
        get { return gun.CurrentAmmo; }
    }
    public int GunDmg
    {
        get { return gun.Damage; }
    }
    public float GunRange
    {
        get { return gun.AttackRange; }
        set { gun.AttackRange = value; }
    }


    //  Reload
    private bool reloading;
    public bool Reloading
    {
        get { return reloading; }
        protected set { reloading = value; }
    }
    // Holds GameManager
    private GameManager gameManager;
    // Holds PlayerScript
    private PlayerController playerController;

    // Is this script attached on Player
    private bool isPlayer;
    #endregion

    void Start()
    {
        // Get GameManager from the singleton
        gameManager = GameManager.Instance;

        // Get PlayerController
        playerController = gameObject.GetComponent<PlayerController>();
        // If PlayerController was not found on the game object then it's not Player object
        if (playerController != null) isPlayer = true;
        else
        {
            isPlayer = false;
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    /// <summary>
    /// Player's shoot function.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="targetPos"></param>
    /// <returns>1 if successfully shot, -1 if unable to shoot</returns>
    public int PlayerShoot(Transform origin, Vector3 mousePos)
    {
        // Cannot shoot whilst reloading weapon
        if (Reloading)
            return -1;

        Vector3 hitPoint;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // Raycast to floor tile
        RaycastHit[] floorHit = Physics.RaycastAll(ray, Mathf.Infinity, floorTileMask);
        foreach (RaycastHit rayHit in floorHit)
        {
            hitPoint = rayHit.point;

            // Compute point X, Y, Z, where Y is player's position.y and X, Z are computed using Cramer's method
            float targetY = transform.position.y;
            Vector3 targetDirection = ComputeShotDirectionForPlayer(targetY, ray.origin, hitPoint);

            // Shoot
            RaycastHit hit;

            // Raycast the actual shot from player, through point under mouse click with height of player, hits only shotLayerMask
            if (Physics.Raycast(origin.position, Vector3.Normalize(targetDirection - origin.position), out hit, 100, shotLayerMask))
            {
                // Call OnReceiveDamage on hit object
                hit.transform.gameObject.GetComponent<IKillableObject>().OnReceiveDamage(gun.Damage);
                SpawnShot(origin.position, hit.point);
            }
            else
            {
                // If enemy was not hit then normalize vector between origin and target
                Vector3 normal = Vector3.Normalize(targetDirection - origin.transform.position);
                // scale it up
                normal.Scale(new Vector3(100, 0, 100));
                // spawn shot
                SpawnShot(origin.position, origin.position + normal);
            }

            // Decrement ammo by 1
            int prevAmmo = gun.CurrentAmmo;
            gun.CurrentAmmo--;
            // DEBUG
            Debug.Log(Time.frameCount + ": Decrementing ammo from " + prevAmmo + " to " + gun.CurrentAmmo);

            // If player update UI
            if (isPlayer)
                origin.GetComponent<PlayerController>().UpdateUIAmmo(CurrentAmmo, MaxAmmo);

            // If currentAmmo <= 0 initiate reload - cooldown on shooting
            if (gun.CurrentAmmo <= 0)
            {
                Reload(gun.ReloadTime);
            }
        }

        return 1;
    }

    /// <summary>
    /// Perform shot from /origin/ point in space through /targetPos/ position.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="targetPos"></param>
    /// <returns>1 if successfully shot, -1 if unable to shoot</returns>
    public int Shoot(Transform origin, Vector3 targetPos)
    {
        // Cannot shoot whilst reloading weapon
        if (Reloading)
            return 0;

        Vector3 normalizedDirection = Vector3.Normalize(targetPos - origin.position);
        RaycastHit hit;
        bool targetHit = false;

        // If hit, decide what to do - enemy takes the hit from player, player takes the hit from enemy
        if (Physics.Raycast(origin.position, normalizedDirection, out hit, GunRange, shotLayerMask))
        {
            // If target is not yet spawned then it is not possible to shoot it
            IKillableObject objectHit = hit.transform.gameObject.GetComponent<IKillableObject>();
            if (objectHit.IsSpawned)
            {
                Debug.Log("Trafiono: " + hit.transform.name);
                hit.transform.gameObject.GetComponent<IKillableObject>().OnReceiveDamage(gun.Damage);
                targetHit = true;
            }
            else
            {
                return -1;
            }
        }
        else
            return -2;

        //  Spawn Line Renderer to obtain this cool effect of briefly showing trajectories of bullets
        if (targetHit == false)
        {
            // If enemy was not hit then normalize vector between origin and target
            Vector3 normal = Vector3.Normalize(targetPos - origin.position);
            // scale it up
            normal.Scale(new Vector3(100, 100, 100));
            normalizedDirection.Scale(new Vector3(100, 100, 100));
            // spawn shot
            SpawnShot(origin.position, origin.position + normalizedDirection);
        }
        else
            SpawnShot(origin.position, hit.point);

        // Decrement ammo by 1
        gun.CurrentAmmo--;

        // If currentAmmo <= 0 initiate reload - cooldown on shooting
        if (gun.CurrentAmmo <= 0)
        {
            Reload(gun.ReloadTime);
        }

        return 1;
    }

    /// <summary>
    /// Equip weapon identified by /weaponName/.
    /// </summary>
    /// <param name="weaponName"></param>
    public void Equip(string weaponName)
    {
        // Search through gunNames array to find matching gun name and retrieve index of the gun
        int weaponIndex = -1;
        if (gameManager.WeaponsList == null)
        {
            return;
        }

        for (int i = 0; i < gameManager.WeaponsList.Count; i++)
        {
            if (weaponName == gameManager.WeaponsList[i].Name)
            {
                weaponIndex = i;
                break;
            }
        }

        // Equip Weapon object
        gun = gameManager.WeaponsList[weaponIndex].Equip();

        // If this is Player Equiping update UI
        /*if (isPlayer)
        {
            //gameScript.UpdateGunText(gun.Name);
            gameScript.UpdateUI();
        }*/

        // DEBUG
        //Debug.Log(transform.name + " equipped " + gun.Name + "(" + gun.ID + ")");
    }

    public void Reload(float time)
    {
        // Check if reloading or currentAmmo == maxAmmo - if it's true then return, no need to reload full magazine
        if (Reloading || gun.CurrentAmmo == gun.MaxAmmo)
            return;

        Reloading = true;

        // If this is Player reloading run OnStartReload() from PlayerController
        if (isPlayer)
        {
            playerController.OnStartReload();
        }

        // Invoke EndReload function after time
        Invoke("EndReload", time);
    }

    private void EndReload()
    {
        // Refill ammo at the end of reload cooldown
        gun.CurrentAmmo = gun.MaxAmmo;

        // If this is Player ending reload run EndReload() from PlayerController
        if (isPlayer)
        {
            playerController.OnEndReload();
        }

        Reloading = false;
    }

    #region Auxiliary functions
    /// <summary>
    /// Spawn bullet trajectory originating in /origin/ point in space, going in straight line to /hitPoint/ point in space with /color/.
    /// </summary>
    /// <param name="origin">Start point</param>
    /// <param name="hitPoint">End point</param>
    /// <param name="color">Color of the line</param>
    private void SpawnShot(Vector3 origin, Vector3 hitPoint, Color color)
    {
        //  Create new game object with line renderer to show the shot
        GameObject lRend = new GameObject();
        lRend.name = "Shot";
        LineRenderer lineRenderer = lRend.AddComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, hitPoint);
        lineRenderer.material = shotMaterial;
        lineRenderer.SetColors(Color.white, Color.white);
        lineRenderer.SetWidth(.025f, .025f);

        //  Destroy line renderer after .5 sec
        Destroy(lRend, 1.5f);
    }
    /// <summary>
    /// Spawn bullet trajectory originating in /origin/ point in space, going in straight line to /hitPoint/ point in space.
    /// </summary>
    /// <param name="origin">Start point</param>
    /// <param name="hitPoint">End point</param>
    private void SpawnShot(Vector3 origin, Vector3 hitPoint)
    {
        SpawnShot(origin, hitPoint, Color.white);
    }

    /// <summary>
    /// Computes line equation given by two points /P1/ and /P2/ using Cramer's method for Y. Returns a and b by Vector2.
    /// </summary>
    /// <param name="P1">First point of the raycast to floor</param>
    /// <param name="P2">Second point of the raycast to floor</param>
    /// <returns>A and B of ax + b linear equation set by P1 and P2 points</returns>
    private Vector2 ComputeLinearEquationCramer(Vector3 P1, Vector3 P2)
    {
        // Matrix to calculate is
        // P1.x     1
        // P2.x     1
        float detMatrix = P1.x - P2.x;

        // Calculate first parameter of linear equation ax + b
        float a = (P1.y - P2.y) / detMatrix;
        // Calculate second parameter of linear equation ax + b
        float b = (P1.x * P2.y - P2.x * P1.y) / detMatrix;

        Vector2 outcome = new Vector2(a, b);
        return outcome;
    }

    /// <summary>
    /// Computes and returns shot direction for player using P1 and P2 points - camera's position and floor hit point by raycast under mouse cursor respectively.
    /// </summary>
    /// <param name="targetY"></param>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    /// <returns></returns>
    private Vector3 ComputeShotDirectionForPlayer(float targetY, Vector3 P1, Vector3 P2)
    {
        Vector2 cramerForY = ComputeLinearEquationCramer(P1, P2);
        Vector3 swapYwithZ1 = new Vector3(P1.x, P1.z, P1.y);
        Vector3 swapYwithZ2 = new Vector3(P2.x, P2.z, P2.y);
        Vector2 cramerForZ = ComputeLinearEquationCramer(swapYwithZ1, swapYwithZ2);

        // Y = cramerY.x * X + cramerY.y
        // X = (Y - cramerY.y) / cramerY.x
        float targetX = (targetY - cramerForY.y) / cramerForY.x;

        // Z = cramerZ.x * X + cramerZ.y
        float targetZ = cramerForZ.x * targetX + cramerForZ.y;

        // Combine computations into point in Vector3
        Vector3 targetDirection = new Vector3(targetX, targetY, targetZ);
        return targetDirection;
    }
    #endregion
}
