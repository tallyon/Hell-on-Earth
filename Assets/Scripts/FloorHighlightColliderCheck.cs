using UnityEngine;

public class FloorHighlightColliderCheck : MonoBehaviour
{
    public bool collided = false;

    void OnTriggerStay(Collider col)
    {
        // Don't check collision with triggers
        if (col.isTrigger == true)
            return;

        collided = true;
    }

    void OnTriggerExit(Collider col)
    {
        // Don't check collision with triggers
        if (col.isTrigger == true)
            return;

        collided = false;
    }
}
