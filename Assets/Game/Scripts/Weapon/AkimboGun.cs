public class AkimboGun : Weapon
{
    private int shootVariant = 1;

    public override void OnShoot()
    {
        base.OnShoot();
        animator.SetInteger("ShootVariant", shootVariant);

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