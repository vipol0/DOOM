using UnityEngine;

public class ItemHealth : Health
{
    [SerializeField] private Item item;

    protected override void Died()
    {
        item.OnGetItem();
        base.Died();
    }
}
