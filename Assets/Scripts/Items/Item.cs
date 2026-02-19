using UnityEngine;
using System.Collections;

public enum ItemType { None, Beer, Balloon, Dynamite, Knife, Key }; // WHEN ADDING NEW ITEM TYPES, MAKE SURE TO UPDATE THE ITEMTYPE ENUM INSIDE VDG
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