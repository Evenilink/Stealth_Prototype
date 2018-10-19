using UnityEngine;

public class CoverComponent : MonoBehaviour {

    [Header("Default")]
    [SerializeField] private LayerMask coverObstaclesLayer;
    [SerializeField] private const float RAYCAST_LENGTH = 5f;
    private const float RAYCAST_SPHERE_RADIOUS = 0.5f;
    private bool isInCover = false;

    [Header("Lateral Movement")]
    // The distance in which the player will stop before bumping into other cover.
    [SerializeField] private float minDistanceToMoveFromCover = 1f;
    private const float checkCanMoveRaycastLength = 1.5f;
    private bool canKeepMoving;
    private Vector3 checkRight;
    private Vector3 checkLeft;
    private Vector3 coverNormal;
    public enum Side {
        RIGHT, LEFT
    };

    [Header("Swap")]
    [SerializeField] private float swapTriggerTime = 0.5f;
    [SerializeField] private float swapRaycastLength = 1.3f;
    private RaycastHit swapHit;
    private bool swapAvailable = false;

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

    public void UpdateComponent(Vector3 dir, Side side) {
        if (isInCover) {
            CalculateLateralMovementAvailability(dir, side);
            CalculateSwapChangeAvailability(dir);
        }
    }

    private void CalculateLateralMovementAvailability(Vector3 dir, Side side) {
        if (debug) {
            Vector3 end = transform.position + (side == Side.RIGHT ? checkRight : checkLeft) * checkCanMoveRaycastLength;
            Debug.DrawLine(transform.position, end, Color.black);
        }
        bool currCanMove = Physics.Raycast(transform.position, side == Side.RIGHT ? checkRight : checkLeft, checkCanMoveRaycastLength, coverObstaclesLayer);
        if (!currCanMove && canKeepMoving && OnCornerEnter != null)
            OnCornerEnter(side);
        if (currCanMove && !canKeepMoving && OnCornerExit != null)
            OnCornerExit();
        canKeepMoving = currCanMove;
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
        checkRight = Quaternion.Euler(0, -135, 0) * coverNormal;
        checkLeft = Quaternion.Euler(0, 135, 0) * coverNormal;

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

    public bool CanKeepMoving() {
        return canKeepMoving;
    }

    /*private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, transform.position + transform.forward * currHitDtsiance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * currHitDtsiance, RAYCAST_SPHERE_RADIOUS);
    }*/

    // *************************************
    // SWAP
    // *************************************

    private void CalculateSwapChangeAvailability(Vector3 dir) {
        // Direct cover.
        if (debug) {
            Debug.DrawLine(transform.position, transform.position + dir * swapRaycastLength, Color.green);
            Debug.DrawLine(transform.position + dir * swapRaycastLength + -transform.forward * 2f, transform.position + dir * swapRaycastLength + -transform.forward * 2f + -dir * swapRaycastLength, Color.yellow);
        }

        RaycastHit hit;
        // Test if a direct swap if possible.
        bool directSwapAvailable = Physics.Raycast(transform.position, dir, out hit, swapRaycastLength, coverObstaclesLayer);
        bool undirectSwapAvailable = false;
        // If a direct swap isn't possible, we'll attempt an undirect swap.
        if (!directSwapAvailable)
            undirectSwapAvailable = Physics.Raycast(transform.position + dir * swapRaycastLength + -transform.forward, -dir, out hit, swapRaycastLength * 2f, coverObstaclesLayer);
        if(directSwapAvailable || undirectSwapAvailable) {
            if (hit.point != swapHit.point || !swapAvailable) {
                swapAvailable = true;
                swapHit = hit;
                if (OnSwapChangeAvailability != null)
                    OnSwapChangeAvailability(true);
            }
            // We only want to stop the movement if we're too close to a direct swap.
            if (directSwapAvailable && hit.distance <= minDistanceToMoveFromCover)
                canKeepMoving = false;
        }
        else if (swapAvailable) {
            swapAvailable = false;
            if (OnSwapChangeAvailability != null)
                OnSwapChangeAvailability(false);
        }
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
