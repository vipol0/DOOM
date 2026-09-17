using System.Collections;
using UnityEngine;

public class ItemSpawner : BaseMonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnCooldown = 1f;

    private GameObject currentItem;

    private void Awake()
    {
        ValidateReference(prefab, nameof(prefab));
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        if (prefab == null) yield break;

        while (true)
        {
            yield return new WaitForSeconds(spawnCooldown);

            if (currentItem != null) continue;
            currentItem = Instantiate(prefab, transform.position, Quaternion.identity);
            currentItem.GetComponent<Item>().GetItem += OnGetItem;
        }
    }

    private void OnGetItem()
    {
        currentItem.GetComponent<Item>().GetItem -= OnGetItem;
        currentItem = null;
    }
}