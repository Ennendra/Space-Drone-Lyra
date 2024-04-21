using Godot;
using System;

public partial class ItemFireRate : ItemParent
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        //add what will be displayed in the level up list
        itemLevelUpDescriptions[1] = "Increase all weapon's rate of fire by 10%";
        itemLevelUpDescriptions[2] = "Increase all weapon's rate of fire by 10%";
        itemLevelUpDescriptions[3] = "Increase all weapon's rate of fire by 10%";
        itemLevelUpDescriptions[4] = "Increase all weapon's rate of fire by 10%";
        itemLevelUpDescriptions[5] = "Increase all weapon's rate of fire by 10%";
    }

    //What changes when the item levels up
    public override void LevelUpItem()
    {
        base.LevelUpItem();

        switch (itemLevel)
        {
            case 1:
                {
                    globals.ChangeFireRateModifier(0.1f);
                    break;
                }
            case 2:
                {
                    globals.ChangeFireRateModifier(0.1f);
                    break;
                }
            case 3:
                {
                    globals.ChangeFireRateModifier(0.1f);
                    break;
                }
            case 4:
                {
                    globals.ChangeFireRateModifier(0.1f);
                    break;
                }
            case 5:
                {
                    globals.ChangeFireRateModifier(0.1f);
                    break;
                }
        }
    }

}
