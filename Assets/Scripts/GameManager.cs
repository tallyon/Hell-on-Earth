using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;

/// <summary>
/// Master script for current level. Holds information about certain game objects in lists and register them on instantiation.
/// </summary>
public class GameManager : MonoBehaviour, IWeaponList, IBuildingsList
{
    #region Data fields and properties
    // Singleton instance
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
        protected set
        {
            instance = value;
        }
    }

    // Player game object with property to protect setting
    private GameObject player;
    public GameObject Player
    {
        get
        {
            if (player == null)
                player = GameObject.Find("Player");

            return player;
        }
        protected set
        {
            player = value;
        }
    }

    // Default melee weapon object
    private Weapon melee_01 = new Weapon(0, "Claw", 5, 2);
    // Default ranged weapon object
    private Weapon ranged_01 = new Weapon(1, "Pistol", 15, 10, 2, 10);
    // Default tower weapon object
    private Weapon tower_01 = new Weapon(2, "TowerArrow", 10, 1, 2, 50);

    // List of all weapon objects
    public List<Weapon> WeaponsList { get; set; }
    // List of all building objects
    public List<Building> BuildingsList { get; set; }
    // List of all buildings in scene
    private List<BuildingInScene> ListBuildingsInScene;

    [SerializeField]
    private GameObject[] buildingsPrefabs = null, buildingsSilhouettes = null;
    #endregion

    void Start()
    {
        // Set static instance to this GameManager
        instance = this;

        ListBuildingsInScene = new List<BuildingInScene>();
        WeaponsList = new List<Weapon>();
        BuildingsList = new List<Building>();

        // Add default weapons to the weapons list
        RegisterWeapon(melee_01);
        RegisterWeapon(ranged_01);
        RegisterWeapon(tower_01);

        // Add buildings selected in editor to the buildings list
        for (int i = 0; i < buildingsPrefabs.Length; i++)
        {
            // If there are no more building prefabs or there are less building silhouettes stop registering buildings
            if (buildingsPrefabs[i] == null || buildingsSilhouettes == null)
            {
                Debug.LogError("Error when populating buildings list! Check Buildings Prefabs and Buildings Silhouettes in Game Manager script!");
                break;
            }

            Building newBuilding = new Building(BuildingsList.Count, 10, buildingsPrefabs[i], buildingsSilhouettes[i]);
            RegisterBuilding(newBuilding);
        }

        Player = GameObject.Find("Player");
    }

    // Private constructor for singleton class
    private GameManager() { }

    /// <summary>
    /// Register /building/ Building object in the list of buildings in BuildingsList
    /// </summary>
    /// <param name="building">Building object</param>
    /// <returns></returns>
    public int RegisterBuilding(Building building)
    {
        // Add new building to the list
        BuildingsList.Add(building);

        // Return index of building added
        return BuildingsList.Count - 1;
    }

    /// <summary>
    /// Creates new object that maps the Transform /transform/ of the building to the /owner/ ID
    /// </summary>
    /// <param name="owner">ID of the owner of the building</param>
    /// <param name="transform">Transform of the building</param>
    /// <returns></returns>
    public int RegisterBuildingInScene(int owner, Transform transform)
    {
        // Create new building record
        BuildingInScene newBuilding = new BuildingInScene();
        newBuilding.Index = ListBuildingsInScene.Count;
        newBuilding.Owner = owner;
        newBuilding.Transform = transform;

        // Add new building to the list
        ListBuildingsInScene.Add(newBuilding);

        return newBuilding.Index;
    }

    /// <summary>
    /// Register /weapon/ Weapon object in the list of weapons in WeaponsList
    /// </summary>
    /// <param name="weapon">Weapons object</param>
    /// <returns></returns>
    public int RegisterWeapon(Weapon weapon)
    {
        // Add new weapon to the list
        WeaponsList.Add(weapon);

        // Return index of weapon added
        return WeaponsList.Count - 1;
    }

    public void GameOver()
    {
        Debug.LogError("Game over!");
    }
    
    // Building that is built in scene structure
    private struct BuildingInScene
    {
        public int Index;
        // ID to define the building's owner
        public int Owner;
        // transform of the building
        public Transform Transform;
    }
}
