using Godot;
using System;

public partial class ItemHealthRegen : ItemParent
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        //add what will be displayed in the level up list
        itemLevelUpDescriptions[1] = "Increase health regeneration by 0.2 per second";
        itemLevelUpDescriptions[2] = "Increase health regeneration by 0.2 per second";
        itemLevelUpDescriptions[3] = "Increase health regeneration by 0.2 per second";
        itemLevelUpDescriptions[4] = "Increase health regeneration by 0.2 per second";
        itemLevelUpDescriptions[5] = "Increase health regeneration by 0.2 per second";
    }

    //What changes when the item levels up
    public override void LevelUpItem()
    {
        base.LevelUpItem();

        switch (itemLevel)
        {
            case 1:
                {
                    globals.ChangeHPRegenAmount(0.2);
                    break;
                }
            case 2:
                {
                    globals.ChangeHPRegenAmount(0.2);
                    break;
                }
            case 3:
                {
                    globals.ChangeHPRegenAmount(0.2);
                    break;
                }
            case 4:
                {
                    globals.ChangeHPRegenAmount(0.2);
                    break;
                }
            case 5:
                {
                    globals.ChangeHPRegenAmount(0.2);
                    break;
                }
        }
    }

}
