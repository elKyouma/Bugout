using UnityEngine;
using System.Collections;

public enum ItemType { None, Beer, Balloon, Dynamite, Knife, Key };
public class Item
{
    public ItemType type { get; set; }
    public Sprite UiImage { get; set; }

    public Item(ItemType type, Sprite UiImage) 
    {
        this.type = type;
        this.UiImage = UiImage;
    }
}