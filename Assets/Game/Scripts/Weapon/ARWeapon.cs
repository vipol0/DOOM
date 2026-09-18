using UnityEngine;

public class ARWeapon : Weapon
{
    private bool isShooting;

    private void Update()
    {
        if (!isHeld)
            return;

        bool holdingFire = Input.GetMouseButton(0);

        if (holdingFire)
            OnShoot();
    }

    public override void OnShoot()
    {
        if (!CanShoot())
            return;

        if (currentAmmo <= 0)
        {
            OnReload();
            return;
        }

        Shoot();
    }
}