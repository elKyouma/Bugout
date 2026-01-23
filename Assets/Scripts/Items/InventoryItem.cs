using UnityEngine;

public class InventoryItem : Collectable
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private Sprite UIImage;
    private Item item;

    private void Start()
    {
        item = new Item(itemType, UIImage);
    }
    protected override void Collect()
    {
        if (item.type != ItemType.None) 
        {
            bool success = GameManager.Instance.TryAddItemToInventory(item);
            if (success)
                ObjectDestroy();
        }
    }
}
