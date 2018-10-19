using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraComponent : BaseCamera {

    [Header("Camera Settings")]
    [SerializeField] private Transform rotationPivot;

    [Header("Corner Settings")]
    [SerializeField] private float aroundCornerOffset;
    [SerializeField] private float aroundCornerTime;
    private Vector3 pivotLocalPosition;
    private IEnumerator cornerCoroutine;

    void Start () {
        // x starts with the current y rotation of the pivot, so that the camera can already start in its default position,
        // since in the LateUpdate method, we're replacing its rotation value, instead of adding to it.
        currMouseLook = new Vector3(rotationPivot.localEulerAngles.y, 0);
        pivotLocalPosition = rotationPivot.localPosition;
        CoverComponent.OnCornerEnter += OnCornerEnterHandler;
        CoverComponent.OnCornerExit += OnCornerExitHandler;
    }

    protected void LateUpdate() {
        base.LateUpdate();
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
