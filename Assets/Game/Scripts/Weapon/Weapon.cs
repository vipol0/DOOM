using System;
using System.Collections;
using UnityEngine;

public class Weapon : BaseMonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Billboard billboard;
    [SerializeField] protected AudioSource audioSource;

    [Header("SFX")] [SerializeField] protected AudioClip clipShoot;
    [SerializeField] protected AudioClip clipReload;

    private static readonly int IsHeldHash = Animator.StringToHash("IsHeld");

    protected int currentAmmo;
    private int reserveAmmo;
    private int magazineSize;

    protected Camera playerCamera;
    protected Collider weaponCollider;
    protected Rigidbody weaponRigidBody;

    protected bool isHeld;
    private bool isReloading;
    private bool canShoot = true;

    public event Action OnGetingWeapon;

    public WeaponType WeaponType => weaponData != null
        ? weaponData.WeaponType
        : default;

    public event Action<int, int, int, bool> AmmoChanged;

    protected virtual void Awake()
    {
        playerCamera = Camera.main;
        weaponCollider = GetComponent<Collider>();
        weaponRigidBody = GetComponent<Rigidbody>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (!ValidateReference(weaponData, nameof(weaponData)) ||
            !ValidateReference(animator, nameof(animator)) ||
            !ValidateReference(playerCamera, nameof(playerCamera)) ||
            !ValidateReference(weaponRigidBody, nameof(weaponRigidBody)) ||
            !ValidateReference(weaponCollider, nameof(weaponCollider)) ||
            !ValidateReference(billboard, nameof(billboard)) ||
            !ValidateReference(audioSource, nameof(audioSource)))
            return;

        currentAmmo = weaponData.StartingAmmo;
        magazineSize = weaponData.MagazineSize;
    }

    public virtual void OnShoot()
    {
        if (!CanShoot())
            return;

        if (currentAmmo <= 0)
        {
            OnReload();
            return;
        }

        Shoot();
    }

    protected virtual void Shoot()
    {
        canShoot = false;
        currentAmmo--;

        if (audioSource != null)
            audioSource.PlayOneShot(clipShoot);

        NotifyAmmoChanged();

        var ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(
                ray,
                out var hit,
                weaponData.ShootRange,
                weaponData.Mask))
            if (hit.collider.TryGetComponent<IDamagable>(out var damagable))
                damagable.TakeDamage(weaponData.Damage);

        StartCoroutine(ShootCooldown());
    }

    protected bool CanShoot()
    {
        return !isReloading &&
               isHeld &&
               canShoot &&
               playerCamera != null;
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(weaponData.ShootCooldown);
        canShoot = true;
    }

    public void OnReload()
    {
        if (currentAmmo >= weaponData.MagazineSize ||
            reserveAmmo <= 0 ||
            isReloading ||
            !isHeld)
            return;

        StartCoroutine(Reloading());
    }

    private IEnumerator Reloading()
    {
        isReloading = true;

        if (audioSource != null)
            audioSource.PlayOneShot(clipReload);

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
        if (weaponRigidBody == null ||
            weaponCollider == null ||
            billboard == null)
            return;

        isHeld = true;

        OnGetingWeapon?.Invoke();

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
        if (weaponRigidBody == null ||
            weaponCollider == null ||
            billboard == null)
            return;

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
            weaponRigidBody.AddForce(
                playerCamera.transform.forward * weaponData.ThrowForce,
                ForceMode.Impulse
            );
    }

    protected void NotifyAmmoChanged()
    {
        AmmoChanged?.Invoke(
            currentAmmo,
            magazineSize,
            reserveAmmo,
            isReloading
        );
    }
}