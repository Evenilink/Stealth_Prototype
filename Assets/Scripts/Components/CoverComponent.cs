using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverComponent : MonoBehaviour {

    private const float RAYCAST_LENGTH = 5f;
    private const float RAYCAST_SPHERE_RADIOUS = 0.5f;
    private const float RAYCAST_CAN_MOVE_LENGTH = 1.5f;

    [SerializeField] private LayerMask coverObstaclesLayer;
    private bool isInCover = false;
    private Vector3 coverNormal;
    private Vector3 checkLeft;
    private Vector3 checkRight;
    private bool canMoveRight;
    private bool canMoveLeft;

    [Header("Debug")]
    private bool debug = true;
    private float currHitDtsiance = RAYCAST_LENGTH;

    private void Update() {
        if (debug && !isInCover) {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, RAYCAST_SPHERE_RADIOUS, transform.forward, out hit, RAYCAST_LENGTH, coverObstaclesLayer, QueryTriggerInteraction.Ignore)) {
                Debug.DrawLine(hit.point, hit.point + hit.normal * 20);
            }
        }
        if (isInCover) {
            if (debug) {
                Vector3 end = transform.position + Quaternion.Euler(0, 135, 0) * coverNormal * RAYCAST_CAN_MOVE_LENGTH;
                Debug.DrawLine(transform.position, end, Color.green, 1f);
                Vector3 end2 = transform.position + Quaternion.Euler(0, -135, 0) * coverNormal * RAYCAST_CAN_MOVE_LENGTH;
                Debug.DrawLine(transform.position, end2, Color.blue, 1f);
            }

            canMoveRight = Physics.Raycast(transform.position, checkRight, RAYCAST_CAN_MOVE_LENGTH, coverObstaclesLayer, QueryTriggerInteraction.UseGlobal);
            canMoveLeft = Physics.Raycast(transform.position, checkLeft, RAYCAST_CAN_MOVE_LENGTH, coverObstaclesLayer, QueryTriggerInteraction.UseGlobal);
        }
    }

    public bool ToogleCover() {
        if (!isInCover) {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, RAYCAST_SPHERE_RADIOUS, transform.forward, out hit, RAYCAST_LENGTH, coverObstaclesLayer, QueryTriggerInteraction.Ignore)) {
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
        // We save the hit normal  and calculate the vectors that will help determine if the player can continue to move left or right in cover.
        coverNormal = hit.normal;
        checkLeft = Quaternion.Euler(0, 135, 0) * coverNormal * RAYCAST_CAN_MOVE_LENGTH;
        checkRight = Quaternion.Euler(0, -135, 0) * coverNormal * RAYCAST_CAN_MOVE_LENGTH;

        // Set new position.
        Vector3 coverPosition = hit.point;
        coverPosition.y = transform.position.y;
        transform.position = coverPosition;

        // Set new rotation.
        float angleToRotate = Vector3.Angle(transform.forward, hit.normal);
        transform.localRotation = Quaternion.LookRotation(hit.normal, transform.up);

        isInCover = true;
    }

    private void DeactivateCover() {
        isInCover = false;
    }

    /*private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, transform.position + transform.forward * currHitDtsiance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * currHitDtsiance, RAYCAST_SPHERE_RADIOUS);
    }*/

    public Vector3 GetCoverNormal() {
        return coverNormal;
    }

    public bool CanMoveLeft() {
        return canMoveLeft;
    }

    public bool CanMoveRight() {
        return canMoveRight;
    }
}
