using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class MainUI : CanvasLayer
{
	public GlobalScript globals;
	public List<UIWeaponSlot> weaponUISlots;
	public List<UIItemSlot> itemUISlots;

	public Node weaponSlotContainer;
	public Node itemSlotContainer;

	public ProgressBar healthBar, experienceBar;
	public Label gameTimer, killLabel;

	public Control BossHealthContainer;
	public ProgressBar bossHealthBar;

	public PackedScene weaponUISlotScene, itemUISlotScene;

    double tMinutes = 0, tSeconds = 0, tSecFraction = 0;

	Color timerTextColor = new Color(1, 1, 1, 1);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		//apply the globalscript object
        globals = GetNode<GlobalScript>("/root/Globals");

		//apply scene variables for the weapon UI
		weaponUISlots = new List<UIWeaponSlot>();
		itemUISlots = new List<UIItemSlot>();

		weaponSlotContainer = GetNode("WeaponContainer/CenterContainer/HContainer");
		weaponUISlotScene = (PackedScene)ResourceLoader.Load("res://Objects/UI/UIWeaponSlot.tscn");

        itemSlotContainer = GetNode("ItemContainer/CenterContainer/VContainer");
        itemUISlotScene = (PackedScene)ResourceLoader.Load("res://Objects/UI/UIItemSlot.tscn");

		healthBar = GetNode<ProgressBar>("HealthExperienceContainer/CenterContainer/VContainer/HContainer/HealthBarBackground/HealthBar");
        experienceBar = GetNode<ProgressBar>("HealthExperienceContainer/CenterContainer/VContainer/HContainer/ExperienceBarBackground/ExperienceBar");
		gameTimer = GetNode<Label>("HealthExperienceContainer/CenterContainer/VContainer/HBox/GameTimer");
        killLabel = GetNode<Label>("HealthExperienceContainer/CenterContainer/VContainer/HBox/KillLabel");

        BossHealthContainer = GetNode<Control>("BossHealthContainer");
		bossHealthBar = GetNode<ProgressBar>("BossHealthContainer/CenterContainer/HContainer/HealthBarBackground/HealthBar");

        //generate a set of the weaponUIslot instances based on the max amount of weapons that can be equipped
        for (int i = 0; i < globals.playerMaxWeapons; i++)
		{
			UIWeaponSlot newSlot = (UIWeaponSlot)weaponUISlotScene.Instantiate();
			weaponSlotContainer.AddChild(newSlot);
			weaponUISlots.Add(newSlot);
		}

		for (int i = 0; i < globals.playerMaxItems; i++)
		{
			UIItemSlot newSlot = (UIItemSlot)itemUISlotScene.Instantiate();
			itemSlotContainer.AddChild(newSlot);
			itemUISlots.Add(newSlot);
		}

    }



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//draw the level timer
		string composition = "";
		if (globals.tMinutes < 10)
		{
			composition = composition + "0";
		}
		composition = composition + globals.tMinutes.ToString() + ":";
		if (globals.tSeconds < 10)
		{
			composition = composition + "0";
		}
		composition = composition + globals.tSeconds.ToString();

		gameTimer.Text = composition;

		killLabel.Text = "K: " + globals.enemiesKilled.ToString() + "  ";
    }
	public void SetGameTimerColor(Color color)
	{
		gameTimer.Modulate = color;
	}

	public void AddWeaponElement(WeaponParent newWeapon)
	{
		for (int i = 0; i < weaponUISlots.Count; i++)
		{
			if (weaponUISlots[i].weapon == null)
			{
				weaponUISlots[i].InitiateWeaponUI(newWeapon);
				return;
			}

		}
	}

	public void AddItemElement(ItemParent newItem)
	{
		for (int i=0; i < itemUISlots.Count; i++)
		{
			if (itemUISlots[i].item == null)
			{
				itemUISlots[i].InitiateItemUI(newItem); 
				return;
			}
		}
	}

	public void SetHealthValue(double health)
	{
		healthBar.Value = health;
	}
	public void SetMaxHealthValue(double maxHealth)
	{
		healthBar.MaxValue = maxHealth;
	}
	public void SetExperienceValue(float experience)
	{
		experienceBar.Value = experience;
	}
	public void SetMaxExperienceValue(float maxExperience)
	{
		experienceBar.MaxValue = maxExperience;
	}

	public void SetBossHealthValue(float health)
	{
		bossHealthBar.Value = health;
	}
	public void SetBossMaxHealthValue(float maxHealth)
	{
		bossHealthBar.MaxValue = maxHealth;
	}

	public void SetBossUIVisibility(bool visibility)
	{
		BossHealthContainer.Visible = visibility;
	}

}
