using UnityEngine;

public class FirstPersonCameraComponent : BaseCamera {

    private void Start () {
        base.Start();
        // x starts with the current y rotation of the target, and y with the x rotation of the target,
        // so that the camera can already start in its default position, since in the LateUpdate method,
        // we're replacing its rotation value, instead of adding to it.
        currMouseLook = new Vector2(target.transform.eulerAngles.y, target.transform.eulerAngles.x);
    }
	
	private void LateUpdate () {
        base.LateUpdate();

        transform.localRotation = Quaternion.AngleAxis(-currMouseLook.y, Vector3.right); // Vertical rotation.
        target.transform.localRotation = Quaternion.AngleAxis(currMouseLook.x, Vector3.up);  // Horizontal rotation.
    }
}
