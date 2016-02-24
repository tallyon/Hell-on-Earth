using UnityEngine;

public class BuildController : MonoBehaviour
{
    #region Data fields and properties
    // State of a build controller
    public enum State { CLOSED, OPEN, BUILDING_SELECTED };
    public State state;

    // FloorHighlight prefab
    public GameObject prefab_FloorHighlight;

    // Instantiated building
    private GameObject building_silhouette;
    // Selected building
    private Building selectedBuilding;

    // Instantiated floor highlit
    private GameObject floorHighlight;

    // RaycastController
    private RaycastController raycastController;

    // GameManager
    public GameManager gameManager;

    // Building UI Panel
    public GameObject buildingUIPanel;

    // Materials for floor tile highlights
    public Material greenBuild, redBuild;
    #endregion

    // Use this for initialization
    void Start()
    {
        raycastController = gameObject.GetComponent<RaycastController>();
        state = State.CLOSED;
    }

    void Update()
    {
        // If there is building selected to build, set it's silhouette position accordingly to currently highlited floor tile
        if (state == State.BUILDING_SELECTED)
        {
            // Check if building can be built on floor tile under the cursor
            HighlightFloor(Input.mousePosition);

            building_silhouette.transform.position = floorHighlight.transform.position;

            // Check for left click when in state BUILDING_SELECTED
            if (Input.GetMouseButtonDown(0))
            {
                Build();
            }
        }
    }

    // Click floor tile
    public void HighlightFloor(Vector3 mousePos)
    {
        // If BuildController is in state CLOSED - return
        if (state == State.CLOSED)
            return;

        // Raycast to floor tile, if got null then exit the function
        GameObject floorTileHit = raycastController.RaycastToFloorTile(mousePos);
        if (floorTileHit == null)
            return;

        // Instantiate FloorHighlight game object where the floor tile hit by ray is
        floorHighlight.transform.position = floorTileHit.transform.position;
        floorHighlight.transform.Translate(0, .1f, 0);

        // Decide if the floor tile selected by cursor is suitable to be built upon - change color to green if so, red otherwise
        bool canBuildOnTile = true;

        canBuildOnTile = !floorHighlight.GetComponent<FloorHighlightColliderCheck>().collided;

        // Set material according to canBuildOnTile variable
        floorHighlight.GetComponent<Renderer>().material = canBuildOnTile ? greenBuild : redBuild;
    }

    public void ChooseBuilding()
    {
        // If active state is already BUILDING_SELECTED - return
        if (state == State.BUILDING_SELECTED)
            return;

        // Set state
        state = State.BUILDING_SELECTED;

        // Choose first and only building registered in Game Manager
        selectedBuilding = gameManager.BuildingsList[0];

        // Instantiate building's silhouette prefab on currently highlighted floor tile
        building_silhouette = (GameObject)Instantiate(selectedBuilding.SilhouettePrefab, floorHighlight.transform.position, Quaternion.identity);
    }

    private void Build()
    {
        // Check floorHighlit collider if building can be build on selected floor tile - if not, show message and return
        bool canBuildOnTile = !floorHighlight.GetComponent<FloorHighlightColliderCheck>().collided;
        if (!canBuildOnTile)
        {
            Debug.Log("CAN'T BUILD THERE!");
            return;
        }

        // If above check returned canBuildOnTile = true proceed to build
        // Clone building
        GameObject built = (GameObject)Instantiate(selectedBuilding.Prefab, building_silhouette.transform.position, building_silhouette.transform.rotation);

        // Register new building
        int index = gameManager.RegisterBuildingInScene(0, built.transform);

        // Change name to index_buildingName
        built.name = index + "_" + built.name;
    }

    // Controls start and stop of building mode
    #region Start and stop
    public void ToggleBuilding()
    {
        if (state == State.CLOSED)
            StartBuilding();
        else
            StopBuilding();
    }

    private void StartBuilding()
    {
        state = State.OPEN;
        // Animate camera to build mode position
        Camera.main.GetComponent<CameraController>().ChangeToBuilder();
        // Enable Building UI Panel
        buildingUIPanel.SetActive(true);
        // Instantiate floor highlight
        floorHighlight = (GameObject)Instantiate(prefab_FloorHighlight, new Vector3(0, 0, 0), Quaternion.identity);

        Debug.Log("Build mode started.");
    }

    private void StopBuilding()
    {
        state = State.CLOSED;
        // Animate camera to shooter mode position
        Camera.main.GetComponent<CameraController>().ChangeToShooter();

        // Disable Building UI Panel
        buildingUIPanel.SetActive(false);

        // Destroy floor highlight instance
        Destroy(floorHighlight);
        // Destroy building instance
        Destroy(building_silhouette);
        // Destroy selected building
        selectedBuilding = null;

        Debug.Log("Build mode stopped.");
    }
    #endregion
}
