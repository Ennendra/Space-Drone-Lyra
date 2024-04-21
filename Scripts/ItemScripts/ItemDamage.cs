using Godot;
using System;
public partial class ItemDamage : ItemParent
{
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

        //add what will be displayed in the level up list
        itemLevelUpDescriptions[1] = "Increase damage by 10%";
        itemLevelUpDescriptions[2] = "Increase damage by 10%";
        itemLevelUpDescriptions[3] = "Increase damage by 10%";
        itemLevelUpDescriptions[4] = "Increase damage by 10%";
        itemLevelUpDescriptions[5] = "Increase damage by 10%";
    }

    //What changes when the item levels up
    public override void LevelUpItem()
    {
        base.LevelUpItem();

        switch (itemLevel)
        {
            case 1:
                {
                    globals.ChangeDamageModifier(0.1f);
                    break;
                }
            case 2:
                {
                    globals.ChangeDamageModifier(0.1f);
                    break;
                }
            case 3:
                {
                    globals.ChangeDamageModifier(0.1f);
                    break;
                }
            case 4:
                {
                    globals.ChangeDamageModifier(0.1f);
                    break;
                }
            case 5:
                {
                    globals.ChangeDamageModifier(0.1f);
                    break;
                }
        }
    }

}
