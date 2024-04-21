using Godot;
using System;

public partial class WeaponShotgun : WeaponParent
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

        weaponLevelUpDescriptions[2] = "Increases rate of fire to 4 per second";
        weaponLevelUpDescriptions[3] = "Increase ammo capacity by 3";
        weaponLevelUpDescriptions[4] = "Increases base damage by 1";
        weaponLevelUpDescriptions[5] = "Increases the effective range by 33%";
        weaponLevelUpDescriptions[6] = "Increase reload speed by 25%";
        weaponLevelUpDescriptions[7] = "Increases base damage by 1";
        weaponLevelUpDescriptions[8] = "Doubles the projectiles fired and doubles the scatter angle";

    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

		switch (weaponLevel)
		{
			case 2:
				{
                    rateOfFire += 1;
					break;
				}
            case 3:
                {
                    maxAmmo += 3;
                    break;
                }
            case 4:
                {
                    damage += 1;
                    break;
                }
            case 5:
                {
                    distanceToFire += 100;
                    break;
                }
            case 6:
                {
                    reloadTime = reloadTime * 0.75;
                    break;
                }
            case 7:
                {
                    damage += 1;
                    break;
                }
            case 8:
                {
                    projectilesPerShot = projectilesPerShot * 2;
                    weaponScatter = weaponScatter * 2;
                    break;
                }
        }

	}

}
