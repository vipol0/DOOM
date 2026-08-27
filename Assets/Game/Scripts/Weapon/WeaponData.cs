using UnityEngine;

[CreateAssetMenu(fileName = "New weapon data", menuName = "Weapon/New weapon data", order = 0)]
public class WeaponData : ScriptableObject
{
    [SerializeField] private float damage = 15;
    [SerializeField] private int magazineSize = 15;
    [SerializeField] private int startingAmmo = 15;
    [SerializeField] private float shootRange = 25;
    [SerializeField] private float shootCooldown = 0.1f;
    [SerializeField] private float throwForce = 25;
    [SerializeField] private float reloadTime = 2.5f;
    [SerializeField] private LayerMask mask;
    [SerializeField] private WeaponType weaponType;

    public float Damage => damage;
    public int MagazineSize => magazineSize;
    public int StartingAmmo => startingAmmo;
    public float ShootRange => shootRange;
    public float ShootCooldown => shootCooldown;
    public float ThrowForce => throwForce;
    public float ReloadTime => reloadTime;
    public LayerMask Mask => mask;
    public WeaponType WeaponType => weaponType;
}