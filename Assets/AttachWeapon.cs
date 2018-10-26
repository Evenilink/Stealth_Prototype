using UnityEngine;

public class AttachWeapon : MonoBehaviour {

    [SerializeField] private Transform attachTransform;

    void Start () {
        transform.parent = attachTransform;
        transform.localPosition = Vector3.zero;
	}
}
