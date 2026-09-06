using UnityEngine;

public class AmmoBox : Item
{
    [SerializeField] private int ammo = 5;
    [SerializeField] private WeaponType weaponType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<PlayerWeaponSystem>()?.Resupply(weaponType, ammo);
        OnGetItem();
        Destroy(gameObject);
    }
}