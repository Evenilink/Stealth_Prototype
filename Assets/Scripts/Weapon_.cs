using UnityEngine;

public class Weapon_ : MonoBehaviour {

    [SerializeField] private BaseWeapon weaponStats;
    [SerializeField] private BaseBullet bulletStats;
    [SerializeField] private Transform weaponHolder;
    private Transform[] bulletSpawnPoints;
    private GameObject weaponObj;

    private float timeBetweenShots; // Calculated based on the fire rate.
    private float timeLastShot = 0f;

    protected void Awake() {
        timeBetweenShots = 1f / weaponStats.fireRate;
    }

    protected void Start() {
        SetWeapon(weaponStats);
    }

    public void Fire() {
        if (!CanFire())
            return;

        for (int i = 0; i < bulletSpawnPoints.Length; i++) {
            GameObject bullet = Instantiate(bulletStats.prefab, bulletSpawnPoints[i].position, transform.rotation);
            bullet.GetComponent<Bullet>().Fire(weaponObj.transform.forward, weaponStats.bulletSpeed);
        }

        timeLastShot = Time.time;
    }

    private void SetWeapon(BaseWeapon newWeaponStats) {
        weaponStats = newWeaponStats;
        weaponObj = Instantiate(weaponStats.prefab, weaponHolder);
        bulletSpawnPoints = weaponObj.GetComponent<BulletSpawnLocation>().points;
    }

    private bool CanFire() {
        return Time.time - timeLastShot >= timeBetweenShots;
    }
}
