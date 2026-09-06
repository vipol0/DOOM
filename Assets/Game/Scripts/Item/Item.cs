using UnityEngine;
using System;

public abstract class Item : MonoBehaviour
{
    public event Action GetItem;

    public void OnGetItem()
    {
        GetItem?.Invoke();
    }
}
