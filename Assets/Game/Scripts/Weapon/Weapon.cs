using System;
using System.Collections;
using UnityEngine;

public class Weapon : BaseMonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Billboard billboard;
    [SerializeField] protected AudioSource audioSource;
    
    [Header("SFX")]
    [SerializeField] protected AudioClip clipShoot;
    [SerializeField] protected AudioClip clipReload;
    
    private static readonly int IsHeldHash = Animator.StringToHash("IsHeld");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");

    private int currentAmmo;
    private int reserveAmmo;
    private int magazineSize;

    private Camera playerCamera;
    private Collider weaponCollider;
    private Rigidbody weaponRigidBody;

    private bool isHeld;
    private bool isReloading;
    private bool canShoot = true;

    public WeaponType WeaponType => weaponData != null ? weaponData.WeaponType : default;
    public event Action<int, int, int, bool> AmmoChanged;

    private void Awake()
    {
        playerCamera = Camera.main;
        weaponCollider = GetComponent<Collider>();
        weaponRigidBody = GetComponent<Rigidbody>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        if (!ValidateReference(weaponData, nameof(weaponData)) || !ValidateReference(animator, nameof(animator)) || 
            !ValidateReference(playerCamera, nameof(playerCamera)) 
            || !ValidateReference(weaponRigidBody, nameof(weaponRigidBody))
            || !ValidateReference(weaponCollider, nameof(weaponCollider))
            || !ValidateReference(billboard, nameof(billboard))
            || !ValidateReference(audioSource, nameof(audioSource)))
            return;

        currentAmmo = weaponData.StartingAmmo;
        magazineSize = weaponData.MagazineSize;
    }

    public virtual void OnShoot()
    {
        if (isReloading || !isHeld || currentAmmo <= 0 || !canShoot || playerCamera == null) return;

        canShoot = false;
        currentAmmo--;
        if (audioSource != null) audioSource.PlayOneShot(clipShoot);

        NotifyAmmoChanged();
        animator.SetTrigger(ShootHash);

        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out var hit, weaponData.ShootRange, weaponData.Mask))
            if (hit.collider.TryGetComponent<IDamagable>(out var damagable))
                damagable.TakeDamage(weaponData.Damage);

        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(weaponData.ShootCooldown);
        canShoot = true;
    }

    public void OnReload()
    {
        if (currentAmmo >= weaponData.MagazineSize || reserveAmmo <= 0 || isReloading || !isHeld)
            return;

        StartCoroutine(Reloading());
    }

    private IEnumerator Reloading()
    {
        isReloading = true;
        if (audioSource != null) audioSource.PlayOneShot(clipReload);
        NotifyAmmoChanged();

        yield return new WaitForSeconds(weaponData.ReloadTime);

        var neededAmmo = weaponData.MagazineSize - currentAmmo;
        var ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        NotifyAmmoChanged();
    }

    public void GetAmmo(int newAmmo)
    {
        reserveAmmo = newAmmo;
        NotifyAmmoChanged();
    }

    public void OnGetWeapon(Transform weaponHolder)
    {
        if (weaponRigidBody == null || weaponCollider == null || billboard == null) return;

        isHeld = true;
        
        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        weaponRigidBody.isKinematic = true;
        weaponCollider.enabled = false;
        
        billboard.ResetPosition();
        billboard.enabled = false;

        animator.SetBool(IsHeldHash, true);
        NotifyAmmoChanged();
    }

    public void OnDropWeapon()
    {
        if (weaponRigidBody == null || weaponCollider == null || billboard == null) return;

        StopAllCoroutines();
        isReloading = false;
        canShoot = true;
        isHeld = false;

        animator.SetBool(IsHeldHash, false);

        transform.SetParent(null);

        weaponRigidBody.isKinematic = false;
        weaponCollider.enabled = true;
        billboard.enabled = true;

        if (playerCamera != null)
            weaponRigidBody.AddForce(playerCamera.transform.forward * weaponData.ThrowForce, ForceMode.Impulse);
    }

    private void NotifyAmmoChanged()
    {
        AmmoChanged?.Invoke(currentAmmo, magazineSize, reserveAmmo, isReloading);
    }
}