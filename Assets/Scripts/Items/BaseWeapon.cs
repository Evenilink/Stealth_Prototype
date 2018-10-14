using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon")]
public class BaseWeapon : BaseItem {

    [Header("Weapon")]
    public WeaponType weaponType;
    public float fireRate;
    public float reloadSpeed;
    public float damage;
    public int ammoCapacity;
    public float bulletSpeed;
    public GameObject prefab;
    public BaseUpgrade[] allowedUpgrades;

    public enum WeaponType {
        PISTOL,
        ASSAULT_RIFLE,
        SNIPER_RIFLE,
        SHOTGUN
    }
}