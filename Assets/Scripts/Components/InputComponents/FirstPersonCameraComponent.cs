using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraComponent : MonoBehaviour {

    [SerializeField] private GameObject target;
    [SerializeField] private float hSensibility = 2f;
    [SerializeField] private float vSensibility = 2f;
    [SerializeField, Range(0, 90)] private float maxUpAngle = 90f;
    [SerializeField, Range(0, 90)] private float minDownAngle = 90f;
    private Vector2 mouseDelta;
    private Vector2 currMouseLook;

    void Start () {
        mouseDelta = new Vector2(0, 0);
        currMouseLook = new Vector2(0, 0);
    }
	
	void Update () {
        // Mouse camera calculations.
        mouseDelta.x = Input.GetAxisRaw("Mouse X") * hSensibility;
        mouseDelta.y = Input.GetAxisRaw("Mouse Y") * vSensibility;
        currMouseLook.x += mouseDelta.x;
        currMouseLook.y = Mathf.Clamp(currMouseLook.y + mouseDelta.y, -minDownAngle, maxUpAngle);

        transform.localRotation = Quaternion.AngleAxis(-currMouseLook.y, Vector3.right); // Vertical rotation.
        target.transform.localRotation = Quaternion.AngleAxis(currMouseLook.x, Vector3.up);  // Horizontal rotation.
    }
}
