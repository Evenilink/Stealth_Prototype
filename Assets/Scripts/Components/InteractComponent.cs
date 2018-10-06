using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractComponent : MonoBehaviour {

    [Header("Default")]
    [SerializeField] private Transform view;    // Raycast from this transform.
    [SerializeField] private LayerMask interactables;
    private GameObject interactable; // Object the 'view' is looking.
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
        if (Physics.Raycast(view.position, view.forward, out hit, raycastDistance, interactables)) {
            GameObject currInteractable = hit.collider.gameObject;
            if (currInteractable != interactable && OnSeeInteractable != null)
                OnSeeInteractable(currInteractable);
            interactable = currInteractable;
        } else if (interactable != null) {
            interactable = null;
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
        interactable.GetComponent<Rigidbody>().useGravity = false;
        //interactable.GetComponent<BoxCollider>().enabled = false;
        float a = 0;

        while (true) {
            // The 'view' position +
            // the character forward vector * distance we want to hold the object +
            // the sin amount of the 'view' rotation in the 'y' axis (sets the picked object 'y' axis based on the 'view' rotation).
            interactable.transform.position = Vector3.Lerp(interactable.transform.position, view.position + transform.forward * holdingDistance + new Vector3(0, - Mathf.Sin(view.eulerAngles.x * Mathf.Deg2Rad), 0), a);
            // The picked object is always facing 'view' without rotating the other axis.
            // E.g. in this example, we're using the first person camera as the view, and although its 'y' local rotation is 0, its global one isn't.
            interactable.transform.eulerAngles = Vector3.Lerp(interactable.transform.eulerAngles, new Vector3(0, view.eulerAngles.y, 0), a);
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
}
