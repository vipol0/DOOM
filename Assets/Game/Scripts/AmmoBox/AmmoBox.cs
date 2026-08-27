using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [SerializeField] private int ammo = 5;
    [SerializeField] private WeaponType weaponType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerWeaponSystem>()?.Resupply(weaponType, ammo);
            Destroy(gameObject);
        }
    }
}
