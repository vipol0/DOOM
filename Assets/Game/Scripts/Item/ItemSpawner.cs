using System;
using System.Collections;
using UnityEngine;

public class ItemSpawner : BaseMonoBehaviour
{
    [SerializeField] private ItemSystem itemSystem;
    [SerializeField] private GameObject akimboAmmoBoxPrefab;
    [SerializeField] private GameObject medkitPrefab;
    [SerializeField] private float spawnCooldown = 1f;

    private GameObject currentItem;

    private void Awake()
    {
        ValidateReference(akimboAmmoBoxPrefab, nameof(akimboAmmoBoxPrefab));
        ValidateReference(medkitPrefab, nameof(medkitPrefab));
        ValidateReference(itemSystem, nameof(itemSystem));
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnRoutine());
        ValidateReference(itemSystem, nameof(itemSystem));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        if (itemSystem == null || akimboAmmoBoxPrefab == null || medkitPrefab == null) yield break;

        while (true)
        {
            yield return new WaitForSeconds(spawnCooldown);

            if (currentItem != null) continue;

            var valueItem = itemSystem.GetItemToSpawn();

            switch (valueItem)
            {
                case ItemType.Medkit:
                    SpawnMedkit();
                    break;
                case ItemType.AkimboAmmo:
                    SpawnAmmoBoxAkimbo();
                    break;
            }
        }
    }

    private void SpawnMedkit()
    {
        currentItem = Instantiate(medkitPrefab, transform.position, Quaternion.identity);
        currentItem.GetComponent<Item>().GetItem += OnGetItem;
    }

    private void SpawnAmmoBoxAkimbo()
    {
        currentItem = Instantiate(akimboAmmoBoxPrefab, transform.position, Quaternion.identity);
        currentItem.GetComponent<Item>().GetItem += OnGetItem;
    }

    private void OnGetItem()
    {
        currentItem.GetComponent<Item>().GetItem -= OnGetItem;
        currentItem = null;
    }
}