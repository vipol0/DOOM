using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSystem : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float minHealth = 90f;

    [Header("Ammo")]
    [SerializeField] private PlayerWeaponSystem weaponSystem;
    [SerializeField] private int minAmmoAkimbo = 15;

    private float CurrentHealth => playerHealth.CurrentHealth;
    private int CurrentAmmoAkimbo => weaponSystem.GetAmmo(WeaponType.Akimbo);

    public ItemType GetItemToSpawn()
    {
        if (CurrentHealth <= minHealth)
        {
            return ItemType.Medkit;
        }
        
        if (CurrentHealth >= playerHealth.MaxHealth)
        {
            return ItemType.AkimboAmmo;
        }
        
        if (CurrentAmmoAkimbo <= minAmmoAkimbo)
        {
            return ItemType.AkimboAmmo;
        }
        
        return (ItemType)Random.Range(1, 3);
    }
}