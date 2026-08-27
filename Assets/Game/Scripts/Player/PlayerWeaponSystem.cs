using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSystem : MonoBehaviour
{
    [Serializable]
    public struct AmmoSlot
    {
        public WeaponType ammoType;
        public int amount;
    }

    [Header("Inventory Settings")] [SerializeField]
    private List<AmmoSlot> initialAmmoList = new();

    [Header("Weapon System Setup")] [SerializeField]
    private LayerMask pickupMask;

    [SerializeField] private LayerMask weaponMask;
    [SerializeField] private LayerMask handMask;
    [SerializeField] private float pickupRange = 5;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AmmoText ammoText;
    [SerializeField] private Transform weaponHolder;

    private readonly Dictionary<WeaponType, int> ammoInventory = new();
    private Weapon currentWeapon;

    private void Awake()
    {
        foreach (var slot in initialAmmoList) ammoInventory[slot.ammoType] = slot.amount;
    }

    private void Start()
    {
        ammoText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (currentWeapon != null) currentWeapon.AmmoChanged -= AmmoChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) OnRaycast();
        if (Input.GetKeyDown(KeyCode.Q)) OnDropWeapon();
        if (Input.GetKeyDown(KeyCode.R)) OnReload();
        if (Input.GetMouseButtonDown(0)) OnShoot();
    }

    public int GetAmmo(WeaponType type)
    {
        return ammoInventory.TryGetValue(type, out var amount) ? amount : 0;
    }

    public void SetAmmo(WeaponType type, int amount)
    {
        ammoInventory[type] = Mathf.Max(0, amount);
    }

    public void Resupply(WeaponType type, int amount)
    {
        var newAmount = GetAmmo(type) + amount;
        SetAmmo(type, newAmount);

        if (currentWeapon != null && currentWeapon.WeaponType == type) currentWeapon.GetAmmo(newAmount);
    }

    private void OnRaycast()
    {
        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out var hit, pickupRange, pickupMask))
            if (hit.collider.gameObject.CompareTag("Weapon"))
            {
                var newWeapon = hit.collider.gameObject.GetComponent<Weapon>();
                if (currentWeapon == newWeapon) return;

                if (currentWeapon != null) OnDropWeapon();
                OnGetWeapon(newWeapon);
            }
    }

    private void OnGetWeapon(Weapon newWeapon)
    {
        if (newWeapon == null) return;

        currentWeapon = newWeapon;
        SetLayerRecursively(currentWeapon.gameObject, GetLayerIndex(handMask));

        ammoText.gameObject.SetActive(true);
        currentWeapon.AmmoChanged += AmmoChanged;

        var reserve = GetAmmo(currentWeapon.WeaponType);
        currentWeapon.GetAmmo(reserve);
        currentWeapon.OnGetWeapon(weaponHolder);
    }

    private void OnDropWeapon()
    {
        if (currentWeapon != null)
        {
            SetLayerRecursively(currentWeapon.gameObject, GetLayerIndex(weaponMask));
            currentWeapon.OnDropWeapon();
            currentWeapon.AmmoChanged -= AmmoChanged;
        }

        currentWeapon = null;
        ammoText.gameObject.SetActive(false);
    }

    private void OnShoot()
    {
        if (currentWeapon != null) currentWeapon.OnShoot();
    }

    private void OnReload()
    {
        if (currentWeapon != null) currentWeapon.OnReload();
    }

    private void AmmoChanged(int currentAmmo, int maxAmmo, int reserveAmmo, bool isReload)
    {
        if (ammoText == null || currentWeapon == null) return;

        // Сохраняем изменившийся запас для текущего типа патронов
        SetAmmo(currentWeapon.WeaponType, reserveAmmo);
        ammoText.UpdateText(currentAmmo, maxAmmo, reserveAmmo, isReload);
    }

    private int GetLayerIndex(LayerMask mask)
    {
        return Mathf.RoundToInt(Mathf.Log(mask.value, 2));
    }

    private void SetLayerRecursively(GameObject target, int layerIndex)
    {
        if (target == null) return;

        target.layer = layerIndex;
        foreach (Transform child in target.transform) SetLayerRecursively(child.gameObject, layerIndex);
    }
}