using UnityEngine;

public class ARWeapon : Weapon
{
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private bool isShooting;

    private void Update()
    {
        if (!isHeld)
            return;
        
        if (Input.GetMouseButton(0))
        {
            isShooting = true;
            animator.SetBool(ShootHash, isShooting);
            OnShoot();
        }
        else
        {
            isShooting = false;
            animator.SetBool(ShootHash, isShooting);
        }
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