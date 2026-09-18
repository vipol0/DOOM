using UnityEngine;

public class AkimboGun : Weapon
{
    private static readonly int ShootVariantHash = Animator.StringToHash("ShootVariant");
    private static readonly int ShootTriggerHash = Animator.StringToHash("Shoot");
    private int shootVariant = 1;

    public override void OnShoot()
    {
        base.OnShoot();
        animator.SetInteger(ShootVariantHash, shootVariant);

        switch (shootVariant)
        {
            case 1:
                shootVariant = 2;
                break;
            case 2:
                shootVariant = 1;
                break;
        }
    }
}