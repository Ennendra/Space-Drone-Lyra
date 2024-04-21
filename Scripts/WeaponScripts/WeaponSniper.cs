using Godot;
using System;

public partial class WeaponSniper : WeaponParent
{

    double initialRateOfFire;
    // Called when the node enters the scene tree for the first time.

    AudioStreamPlayer2D audioWeaponFireUpgraded;

    PackedScene upgradedProjectile;

	public override void _Ready()
	{
		base._Ready();

        audioWeaponFireUpgraded = GetNode<AudioStreamPlayer2D>("AudioWeaponFireUpgraded");

        upgradedProjectile = (PackedScene)ResourceLoader.Load("res://Objects/Projectiles/ProjectileSniperUpgraded.tscn");

        weaponLevelUpDescriptions[2] = "Increase rate of fire by 25%";
        weaponLevelUpDescriptions[3] = "Increase ammo count by 3";
        weaponLevelUpDescriptions[4] = "Increase base damage by 15";
        weaponLevelUpDescriptions[5] = "Reduce reload time by 1 second";
        weaponLevelUpDescriptions[6] = "Increase rate of fire by an additional 25%";
        weaponLevelUpDescriptions[7] = "Increase base damage by 15";
        //weaponLevelUpDescriptions[8] = "Change the target type and damage to be based on MAX HP rather than current HP";
        weaponLevelUpDescriptions[8] = "Weapon now fires a piercing line at the target, dealing damage to all in its path";

        initialRateOfFire = rateOfFire;
    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

		switch (weaponLevel)
		{
			case 2:
				{
                    rateOfFire += initialRateOfFire * 0.25;
					break;
				}
            case 3:
                {
                    maxAmmo += 3;
                    break;
                }
            case 4:
                {
                    damage += 15;
                    break;
                }
            case 5:
                {
                    reloadTime -= 1;
                    break;
                }
            case 6:
                {
                    rateOfFire += initialRateOfFire * 0.25;
                    break;
                }
            case 7:
                {
                    damage += 15;
                    break;
                }
            case 8:
                {
                    projectileToSpawn = upgradedProjectile;
                    audioWeaponFire = audioWeaponFireUpgraded;
                    isDirectHit = false;
                    break;
                }
        }

	}

}
