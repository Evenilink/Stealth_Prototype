using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableComponent : MonoBehaviour {

    [SerializeField] private float health = 100f;

    public void TakeDamage(float damage) {
        health -= damage;
        Debug.Log(name + " took " + damage + " damage. Health = " + health);
        if (health <= 0)
            Destroy(gameObject);
    }
}
