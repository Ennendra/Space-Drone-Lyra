using Godot;
using System;

public partial class WeaponADS : WeaponParent
{
    [Signal] public delegate void LinkToHurtboxEventHandler(HurtBoxComponent component);

    public override void _Ready()
    {
        base._Ready();

        weaponLevelUpDescriptions[2] = "Increase ammo capacity by 1";
        weaponLevelUpDescriptions[3] = "Increase fire rate by 25%";
        weaponLevelUpDescriptions[4] = "Increase ammo capacity by 1";
        weaponLevelUpDescriptions[5] = "Decrease reload time by 1 second";
        //weaponLevelUpDescriptions[6] = "";
        //weaponLevelUpDescriptions[7] = "";
        //weaponLevelUpDescriptions[8] = "";

        LinkToHurtbox += LinkADSToHurtbox;
    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        switch (weaponLevel)
        {
            case 2:
                {
                    maxAmmo++;
                    break;
                }
            case 3:
                {
                    rateOfFire = rateOfFire * 1.25;
                    break;
                }
            case 4:
                {
                    maxAmmo++;
                    break;
                }
            case 5:
                {
                    reloadTime -= 1;
                    break;
                }
            case 6:
                {
                    
                    break;
                }
            case 7:
                {
                    
                    break;
                }
            case 8:
                {
                    
                    break;
                }
        }

    }

    //override the weapon process so it isn't "attempting to fire"
    public override void _Process(double delta)
    {
        //increment timers
        reloadTimer += delta;
        fireTimer += delta;
        //reload weapon if the timer has been reached
        if (reloadTimer >= GetReloadTime)
        {
            ammo = maxAmmo;
        }
    }

    public bool CanADSFire()
    {
        //stop attempting to fire if the weapon is disabled (ie, player dead)
        if (!isActive) return false;
        //check if we havent fired early before and that we have ammo remaining
        if (fireTimer >= GetTimeToFire && ammo > 0)
        {
            return true;
        }
        return false;
    }

    public void FireADSWeapon(Node2D target)
    {
        //reset timers and drop down ammo
        fireTimer = 0;
        reloadTimer = 0;
        ammo--;
        //get the angle to the target
        float angleToTarget = GlobalPosition.DirectionTo(target.GlobalPosition).Angle();
        target.EmitSignal("emitADSParticle");
        //play the weapon sound
        audioWeaponFire.Play();
    }

    public void LinkADSToHurtbox(HurtBoxComponent component)
    {
        component.ADSLink = this;
    }
}
