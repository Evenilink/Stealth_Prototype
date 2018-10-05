using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableComponent : MonoBehaviour {

    private float weight;

    // TODO: Remove validation checks. DEBUG only.
    private void Start() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
            Debug.LogError("Pickable object '" + name + "' must have an associated rigidbody.");
        weight = rigidbody.mass;
    }

    public float GetWeight() {
        return weight;
    }
}
