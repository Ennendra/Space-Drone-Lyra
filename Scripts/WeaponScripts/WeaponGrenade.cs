using Godot;
using System;

public partial class WeaponGrenade : WeaponParent
{

    [Export] internal PackedScene maxLevelProjectileToSpawn;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        weaponLevelUpDescriptions[2] = "Increase detonate area by 20%";
        weaponLevelUpDescriptions[3] = "Increase base damage by 5";
        weaponLevelUpDescriptions[4] = "Increase ammo capacity by 1";
        weaponLevelUpDescriptions[5] = "Increase range by 40%";
        weaponLevelUpDescriptions[6] = "Increase detonate area by 20%";
        weaponLevelUpDescriptions[7] = "Increase base damage by 5";
        weaponLevelUpDescriptions[8] = "Launch a cluster of projectiles on detonation, dealing 50% damage each";

    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        switch (weaponLevel)
        {
            case 2:
                {
                    scaleModifier += 0.2f;
                    break;
                }
            case 3:
                {
                    damage += 5;
                    break;
                }
            case 4:
                {
                    maxAmmo += 1;
                    break;
                }
            case 5:
                {
                    distanceToFire = distanceToFire * 1.4f;
                    break;
                }
            case 6:
                {
                    scaleModifier += 0.2f;
                    break;
                }
            case 7:
                {
                    damage += 5;
                    break;
                }
            case 8:
                {
                    projectileToSpawn = maxLevelProjectileToSpawn;
                    break;
                }
        }

    }

    public override ProjectileParent CreateProjectile(Vector2 tempPos, float tempRot)
    {
        ProjectileParent newProjectile = base.CreateProjectile(tempPos, tempRot);

        Vector2 targetPos = currentTarget.GlobalPosition - new Vector2(20,20) + new Vector2(40*GD.Randf(), 40 * GD.Randf());
        newProjectile.speed = tempPos.DistanceTo(targetPos) / 0.3f;

        return newProjectile;
    }

}
