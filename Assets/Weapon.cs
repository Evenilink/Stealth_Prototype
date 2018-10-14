using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : BaseItem {

    [Header("Stats")]
    [SerializeField] private WeaponType weaponType;
    [SerializeField] public float fireRate;
    [SerializeField] private float reloadTime;
    [SerializeField] private float damage;
    [SerializeField] private int ammoCapacity;  // TODO: remove this.
    [SerializeField] private int clipCapacity;
    [SerializeField] private float bulletForce;
    [SerializeField] private GameObject[] compatibleBullets;
    [SerializeField] private GameObject[] compatibleUpgrades;
    private enum WeaponType {
        PISTOL,
        ASSAULT_RIFLE,
        SNIPER_RIFLE,
        SHOTGUN
    }

    [Header("Defaults")]
    [SerializeField] private Transform[] bulletSpawnPoints;
    private float timeBetweenShots; // Calculated based on the fire rate.
    private float timeLastShot = 0f;
    private GameObject equippedBullets;
    private List<Upgrade> equippedUpgrades;
    private int currAmmo;
    private int currClip;
    private bool isReloading = false;

    public delegate void ClipAmmoChange(int currClip);
    public static ClipAmmoChange OnClipAmmoChange;

    private void Awake() {
        equippedBullets = compatibleBullets[0]; // FIXME: temporary fix.
        timeBetweenShots = 1f / fireRate;
        currClip = clipCapacity;
    }

    private void Start() {
        if (OnClipAmmoChange != null)
            OnClipAmmoChange(currClip);
    }

    public void Fire() {
        if (!CanFire())
            return;

        if (HasEmptyClip()) {
            if (!isReloading)
                StartCoroutine(Reload());
            return;
        }

        for (int i = 0; i < bulletSpawnPoints.Length; i++) {
            GameObject bullet = Instantiate(equippedBullets, bulletSpawnPoints[i].position, transform.rotation);
            bullet.GetComponent<Bullet>().Fire(transform.forward, bulletForce);
        }

        timeLastShot = Time.time;
        SetCurrClip(currClip - 1);
    }

    private bool CanFire() {    // TODO: change method name.
        return Time.time - timeLastShot >= timeBetweenShots;
    }

    private bool HasEmptyClip() {
        return currClip <= 0;
    }

    private IEnumerator Reload() {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        SetCurrClip(clipCapacity);
        isReloading = false;
    }

    private void SetCurrClip(int newCurrClip) {
        currClip = newCurrClip;
        if (OnClipAmmoChange != null)
            OnClipAmmoChange(currClip);
    }

    private void EquipUpgrade(Upgrade upgrade) {
        equippedUpgrades.Add(upgrade);
        upgrade.Use(this);
    }
}
