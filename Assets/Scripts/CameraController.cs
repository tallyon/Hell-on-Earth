using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    // Shooter camera position
    public Vector3 shooterCameraPos;
    public Vector3 shooterCameraRot;

    // Builder camera position
    public Vector3 builderCameraPos;
    public Vector3 builderCameraRot;

    private Animator animator;
    
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        // Set camera's fixed Y rotation
        transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
    }

    public void ChangeToShooter()
    {
        // Transition camera to shooter position
        animator.Play("CameraToShooterMode");
        Debug.Log("Change to shooter mode");
    }

    public void ChangeToBuilder()
    {
        // Transition camera to builder position
        animator.Play("CameraToBuildMode");
        Debug.Log("Change to builder mode");
    }
}
