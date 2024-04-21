using Godot;
using System;

public partial class ItemExperienceMagnet : ItemParent
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        //add what will be displayed in the level up list
        itemLevelUpDescriptions[1] = "Increases experience magnetisation range by 20%";
        itemLevelUpDescriptions[2] = "Increases experience magnetisation range by 20%";
        itemLevelUpDescriptions[3] = "Increases experience magnetisation range by 20%";
        itemLevelUpDescriptions[4] = "Increases experience magnetisation range by 20%";
        itemLevelUpDescriptions[5] = "Increases experience magnetisation range by 20%";
    }

    //What changes when the item levels up
    public override void LevelUpItem()
    {
        base.LevelUpItem();

        switch (itemLevel)
        {
            case 1:
                {
                    globals.ChangeExperienceMagnetScaleModifier(0.2f);
                    break;
                }
            case 2:
                {
                    globals.ChangeExperienceMagnetScaleModifier(0.2f);
                    break;
                }
            case 3:
                {
                    globals.ChangeExperienceMagnetScaleModifier(0.2f);
                    break;
                }
            case 4:
                {
                    globals.ChangeExperienceMagnetScaleModifier(0.2f);
                    break;
                }
            case 5:
                {
                    globals.ChangeExperienceMagnetScaleModifier(0.2f);
                    break;
                }
        }
    }

}
