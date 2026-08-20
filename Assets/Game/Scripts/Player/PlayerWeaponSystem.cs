using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeaponSystem : MonoBehaviour
{
    [SerializeField] private LayerMask pickupMask;
    [SerializeField] private LayerMask weaponMask;
    [SerializeField] private LayerMask handMask;
    [SerializeField] private float pickupRange = 5;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AmmoText ammoText;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private int currentReserveAmmo = 10;

    private RaycastHit hit;
    private Weapon currentWeapon;

    private void Start()
    {
        ammoText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (weaponHolder != null) currentWeapon.AmmoChanged -= AmmoChanged;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E)) OnRaycast();

        if (Input.GetKey(KeyCode.Q)) OnDropWeapon();

        if (Input.GetKey(KeyCode.R)) OnReload();

        if (Input.GetMouseButtonDown(0)) OnShoot();
    }

    private void OnRaycast()
    {
        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out var hit, pickupRange, pickupMask))
            if (hit.collider.gameObject.CompareTag("Weapon"))
            {
                var newWeapon = hit.collider.gameObject.GetComponent<Weapon>();

                if (currentWeapon == newWeapon) return;

                if (currentWeapon == null)
                {
                    OnGetWeapon(newWeapon);
                }
                else
                {
                    OnDropWeapon();
                    OnGetWeapon(newWeapon);
                }
            }
    }
    
    private int GetLayerIndex(LayerMask mask)
    {
        return Mathf.RoundToInt(Mathf.Log(mask.value, 2));
    }
    
    private void SetLayerRecursively(GameObject target, int layerIndex)
    {
        if (target == null) return;
    
        target.layer = layerIndex;
        foreach (Transform child in target.transform)
        {
            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }

    private void OnGetWeapon(Weapon newWeapon)
    {
        if (newWeapon == null) return;

        currentWeapon = newWeapon;
        SetLayerRecursively(currentWeapon.gameObject, GetLayerIndex(handMask));

        ammoText.gameObject.SetActive(true);
        if (weaponHolder != null) currentWeapon.AmmoChanged += AmmoChanged;

        currentWeapon.GetAmmo(currentReserveAmmo);
        currentWeapon.OnGetWeapon(weaponHolder);
    }

    private void OnDropWeapon()
    {
        if (currentWeapon != null)
        {
            SetLayerRecursively(currentWeapon.gameObject, GetLayerIndex(weaponMask));
            currentWeapon.OnDropWeapon();
        }

        if (weaponHolder != null) currentWeapon.AmmoChanged -= AmmoChanged;

        currentWeapon = null;
        ammoText.gameObject.SetActive(false);
    }

    private void OnShoot()
    {
        if (currentWeapon == null) return;
        currentWeapon.OnShoot();
    }

    private void OnReload()
    {
        if (currentWeapon == null) return;
        currentWeapon.OnReload();
    }

    private void AmmoChanged(int currentAmmo, int maxAmmo, int reserveAmmo, bool isReload)
    {
        if (ammoText == null || currentWeapon == null) return;

        currentReserveAmmo = reserveAmmo;
        ammoText.UpdateText(currentAmmo, maxAmmo, reserveAmmo, isReload);
    }
}