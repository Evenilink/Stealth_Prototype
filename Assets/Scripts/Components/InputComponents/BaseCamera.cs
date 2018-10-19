using UnityEngine;

public class BaseCamera : MonoBehaviour {

    [SerializeField] protected GameObject target;
    [SerializeField] protected float hSensibility = 2f;
    [SerializeField] protected float vSensibility = 2f;
    [SerializeField, Range(0, 90)] protected float maxUpAngle = 90f;
    [SerializeField, Range(0, 90)] protected float minDownAngle = 90f;
    protected Vector2 mouseDelta;
    protected Vector2 currMouseLook;

    protected void Start() {
        mouseDelta = new Vector2(0, 0);
    }

    protected void LateUpdate() {
        // Mouse camera calculations.
        mouseDelta.x = Input.GetAxisRaw("Mouse X") * hSensibility;
        mouseDelta.y = Input.GetAxisRaw("Mouse Y") * vSensibility;
        currMouseLook.x += mouseDelta.x;
        currMouseLook.y = Mathf.Clamp(currMouseLook.y + mouseDelta.y, -minDownAngle, maxUpAngle);
    }

    public Vector3 GetRotation() {
        return currMouseLook;
    }

    public void SetLastCameraRotation(Vector3 rotation) {
        currMouseLook = rotation;
    }
}
