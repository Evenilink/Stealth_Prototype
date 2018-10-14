using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : BaseItem, IUpgrade {

    public void Use(Weapon weapon) {
        weapon.fireRate *= 0.3f;
    }
}
