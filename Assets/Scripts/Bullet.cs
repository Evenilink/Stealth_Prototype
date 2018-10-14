using UnityEngine;

public class Bullet : BaseItem {



    public void Fire(Vector3 direction, float speed) {
        GetComponent<Rigidbody>().AddForce(direction * speed, ForceMode.Impulse);
        Destroy(gameObject, 5.0f);
    }

    private void OnCollisionEnter(Collision collision) {
        Destroy(gameObject);
    }
}
