using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class CoverComponent : MonoBehaviour {

    [Header("Default")]
    // Distance the character is from the wall when in cover.
    // When this distance is smaller than the collider's radius, the distance is set to the radius value.
    [SerializeField, Range(0f, 0.8f)] private float distToWall = 1f;
    [SerializeField] private LayerMask coverObstaclesLayer;
    private const float RAYCAST_LENGTH = 5f;
    private const float RAYCAST_SPHERE_RADIOUS = 0.5f;
    private bool isInCover = false;
    private Vector3 coverNormal;
    private CapsuleCollider selfCollider;
    private float startColliderRadious;
    public enum SwapType { NORMAL, JUMP };

    [Header("Lateral Movement")]
    // The distance in which the player will stop before bumping into other cover.
    [SerializeField] private float minDistanceToMoveFromCover = 1f;
    private const float checkCanMoveRaycastLength = 1.5f;
    private bool canKeepMoving;
    private Vector3 checkRight;
    private Vector3 checkLeft;
    public enum Side { RIGHT, LEFT };

    [Header("Swap")]
    [SerializeField] private float swapTriggerTime = 0.5f;
    [SerializeField] private float swapRaycastLength = 1.3f;
    private RaycastHit swapHit;
    private bool swapAvailable = false;

    [Header("Swap Jump")]
    [SerializeField] private float hJumpSwapDist = 5f;
    [SerializeField] private float vJumpSwapDist = 5f;
    [SerializeField] private int jumpSwapRays = 5;
    private RaycastHit jumpSwapHit;
    private bool jumpSwapAvailable = false;
    // Obj that the player is currently in cover. We need this to remove the possibility of jump swaping to the same cover object, since that object can be big enough to do that.
    private GameObject coverObj;

    // Event Dispatchers.
    public delegate void CornerEnter(Side fromSide);
    public static CornerEnter OnCornerEnter;
    public delegate void CornerExit();
    public static CornerExit OnCornerExit;
    public delegate void SwapChangeAvailability(SwapType type, bool available);
    public static SwapChangeAvailability OnSwapChangeAvailability;

    [Header("Debug")]
    private bool debug = true;

    private void Awake() {
        selfCollider = GetComponent<CapsuleCollider>();
        startColliderRadious = selfCollider.radius;
        // When this distance is smaller than the collider's radius, the distance is set to the radius value.
        if (distToWall < startColliderRadious)
            distToWall = startColliderRadious;
    }

    public void UpdateComponent(Vector3 dir, Side side) {
        if (isInCover) {
            CalculateLateralMovementAvailability(dir, side);
            CalculateSwapChangeAvailability(dir);
            CalculateJumpSwapAvailability(dir);
        }
    }

    // Calculates the availability of lateral movement.
    // If the player is too close to an edge, it makes the movement impossible.
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
                ActivateCover(hit, false);
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

    private void ActivateCover(RaycastHit hit, bool fromCover) {
        // We save the hit normal and calculate the vectors that will help determine if the player can continue to move left or right in cover.
        coverNormal = hit.normal;
        checkRight = Quaternion.Euler(0, -135, 0) * coverNormal;
        checkLeft = Quaternion.Euler(0, 135, 0) * coverNormal;

        if (coverObj != hit.collider.gameObject)
            coverObj = hit.collider.gameObject;

        isInCover = true;
        swapAvailable = false;
        jumpSwapAvailable = false;

        // Set new position.
        Vector3 dstPosition = hit.point + coverNormal * distToWall;
        dstPosition.y = transform.position.y;
        // Set new rotation.
        Quaternion dstRotation = Quaternion.LookRotation(hit.normal, transform.up);

        if (!fromCover) {
            transform.position = dstPosition;
            transform.rotation = dstRotation;
        }
        else StartCoroutine(MoveCoverToCover(dstPosition, dstRotation));
    }

    // Coroutine to move the character from cover to cover, given a destination position and rotation.
    // It works by zeroing the collider radius, applying the timeline, and assign the start radius again to the collider.
    private IEnumerator MoveCoverToCover(Vector3 dstPosition, Quaternion dstRotation) {
        selfCollider.radius = 0f;
        float fracCompleted = 0;
        float startTime = Time.time;
        while (fracCompleted < 1) {
            fracCompleted = (Time.time - startTime) / 0.5f;
            transform.position = Vector3.Lerp(transform.position, dstPosition, fracCompleted);
            transform.rotation = Quaternion.Lerp(transform.rotation, dstRotation, fracCompleted);
            yield return null;
        }
        selfCollider.radius = startColliderRadious;
    }

    private void DeactivateCover() {
        isInCover = false;
    }

    public bool CanKeepMoving() {
        return canKeepMoving;
    }

    // SWAP **********************************************************************************

    // Calculates if a direct or undirect swap is possible, and sets the appropriate variables.
    private void CalculateSwapChangeAvailability(Vector3 dir) {
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
            // We only assign these values when we find a different swap point from the previously found, otherwise we would be
            // wasting CPU time in the assignments, and we would dispatch the event more times than we would like.
            if (hit.point != swapHit.point || !swapAvailable) {
                swapAvailable = true;
                swapHit = hit;
                if (OnSwapChangeAvailability != null)
                    OnSwapChangeAvailability(SwapType.NORMAL, true);
            }
            // We only want to stop the movement if we're too close to a direct swap.
            // We can ignore the undirect swap, since the method "CalculateLateralMovementAvailability" already makes the character stop moving when the raycasts fails.
            if (directSwapAvailable && hit.distance <= minDistanceToMoveFromCover)
                canKeepMoving = false;
        }
        else if (swapAvailable) {
            swapAvailable = false;
            if (OnSwapChangeAvailability != null)
                OnSwapChangeAvailability(SwapType.NORMAL, false);
        }
    }

    // Executes the swap.
    public void Swap() {
        if (swapAvailable)
            ActivateCover(swapHit, true);
    }

    public bool IsSwapAvailable() {
        return swapAvailable;
    }

    public float GetSwapTriggerTime() {
        return swapTriggerTime;
    }

    // JUMP SWAP **********************************************************************************

    // Calculates if a jump swap is possible, and sets the appropriate variables.
    private void CalculateJumpSwapAvailability(Vector3 dir) {
        RaycastHit hit;
        float step = hJumpSwapDist / jumpSwapRays;
        Vector3 startPosition = transform.position + dir * swapRaycastLength + transform.forward * vJumpSwapDist / 2f;
        bool currJumpSwapAvailable = false;

        // We iterate until numRays + 1 because we also want to raycast one with dir direction (the first and higher priority one).
        for (int i = 0; i < jumpSwapRays + 1; i++) {
            startPosition += dir * step;
            Debug.DrawLine(startPosition, startPosition + -transform.forward * vJumpSwapDist, Color.black);
            if (currJumpSwapAvailable = Physics.Raycast(startPosition, -transform.forward, out hit, vJumpSwapDist, coverObstaclesLayer) && hit.collider.gameObject != coverObj) {
                jumpSwapHit = hit;
                break;
            }
        }

        if (currJumpSwapAvailable && !jumpSwapAvailable) {
            if (OnSwapChangeAvailability != null)
                OnSwapChangeAvailability(SwapType.JUMP, true);
        } else if (!currJumpSwapAvailable && jumpSwapAvailable) {
            if (OnSwapChangeAvailability != null)
                OnSwapChangeAvailability(SwapType.JUMP, false);
        }
        jumpSwapAvailable = currJumpSwapAvailable;

    }

    public void JumpSwap() {
        if (isInCover)
            ActivateCover(jumpSwapHit, true);
    }

    public bool IsJumpSwapAvailable() {
        return jumpSwapAvailable;
    }
}
