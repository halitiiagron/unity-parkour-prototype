using UnityEngine;

public class ClimbingSystem : MonoBehaviour
{
    [Header("References")]
    public SimplePlayerController playerController;
    public CharacterController characterController;

    [Header("Climbing Settings")]
    public float climbSpeed = 3f;
    public float maxLedgeHeight = 3f;
    public float detectionDistance = 1f;

    private bool isClimbing = false;
    private Vector3 climbTarget;

    void Update()
    {
        if (isClimbing)
        {
            HandleClimbingMovement();
        }
        else
        {
            CheckForLedge();
        }
    }

    void CheckForLedge()
    {
        // Raycast forward to detect ledge
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionDistance))
        {
            if (hit.collider.CompareTag("Climbable"))
            {
                float ledgeHeight = hit.point.y - transform.position.y;

                // Check if ledge is within climbable height
                if (ledgeHeight <= maxLedgeHeight && ledgeHeight > 0.5f)
                {
                    Debug.Log("Ledge detected at height: " + ledgeHeight);

                    // Start climb on Jump press
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        StartClimbing(hit.point, ledgeHeight);
                    }
                }
            }
        }
    }

    void StartClimbing(Vector3 ledgePoint, float height)
    {
        isClimbing = true;

        // Calculate climb target (top of ledge)
        climbTarget = new Vector3(
            transform.position.x,
            ledgePoint.y + characterController.height / 2,
            transform.position.z
        );

        Debug.Log("Starting climb to: " + climbTarget);
    }

    void HandleClimbingMovement()
    {
        // Move player upward
        Vector3 moveDirection = (climbTarget - transform.position).normalized;
        characterController.Move(moveDirection * climbSpeed * Time.deltaTime);

        // Check if reached top
        if (Vector3.Distance(transform.position, climbTarget) < 0.1f)
        {
            FinishClimbing();
        }
    }

    void FinishClimbing()
    {
        isClimbing = false;
        Debug.Log("Climb finished!");
    }

    // Visual debug for raycast
    void OnDrawGizmos()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayOrigin, transform.forward * detectionDistance);
    }
}