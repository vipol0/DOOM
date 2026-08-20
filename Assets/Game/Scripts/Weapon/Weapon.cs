using System.Collections;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected Animator animator;

    private int magazineSize;
    private int currentAmmo;
    private int reserveAmmo;

    private float shootRange;
    private float shootCooldown;
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
    private bool canShoot = true;

    public event Action<int, int, int, bool> AmmoChanged;

    private void Awake()
    {
        currentAmmo = weaponData.StartingAmmo;
        magazineSize = weaponData.MagazineSize;
        shootRange = weaponData.ShootRange;
        shootCooldown = weaponData.ShootCooldown;
        damage = weaponData.Damage;
        reloadTime = weaponData.ReloadTime;
        throwForce = weaponData.ThrowForce;
        mask = weaponData.Mask;
        
        weaponCollider = GetComponent<Collider>();
        weaponRigidBody =  GetComponent<Rigidbody>();

        playerCamera = Camera.main;
    }

    public virtual void OnShoot()
    {
        if (isReloading || !isHeld || currentAmmo <= 0 || !canShoot) return;

        canShoot = false;
        currentAmmo--;
        
        AmmoChanged?.Invoke(currentAmmo, magazineSize, reserveAmmo, isReloading);
        
        animator.SetTrigger("Shoot");

        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out var hit, shootRange, mask))
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage);
        
        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot =  true;
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
        AmmoChanged?.Invoke(currentAmmo, magazineSize, reserveAmmo, isReloading);

        yield return new WaitForSeconds(reloadTime);

        var neededAmmo = magazineSize - currentAmmo;
        var ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        AmmoChanged?.Invoke(currentAmmo, magazineSize, reserveAmmo, isReloading);
    }

    public void GetAmmo(int newAmmo)
    {
        reserveAmmo = newAmmo;
        AmmoChanged?.Invoke(currentAmmo, magazineSize, reserveAmmo, isReloading);
    }

    public void OnGetWeapon(Transform weaponHolder)
    {
        if (weaponRigidBody == null || weaponCollider == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Not weaponRigidBody or weaponCollider");
            return;
        }
        
        isHeld = true;
        AmmoChanged?.Invoke(currentAmmo, magazineSize, reserveAmmo, isReloading);

        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        weaponRigidBody.isKinematic = true;
        weaponCollider.enabled = false;
        animator.SetBool("IsHeld", isHeld);
    }

    public void OnDropWeapon()
    {
        if (weaponRigidBody == null || weaponCollider == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Not weaponRigidBody or weaponCollider");
            return;
        }
        
        isHeld = false;
        animator.SetBool("IsHeld", isHeld);

        transform.SetParent(null);
        transform.rotation = Quaternion.identity;

        weaponRigidBody.isKinematic = false;
        weaponCollider.enabled = true;
        weaponRigidBody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
    }
}