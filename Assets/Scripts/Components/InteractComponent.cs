using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractComponent : MonoBehaviour {

    [Header("Default")]
    [SerializeField] private Transform view;    // Raycast from this transform.
    [SerializeField] private LayerMask interactablesLayer;
    private GameObject interactable; // Object the 'view' is looking.
    private Vector3 interactableNormal;
    private float raycastDistance = 1.5f;

    [Header("Pickup")]
    [SerializeField] private float holdingDistance = 1.5f;
    private IEnumerator pickObject;
    private bool pickedUp = false;

    // Event Dispatchers.
    public delegate void SeeInteractable(GameObject gameObject);
    public static SeeInteractable OnSeeInteractable;
    public delegate void StopSeeInteractable();
    public static StopSeeInteractable OnStopSeeInteractable;

    void Update () {
        // If we already have a picked up object, we don't need to raycast.
        if (pickedUp)
            return;

        Debug.DrawLine(view.position, view.position + view.forward * raycastDistance, Color.yellow);
        // Shots a raycast to detect interactable objects.
        RaycastHit hit;
        if (Physics.Raycast(view.position, view.forward, out hit, raycastDistance, interactablesLayer)) {
            GameObject currInteractable = hit.collider.gameObject;
            if (currInteractable != interactable) {
                interactable = currInteractable;
                if (OnSeeInteractable != null)
                    OnSeeInteractable(currInteractable);
            }
            interactableNormal = hit.normal;
        } else if (interactable != null) {
            interactable = null;
            interactableNormal = Vector3.zero;
            if (OnStopSeeInteractable != null)
                OnStopSeeInteractable();
        }
	}

    public void Interact() {
        if (interactable == null)
            return;

        // Pickable object.
        PickableComponent pickableComp = interactable.GetComponent<PickableComponent>();
        if (pickableComp != null) {
            //GetAngleToPickObject(interactable, interactableNormal);
            if (pickedUp) {
                StopCoroutine(pickObject);
                DropObject();
            } else {
                pickObject = PickObject();
                StartCoroutine(pickObject);
            }
        }

        // TODO: NPC dialogue.
    }

    // Picks up the current facing object.
    IEnumerator PickObject() {
        pickedUp = true;
        float yAngleToAdd = GetAngleToPickObject(interactable, interactableNormal);
        float finalAngle = interactable.transform.eulerAngles.y + yAngleToAdd;
        interactable.GetComponent<Rigidbody>().useGravity = false;
        float a = 0;
        float initialCamYRotation = view.eulerAngles.y;

        while (true) {
            // The 'view' position +
            // the character forward vector * distance we want to hold the object +
            // the sin amount of the 'view' rotation in the 'y' axis (sets the picked object 'y' axis based on the 'view' rotation).
            interactable.transform.position = Vector3.Lerp(interactable.transform.position, view.position + transform.forward * holdingDistance + new Vector3(0, - Mathf.Sin(view.eulerAngles.x * Mathf.Deg2Rad), 0), a);

            // 'finalAngle' is the angle the objects needs to face the player. Nevertheless, when the player rotates with the first person camera, that angle will always stay the same,
            // so we need to compensate with the camera rotation from the moment we start rotating! So we need to take the initial camera rotation out.
            float camYRotationDelta = view.eulerAngles.y - initialCamYRotation;
            // The picked object is always facing 'view' without rotating the other axis.
            // E.g. in this example, we're using the first person camera as the view, and although its 'y' local rotation is 0, its global one isn't.
            // interactable.transform.eulerAngles = Vector3.Lerp(interactable.transform.eulerAngles, new Vector3(0, yAngle + view.eulerAngles.y, 0), a);
            interactable.transform.eulerAngles = new Vector3(0, finalAngle + camYRotationDelta, 0);
            // interactable.transform.eulerAngles = Vector3.Lerp(interactable.transform.eulerAngles, new Vector3(0, finalAngle + view.eulerAngles.y, 0), a);
            a += Time.deltaTime;
            yield return null;
        }
    }

    // Drops the current picked object.
    private void DropObject() {
        pickedUp = false;
        Rigidbody rigidbody = interactable.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.useGravity = true;
    }

    private float GetAngleToPickObject(GameObject gameObject, Vector3 collidedSideNormal) {
        return 180 - Vector3.SignedAngle(transform.forward, collidedSideNormal, Vector3.up);
    }
}
