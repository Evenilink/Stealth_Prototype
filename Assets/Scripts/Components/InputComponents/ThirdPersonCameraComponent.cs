using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraComponent : MonoBehaviour {

    [Header("Camera Settings")]
    [SerializeField] private GameObject target;
    [SerializeField] private Transform rotationPivot;
    [SerializeField] private float hSensitivity = 2f;
    [SerializeField] private float vSensitivity = 2f;
    [SerializeField, Range(0, 90)] private float maxUpAngle = 90f;
    [SerializeField, Range(0, 90)] private float minDownAngle = 90f;
    private Vector3 mouseDelta;
    private Vector3 currMouseLook;

    [Header("Corner Settings")]
    [SerializeField] private float aroundCornerOffset;
    [SerializeField] private float aroundCornerTime;
    private Vector3 pivotLocalPosition;
    private IEnumerator cornerCoroutine;

    void Start () {
        mouseDelta = new Vector3(0, 0);
        // x starts with the current y rotation of the pivot, so that the camera can already start in its default position,
        // since in the LateUpdate method, we're replacing its rotation value, instead of adding to it.
        currMouseLook = new Vector3(rotationPivot.localEulerAngles.y, 0);
        pivotLocalPosition = rotationPivot.localPosition;
        CoverComponent.OnCornerEnter += OnCornerEnterHandler;
        CoverComponent.OnCornerExit += OnCornerExitHandler;
    }

    void LateUpdate() {
        mouseDelta.x = Input.GetAxis("Mouse X") * hSensitivity;
        mouseDelta.y = Input.GetAxis("Mouse Y") * vSensitivity;

        currMouseLook.x += mouseDelta.x;
        currMouseLook.y = Mathf.Clamp(currMouseLook.y + mouseDelta.y, -minDownAngle, maxUpAngle);

        // If we want the vertical rotation axis to be inverted (as we normally do), we use a minus here.
        Quaternion rotation = Quaternion.Euler(-currMouseLook.y, currMouseLook.x, 0);
        rotationPivot.rotation = rotation;     
    }

    private void OnCornerEnterHandler(CoverComponent.Side fromSide) {
        if (cornerCoroutine != null)
            StopCoroutine(cornerCoroutine);
        Vector3 newCamPosition = pivotLocalPosition + new Vector3(fromSide == CoverComponent.Side.RIGHT ? - aroundCornerOffset : aroundCornerOffset, 0, 0); // 'x' axis is negative when we're in the 'right' corner because the pivot is rotated 180 degrees by default, so axis values are inverted.
        cornerCoroutine = SetAroundCornerView(newCamPosition);
        StartCoroutine(cornerCoroutine);
    }

    private void OnCornerExitHandler() {
        if (cornerCoroutine != null)
            StopCoroutine(cornerCoroutine);
        cornerCoroutine = SetAroundCornerView(pivotLocalPosition);
        StartCoroutine(cornerCoroutine);
    }

    IEnumerator SetAroundCornerView(Vector3 finalPosition) {
        float startedTime = Time.time;
        float speed = aroundCornerOffset / aroundCornerTime;
        float fracCompleted = 0f;

        while (fracCompleted < 1f) {
            float distCovered = speed * (Time.time - startedTime);
            fracCompleted = distCovered / aroundCornerOffset;
            rotationPivot.localPosition = Vector3.Lerp(rotationPivot.localPosition, finalPosition, fracCompleted);
            yield return null;
        }
    }

    private void OnDestroy() {
        CoverComponent.OnCornerEnter -= OnCornerEnterHandler;
        CoverComponent.OnCornerExit -= OnCornerExitHandler;
    }
}
