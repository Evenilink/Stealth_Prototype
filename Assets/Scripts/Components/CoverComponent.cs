using UnityEngine;

public class CoverComponent : MonoBehaviour {

    [Header("Raycast Lengths")]
    private const float RAYCAST_LENGTH = 5f;
    private const float RAYCAST_SPHERE_RADIOUS = 0.5f;

    [SerializeField] private LayerMask coverObstaclesLayer;
    private bool isInCover = false;

    [Header("Lateral Movement")]
    // The distance in which the player will stop before bumping into other cover.
    [SerializeField] private float minDistanceToMoveFromCover = 1f;
    private const float checkCanMoveRaycastLength = 1.5f;
    private Vector3 coverNormal;
    private Vector3 checkLeft;
    private Vector3 checkRight;
    private bool canMoveRight = true;
    private bool canMoveLeft = true;
    public enum Side {
        RIGHT, LEFT
    };

    [Header("Swap")]
    [SerializeField] private float swapTriggerTime = 0.5f;
    [SerializeField] private float swapRaycastLength = 1.3f;
    private RaycastHit swapHit;
    private bool swapLeftAvailable = false;
    private bool swapRightAvailable = false;

    // Event Dispatchers.
    public delegate void CornerEnter(Side fromSide);
    public static CornerEnter OnCornerEnter;
    public delegate void CornerExit();
    public static CornerExit OnCornerExit;
    public delegate void SwapChangeAvailability(bool available);
    public static SwapChangeAvailability OnSwapChangeAvailability;

    [Header("Debug")]
    private bool debug = true;
    private float currHitDtsiance = RAYCAST_LENGTH;

    private void Update() {
        if (debug && !isInCover) {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, RAYCAST_SPHERE_RADIOUS, transform.forward, out hit, RAYCAST_LENGTH, coverObstaclesLayer)) {
                Debug.DrawLine(hit.point, hit.point + hit.normal * 20);
            }
        }
        if (isInCover) {
            CalculateLateralMovementAvailability();
            CalculateSwapChangeAvailability(-transform.right);  // Move right.
            CalculateSwapChangeAvailability(transform.right);   // Move left.
        }
    }

    private void CalculateLateralMovementAvailability() {
        if (debug) {
            Vector3 end = transform.position + Quaternion.Euler(0, 135, 0) * coverNormal * checkCanMoveRaycastLength;
            Debug.DrawLine(transform.position, end, Color.green);
            Vector3 end2 = transform.position + Quaternion.Euler(0, -135, 0) * coverNormal * checkCanMoveRaycastLength;
            Debug.DrawLine(transform.position, end2, Color.blue);
        }

        bool currCanMoveRight = Physics.Raycast(transform.position, checkRight, checkCanMoveRaycastLength, coverObstaclesLayer);
        if (!currCanMoveRight && canMoveRight && OnCornerEnter != null)
            OnCornerEnter(Side.RIGHT);
        if (currCanMoveRight && !canMoveRight && OnCornerExit != null)
            OnCornerExit();
        canMoveRight = currCanMoveRight;

        bool currCanMoveLeft = Physics.Raycast(transform.position, checkLeft, checkCanMoveRaycastLength, coverObstaclesLayer);
        if (!currCanMoveLeft && canMoveLeft && OnCornerEnter != null)
            OnCornerEnter(Side.LEFT);
        if (currCanMoveLeft && !canMoveLeft && OnCornerExit != null)
            OnCornerExit();
        canMoveLeft = currCanMoveLeft;
    }

    public bool ToogleCover() {
        if (!isInCover) {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, RAYCAST_SPHERE_RADIOUS, transform.forward, out hit, RAYCAST_LENGTH, coverObstaclesLayer)) {
                ActivateCover(hit);
                return true;
            }
            else {
                Debug.Log("Couldn't find any surface to cover");
                return false;
            }
        }
        else {
            DeactivateCover();
            return true;
        }
    }

    private void ActivateCover(RaycastHit hit) {
        // We save the hit normal and calculate the vectors that will help determine if the player can continue to move left or right in cover.
        coverNormal = hit.normal;
        checkLeft = Quaternion.Euler(0, 135, 0) * coverNormal;
        checkRight = Quaternion.Euler(0, -135, 0) * coverNormal;

        // Set new position.
        Vector3 coverPosition = hit.point;
        coverPosition.y = transform.position.y;
        transform.position = coverPosition;

        // Set new rotation.
        transform.rotation = Quaternion.LookRotation(hit.normal, transform.up);

        isInCover = true;
    }

    private void DeactivateCover() {
        isInCover = false;
    }

    public bool CanMoveLeft() {
        return canMoveLeft;
    }

    public bool CanMoveRight() {
        return canMoveRight;
    }

    /*private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, transform.position + transform.forward * currHitDtsiance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * currHitDtsiance, RAYCAST_SPHERE_RADIOUS);
    }*/

    // Swap

    private void CalculateSwapChangeAvailability(Vector3 dir) {
        // Direct cover.
        if (debug)
            Debug.DrawLine(transform.position, transform.position + dir * swapRaycastLength, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, swapRaycastLength, coverObstaclesLayer)) {
            if (hit.point != swapHit.point || !swapAvailable) {
                swapAvailable = true;
                swapHit = hit;
                if (OnSwapChangeAvailability != null)
                    OnSwapChangeAvailability(true);
            }

            if (hit.distance <= minDistanceToMoveFromCover) {
                if (dir == -transform.right)
                    canMoveRight = false;
                else canMoveLeft = false;
            }
        }
        else if (swapAvailable) {
            swapAvailable = false;
            if (OnSwapChangeAvailability != null)
                OnSwapChangeAvailability(false);
        }

        // Undirect cover.
    }

    public void Swap() {
        if (swapAvailable)
            ActivateCover(swapHit);
    }

    public bool IsSwapAvailable() {
        return swapAvailable;
    }

    public float GetSwapTriggerTime() {
        return swapTriggerTime;
    }
}
