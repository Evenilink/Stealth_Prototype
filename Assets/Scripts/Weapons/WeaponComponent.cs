using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour {

    [SerializeField] private new Camera camera;
    private float timeBetweenShots; // Calculated based on the fire rate.
    private float timeLastShot;
    private int currBulletsInClip;
    private bool reloading = false;
    private IEnumerator reloadCoroutine;
    private BaseWeapon weapon;

    // Delegates.
    public delegate void Shoot();
    public static Shoot OnShoot;

    private void Start() {
        timeBetweenShots = 1f / weapon.fireRate;
        timeLastShot = 0f;
        currBulletsInClip = weapon.ammoCapacity;
        reloadCoroutine = ReloadCoroutine();
    }

    public void Fire() {
        if (!CanFire())
            return;

        if (currBulletsInClip <= 0) {
            Reload();
            return;
        }

        Debug.Log("Shoot");

        /*RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range)) {
            DamageableComponent target = hit.collider.gameObject.GetComponent<DamageableComponent>();
            if (target != null)
                target.TakeDamage(weapon.damage);
        }*/

        if (OnShoot != null)
            OnShoot();

        timeLastShot = Time.time;
    }

    private bool CanFire() {
        return Time.time - timeLastShot >= timeBetweenShots;
    }

    public void Reload() {
        if (!reloading)
            StartCoroutine(reloadCoroutine);
    }

    private IEnumerator ReloadCoroutine() {
        reloading = true;
        yield return new WaitForSeconds(weapon.reloadSpeed);
        reloading = false;
        currBulletsInClip = weapon.ammoCapacity;
    }
}
