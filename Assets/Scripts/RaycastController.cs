using UnityEngine;

public class RaycastController : MonoBehaviour
{
    // Ignore collisions when cast ray to floor tile
    public LayerMask ignoreEverythingExceptFloorTiles;

    /// <summary>
    /// Returns /GameObject/ of a floor tile that was hit by the raycast.
    /// </summary>
    /// <param name="mousePos">Position of mouse cursor. Should be Input.mousePosition</param>
    /// <returns></returns>
    public GameObject RaycastToFloorTile(Vector3 mousePos)
    {
        //RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit[] raycastHit = Physics.RaycastAll(ray, Mathf.Infinity, ignoreEverythingExceptFloorTiles);

        foreach (RaycastHit hit in raycastHit)
        {
            Transform objectHit = hit.transform;

            if (1 << objectHit.gameObject.layer == ignoreEverythingExceptFloorTiles)
                return objectHit.gameObject;
        }

        return null;
    }
}
