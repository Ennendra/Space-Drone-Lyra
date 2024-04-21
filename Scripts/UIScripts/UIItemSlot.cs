using Godot;
using System;

public partial class UIItemSlot : Container
{
    //the item this UI element is linked to
    public ItemParent item;
    //the child texture nodes
    public TextureRect itemIcon, itemEmptyIcon;

    public TextureRect[] itemLevelPips = new TextureRect[5];

    public Texture2D levelPipOff, levelPipOn, levelPipRedOff, levelPipRedOn;

    public GlobalScript globals;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        globals = GetNode<GlobalScript>("/root/Globals");

        itemIcon = GetNode<TextureRect>("Container/UIItemBackground/UIItemIcon");
        itemEmptyIcon = GetNode<TextureRect>("Container/UIItemBackground/UIItemEmpty");

        for (int i = 0; i < 5; i++)
        {
            itemLevelPips[i] = GetNode<TextureRect>("Container/UIItemLevel/PipContainer/LevelPip" + (i + 1).ToString());
        }

        levelPipOff = (Texture2D)GD.Load("res://Textures/UI/UIItemLevelPipOff.png");
        levelPipOn = (Texture2D)GD.Load("res://Textures/UI/UIItemLevelPipOn.png");
        levelPipRedOff = (Texture2D)GD.Load("res://Textures/UI/UIItemLevelRedPipOff.png");
        levelPipRedOn = (Texture2D)GD.Load("res://Textures/UI/UIItemLevelRedPipOn.png");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (item != null)
        {
            for (int i = 0; i < item.maxItemLevel; i++)
            {
                if (IsItemAtCurrentLevel(i + 1))
                { itemLevelPips[i].Texture = levelPipOn; }
                else
                { itemLevelPips[i].Texture = levelPipOff; }
            }
        }
	}

    public void InitiateItemUI(ItemParent newItem)
    {
        //apply the new item
        item = newItem;

        //remove the "locked" symbol
        itemEmptyIcon.Visible = false;

        //apply the weapon icon and make it visible
        itemIcon.Texture = item.itemIcon;
        itemIcon.Visible = true;

        for (int i = 0; i < 5; i++)
        {
            if ((i + 1) > item.maxItemLevel)
            {
                CallDeferred("RemoveItemPipTexture", i);
            }
            else
            {
                if (IsItemAtCurrentLevel(i + 1))
                { itemLevelPips[i].Texture = levelPipOn; }
                else
                { itemLevelPips[i].Texture = levelPipOff; }
            }
        }
    }


    public bool IsItemAtCurrentLevel(int level)
    {
        return (item.itemLevel >= level);
    }

    public void RemoveItemPipTexture(int index)
    {
        itemLevelPips[index].QueueFree();
    }
}
