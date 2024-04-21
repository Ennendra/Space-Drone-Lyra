using Godot;
using System;

public partial class ItemParent : Node
{

	//tracks the item level
	public int itemLevel = 0;
	[Export] public int maxItemLevel = 5;
    public string[] itemLevelUpDescriptions;

    //displays for the item
    [Export] public Texture2D itemIcon;
    [Export] public string itemName, itemDescription;

    internal GlobalScript globals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//get the globalscript to interact with the modifier stacks
        globals = GetNode<GlobalScript>("/root/Globals");

        //generate the array for item level up descriptions (adding +1 to the max so we can leave the '0' array blank and access the others without going '-1' on everything)
        itemLevelUpDescriptions = new string[maxItemLevel + 1];
    }

	//add a level to the item. Dictate how it will affect modifiers in each child node
	public virtual void LevelUpItem()
	{
		itemLevel++;
	}
}
