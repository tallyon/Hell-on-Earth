using UnityEngine;

public class UIBuildingController : MonoBehaviour
{
    public BuildController buildController;

    // Update is called once per frame
    void Update()
    {
        // Check for key presses (shortcuts)
        // 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
            buildController.ChooseBuilding();
    }
}
