using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraComponent : MonoBehaviour {

    [SerializeField] private GameObject target;
    [SerializeField] private float hSensitivity = 2f;
    [SerializeField] private float vSensitivity = 2f;
    [SerializeField, Range(0, 90)] private float maxUpAngle = 90f;
    [SerializeField, Range(0, 90)] private float minDownAngle = 90f;
    private Vector3 offset;
    private Vector3 mouseDelta;
    private Vector3 currMouseLook;

    void Start () {
        offset = target.transform.position - transform.position;
        mouseDelta = new Vector3(0, 0);
        currMouseLook = new Vector3(0, 0);
    }

    void LateUpdate() {
        mouseDelta.x = Input.GetAxis("Mouse X") * hSensitivity;
        mouseDelta.y = Input.GetAxis("Mouse Y") * vSensitivity;

        currMouseLook.x += mouseDelta.x;
        currMouseLook.y = Mathf.Clamp(currMouseLook.y + mouseDelta.y, -minDownAngle, maxUpAngle);

        Quaternion rotation = Quaternion.Euler(currMouseLook.y, currMouseLook.x, 0);
        transform.position = target.transform.position - (rotation * offset);   // TODO: how does this quaternion gets calculated with a Vector3?

        transform.LookAt(target.transform);
    }
}
