using Godot;
using System;

public partial class HurtBoxComponent : Area2D
{
	[Export] HealthComponent healthComponent;

    //used to check whether the player has an ADS attached.
    public WeaponADS ADSLink;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SetADSLink(WeaponADS link)
    {
        ADSLink = link;
    }



    public double TakeDamage(double damageTaken, DamageType damageType)
    {
        double finalDamage = 0;

            finalDamage = damageTaken;

            switch (damageType)
            {
                case DamageType.FLAT:
                    {
                        break;
                    }
                case DamageType.PERCENTHP:
                    {
                        finalDamage = (damageTaken / 100) * healthComponent.health;
                        break;
                    }
                case DamageType.PERCENTMAXHP:
                    {
                        finalDamage = (damageTaken / 100) * healthComponent.maxHealth;
                        break;
                    }
            }

        healthComponent.TakeDamage(finalDamage);

        return finalDamage;
    }
}
