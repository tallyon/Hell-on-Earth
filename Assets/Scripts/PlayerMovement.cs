using UnityEngine;

/// <summary>
/// This script handles Player's input and movement routines.
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    private Vector3 movementInput = new Vector3(0, 0, 0);
    public float speed;

    private BuildController buildController;
    private PlayerController playerController;

    void Start()
    {
        buildController = gameObject.GetComponent<BuildController>();
        playerController = gameObject.GetComponent<PlayerController>();

        if (buildController == null)
            Debug.LogError("There is no BuildController script on " + name);
        if (playerController == null)
            Debug.LogError("There is no PlayerController script on " + name);
    }

    void Update()
    {
        // ***************************************
        // Handle input
        // [TOGGLE] TAB key press brings up building menu, if it is open closes it
        if (Input.GetKeyDown(KeyCode.Tab))
            buildController.ToggleBuilding();

        // Left mouse button handling
        if (Input.GetMouseButtonDown(0))
        {
            playerController.HandleLeftMouseButtonClick(buildMode: buildController.state == BuildController.State.CLOSED ? false : true);
        }

        // ESC to quit
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        // ***************************************

        // ***************************************
        // Handle movement
        // Capture movement input on X and Y axis and translate movement on XZ plane
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.z = Input.GetAxisRaw("Vertical");

        // Move the player according to moveInput
        transform.Translate(movementInput.normalized * Time.deltaTime * speed);
        // ***************************************
    }
}
