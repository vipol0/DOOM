using UnityEngine;

public class PlayerPickupSystem : MonoBehaviour
{
    [SerializeField] private LayerMask pickupMask;
    [SerializeField] private float pickupRange;
    [SerializeField] private Camera  playerCamera;
    [SerializeField] private Transform weaponHolder;
    
    private RaycastHit hit;
    private Weapon currentWeapon;

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            OnRaycast();
        }
    }

    private void OnRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask))
        {
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
    }

    private void OnGetWeapon(Weapon newWeapon)
    {
        if (newWeapon == null) return;
        
        currentWeapon = newWeapon;
        
        currentWeapon.OnGetWeapon(weaponHolder);
    }

    private void OnDropWeapon()
    {
        if (currentWeapon != null)
        {
            currentWeapon.OnDropWeapon();
        }
        
        

        currentWeapon = null;
    }
}
