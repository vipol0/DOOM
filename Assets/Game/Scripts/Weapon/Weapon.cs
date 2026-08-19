using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;

    private int magazineSize;
    private int currentAmmo;
    private int reserveAmmo;

    private float shootRange;
    private float damage;
    private float reloadTime;
    private float throwForce;

    private LayerMask mask;
    private RaycastHit hit;

    private Camera playerCamera;
    private Collider weaponCollider;
    private Rigidbody weaponRigidBody;

    private bool isHeld;
    private bool isReloading;

    private void Awake()
    {
        currentAmmo = weaponData.StartingAmmo;
        magazineSize = weaponData.MagazineSize;
        shootRange = weaponData.ShootRange;
        damage = weaponData.Damage;
        reloadTime = weaponData.ReloadTime;
        throwForce = weaponData.ThrowForce;
        mask = weaponData.Mask;
        
        weaponCollider = GetComponent<Collider>();
        weaponRigidBody =  GetComponent<Rigidbody>();

        playerCamera = Camera.main;
    }

    public void OnShoot()
    {
        if (isReloading || !isHeld) return;

        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out var hit, shootRange, mask))
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage);
    }

    public void OnReload()
    {
        if (currentAmmo >= magazineSize || reserveAmmo <= 0 || isReloading || !isHeld)
            return;

        StartCoroutine(Reloading());
    }

    private IEnumerator Reloading()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        var neededAmmo = magazineSize - currentAmmo;
        var ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
    }

    public void GetAmmo(int newAmmo)
    {
        reserveAmmo = newAmmo;
    }

    public void OnGetWeapon(Transform weaponHolder)
    {
        if (weaponRigidBody == null || weaponCollider == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Not weaponRigidBody or weaponCollider");
            return;
        }
        
        isHeld = true;

        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        weaponRigidBody.isKinematic = true;
        weaponCollider.enabled = false;
    }

    public void OnDropWeapon()
    {
        if (weaponRigidBody == null || weaponCollider == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Not weaponRigidBody or weaponCollider");
            return;
        }
        
        isHeld = false;

        transform.SetParent(null);

        weaponRigidBody.isKinematic = false;
        weaponCollider.enabled = true;

        weaponRigidBody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
    }
}