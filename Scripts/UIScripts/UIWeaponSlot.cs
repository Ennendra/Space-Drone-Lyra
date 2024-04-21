using Godot;
using System;

public partial class UIWeaponSlot : Container
{
	//the weapon this UI element is linked to
	public WeaponParent weapon;

	public TextureRect weaponIcon, weaponEmptyIcon;
	public Label weaponAmmo;
	public TextureProgressBar weaponProgressBar;
	public TextureRect[] weaponLevelPips = new TextureRect[8];

	public Texture2D levelPipOff, levelPipOn, levelPipRedOff, levelPipRedOn;

    public GlobalScript globals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        globals = GetNode<GlobalScript>("/root/Globals");
        weaponIcon = GetNode<TextureRect>("Container/UIWeapon/UIWeaponIcon");
        weaponEmptyIcon = GetNode<TextureRect>("Container/UIWeapon/UIWeaponEmpty");
        weaponAmmo = GetNode<Label>("Container/UIWeapon/UIWeaponAmmo");
		weaponProgressBar = GetNode<TextureProgressBar>("Container/UIWeapon/UIReloadProgress");

		for (int i = 0; i < 8; i++)
		{
			weaponLevelPips[i] = GetNode<TextureRect>("Container/UIWeaponLevel/PipContainer/LevelPip"+(i+1).ToString());
		}

		levelPipOff = (Texture2D)GD.Load("res://Textures/UI/UIWeaponLevelPipOff.png");
        levelPipOn = (Texture2D)GD.Load("res://Textures/UI/UIWeaponLevelPipOn.png");
        levelPipRedOff = (Texture2D)GD.Load("res://Textures/UI/UIWeaponLevelRedPipOff.png");
        levelPipRedOn = (Texture2D)GD.Load("res://Textures/UI/UIWeaponLevelRedPipOn.png");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (weapon != null)
		{
			//process the reload progress bar tint and value
			double reloadPercent = (weapon.reloadTimer / weapon.GetReloadTime) * 100;
			weaponProgressBar.Value = reloadPercent;
			float progressAlpha = (float)(reloadPercent / 100) * 5.0f;
			if (progressAlpha > 1.0f) progressAlpha = 1.0f;
			weaponProgressBar.TintProgress = new Color(0,1,0.5f,progressAlpha);


			//process the ammo count display
			//Check if the ammo count for the weapon is under 10 and add a 0 to the beginning if maxammo is 10 or over (e.g. ammo = "02/20" instead of "2/20)
			if (weapon.ammo < 10)
			{
				if (weapon.maxAmmo < 10)
				{
                    weaponAmmo.Text = weapon.ammo + "/" + weapon.maxAmmo;
                }
				else
				{
                    weaponAmmo.Text = "0"+ weapon.ammo + "/" + weapon.maxAmmo;
                }
			}
			else
			{
                weaponAmmo.Text = weapon.ammo + "/" + weapon.maxAmmo;
            }

            for (int i = 0; i < weapon.maxWeaponLevel; i++)
            {
                if (IsWeaponAtCurrentLevel(i + 1))
                { weaponLevelPips[i].Texture = levelPipOn; }
                else
                { weaponLevelPips[i].Texture = levelPipOff; }
            }

        }
		
		
	}

	//run this function when a weapon has been added
	public void InitiateWeaponUI(WeaponParent newWeapon)
	{
		//apply the new weapon
		weapon = newWeapon;

		//remove the "locked" symbol
		weaponEmptyIcon.Visible= false;

		//apply the weapon icon and make it visible
		weaponIcon.Texture = weapon.weaponIcon;
		weaponIcon.Visible = true;

		//Set the weapon ammo counter and make it visible
        weaponAmmo.Text = weapon.ammo + "/" + weapon.maxAmmo;
		weaponAmmo.Visible = true;

		//set the weapon texture to the first (or higher) level
        //weaponLevelCounter.Texture = globals.UILevelTexture8[weapon.weaponLevel];

		
		for (int i=0;i < 8; i++)
		{
			if ((i + 1) > weapon.maxWeaponLevel)
			{
				CallDeferred("RemoveWeaponPipTexture", i);
			}
			else
			{
				if (IsWeaponAtCurrentLevel(i+1))
					{weaponLevelPips[i].Texture = levelPipOn;}
				else
					{weaponLevelPips[i].Texture = levelPipOff;}
			}
		}
    }

	public bool IsWeaponAtCurrentLevel(int level)
	{
		return (weapon.weaponLevel >= level);
	}

	public void RemoveWeaponPipTexture(int index)
	{
		weaponLevelPips[index].QueueFree();
	}
}
