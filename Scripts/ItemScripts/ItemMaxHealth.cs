using Godot;
using System;

public partial class ItemMaxHealth : ItemParent
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        //add what will be displayed in the level up list
        itemLevelUpDescriptions[1] = "Increase maximum health by 20%";
        itemLevelUpDescriptions[2] = "Increase maximum health by 20%";
        itemLevelUpDescriptions[3] = "Increase maximum health by 20%";
        itemLevelUpDescriptions[4] = "Increase maximum health by 20%";
        itemLevelUpDescriptions[5] = "Increase maximum health by 20%";
    }

    //What changes when the item levels up
    public override void LevelUpItem()
    {
        base.LevelUpItem();

        switch (itemLevel)
        {
            case 1:
                {
                    globals.ChangeMaxHPAmount(0.2);
                    break;
                }
            case 2:
                {
                    globals.ChangeMaxHPAmount(0.2);
                    break;
                }
            case 3:
                {
                    globals.ChangeMaxHPAmount(0.2);
                    break;
                }
            case 4:
                {
                    globals.ChangeMaxHPAmount(0.2);
                    break;
                }
            case 5:
                {
                    globals.ChangeMaxHPAmount(0.2);
                    break;
                }
        }
    }
}
