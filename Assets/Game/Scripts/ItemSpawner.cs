using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [Serializable]
    public struct AmmoSlot
    {
        public WeaponType type;
        public GameObject prefab;
    }

    [SerializeField] private List<AmmoSlot> ammoSlots = new();
    [SerializeField] private float spawnCoolDown = 1f;

    private GameObject currentItem;

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
        while (true)
        {
            if (currentItem == null && ammoSlots.Count > 0)
            {
                var index = Random.Range(0, ammoSlots.Count);
                currentItem = Instantiate(ammoSlots[index].prefab, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnCoolDown);
        }
    }
}