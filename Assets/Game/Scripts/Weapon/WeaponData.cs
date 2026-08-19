using UnityEngine;

[CreateAssetMenu(fileName = "New weapon data", menuName = "Weapon/New weapon data", order = 0)]
public class WeaponData : ScriptableObject
{
    [SerializeField] private float damage;
    [SerializeField] private int magazineSize;
    [SerializeField] private int startingAmmo;
    [SerializeField] private float shootRange;
    [SerializeField] private float throwForce;
    [SerializeField] private float reloadTime;
    [SerializeField] private LayerMask mask;

    public float Damage => damage;
    public int MagazineSize => magazineSize;
    public int StartingAmmo => startingAmmo;
    public float ShootRange => shootRange;
    public float ThrowForce => throwForce;
    public float ReloadTime => reloadTime;
    public LayerMask Mask => mask;
}