using Godot;
using System;

public partial class WeaponSMG : WeaponParent
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

        weaponLevelUpDescriptions[2] = "Increase ammo capacity by 5";
        weaponLevelUpDescriptions[3] = "Increase base damage by 1";
        weaponLevelUpDescriptions[4] = "Increase ammo capacity by 5";
        weaponLevelUpDescriptions[5] = "Increase base damage by 1";
        weaponLevelUpDescriptions[6] = "Increase rate of fire by 25%";
        weaponLevelUpDescriptions[7] = "Increase base damage by 1";
        weaponLevelUpDescriptions[8] = "Effectively nullify reload time";

    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

		switch (weaponLevel)
		{
			case 2:
				{
                    maxAmmo += 5;
					break;
				}
            case 3:
                {
                    damage += 1;
                    break;
                }
            case 4:
                {
                    maxAmmo += 5;
                    break;
                }
            case 5:
                {
                    damage += 1;
                    break;
                }
            case 6:
                {
                    rateOfFire = rateOfFire * 1.25;
                    break;
                }
            case 7:
                {
                    damage += 1;
                    break;
                }
            case 8:
                {
                    reloadTime = 0.06;
                    break;
                }
        }

	}

}
