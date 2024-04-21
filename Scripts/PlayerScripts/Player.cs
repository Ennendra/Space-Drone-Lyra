using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public partial class Player : CharacterBody2D
{

    //Components
    [Export] HurtBoxComponent hurtBoxComponent;
    [Export] HealthComponent healthComponent;

    int maxWeapons, maxItems;

    //Their movement speed
    float moveSpeed = 200;

	//experience variables
	int level = 1;
	float experience = 0;
	float experienceRequired = 20;

    float deadTime = 0;

	//a reference to the magnet object attached to the player
	PlayerMagnet magnet;
	//A reference to the levelup menu that will be attached to the player
	LevelUpMenu levelMenu;
	//A reference to the main UI element
	MainUI mainUI;
	//A reference to the sound player when picking up experience
	AudioStreamPlayer audioExpPickup;

    //A reference to the collider
    CapsuleShape2D collider;

    

    [Export] PackedScene starterWeapon;
	[Export] PackedScene starterItem;

    RayCast2D rayCast;

	//the node2D children to help organise the items and weapons
	public Node2D weaponFolder;
	public Node2D itemFolder;
    List<WeaponParent> weapons = new List<WeaponParent>();

    //a reference to the player sprites (used for mirroring animations)
    AnimatedSprite2D spriteMain;

    List<EnemyParent> enemiesInContact = new List<EnemyParent>();

    public LevelController levelController;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        AddUserSignal("deathCall");
        Connect("deathCall", new Callable(this, "OnDeath"));

        mainUI = (MainUI)GetNode("MainUI");

		audioExpPickup = GetNode<AudioStreamPlayer>("AudioExperiencePickup");

        spriteMain = GetNode<AnimatedSprite2D>("Sprite");

        collider = GetNode<CollisionShape2D>("Collider").Shape as CapsuleShape2D;

        magnet = GetNode<PlayerMagnet>("PlayerMagnet");
		magnet.SetPlayer(this);

        weaponFolder = GetNode<Node2D>("Weapons");
		itemFolder = GetNode<Node2D>("Items");

        rayCast = GetNode<RayCast2D>("RayCast2D");

        mainUI.SetMaxHealthValue(healthComponent.maxHealth);
        mainUI.SetHealthValue(healthComponent.health);
        mainUI.SetMaxExperienceValue(experienceRequired);
        mainUI.SetExperienceValue(experience);

	}

    public void SetLevelUpMenu(LevelUpMenu newMenu)
    {
        levelMenu = newMenu;
    }

    public MainUI GetMainUI()
    {
        return mainUI;
    }

	public List<string> InitiateItems()
	{
        List<string> itemsEquipped = new List<string>();

        //Create the instances of the starter weapon and item
        if (starterWeapon != null)
        {
            WeaponParent newWeapon = (WeaponParent)starterWeapon.Instantiate();
            weaponFolder.AddChild(newWeapon);
            itemsEquipped.Add(newWeapon.weaponName);
            AddNewWeaponToList(newWeapon);
        }
        if (starterItem != null)
        {
            WeaponParent newItem = (WeaponParent)starterItem.Instantiate();
            itemFolder.AddChild(newItem);
            itemsEquipped.Add(newItem.weaponName);
        }

        return itemsEquipped;
    }

    public void AddNewWeaponToList(WeaponParent weapon)
    {
        weapons.Add(weapon);
        weapon.lControl = levelController;
    }
    public void SetWeaponActivity(bool isActive)
    {
        foreach (WeaponParent weapon in weapons)
        {
            weapon.isActive = isActive;
        }
    }

	public void SetMaxEquipmentValues(int mWeapon, int mItem)
	{
		maxWeapons = mWeapon;
		maxItems = mItem;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        mainUI.SetMaxHealthValue(healthComponent.maxHealth);
        mainUI.SetHealthValue(healthComponent.health);

        //check if the player isn't dead
        if (!healthComponent.isDead )
        {
            //process experience
            if (experience > experienceRequired)
            {
                LevelUp();
            }
        }
        /*else //we are dead
        {
            deadTime += (float)delta;
            if (deadTime > 4.0f)
            {
                GetTree().ReloadCurrentScene();
            }
        }*/


        
    }



    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!healthComponent.isDead)
        {
            //get the vector from the keyboard input and apply it to movement
            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
            //set the movement velocity
            Velocity = input * moveSpeed;

            //flip the sprites if moving left
            if (input.X < -0.1)
            {
                spriteMain.FlipH = true;
            }
            if (input.X > 0.1)
            {
                spriteMain.FlipH = false;
            }

            MoveAndSlide();
        }
    }

    //Signal when a new weapon is added to the weaponfolder tree
    public void OnNewWeaponAdded(Node node)
	{
		WeaponParent newWeapon = (WeaponParent)node;
        levelMenu.GenerateWeaponUpgradeLink(newWeapon);
		mainUI.AddWeaponElement(newWeapon);
		if (weaponFolder.GetChildCount() >= maxWeapons)
		{
			levelMenu.ClearNewWeaponLinks();
		}
    }
    //Signal when a new item is added to the itemfolder tree
    public void OnNewItemAdded(Node node)
    {
        ItemParent newItem = (ItemParent)node;
        levelMenu.GenerateItemUpgradeLink(newItem);
        mainUI.AddItemElement(newItem);
		if (itemFolder.GetChildCount() >= maxItems)
		{
			levelMenu.ClearNewItemLinks();
		}
    }

    //gain experience and level up if needed
    public void GainExperience(float amount)
	{
		experience += amount;
		mainUI.SetExperienceValue(experience);

        if (experience < experienceRequired)
        {
			//play this sound only when not levelling up, as the sound pauses when 
            audioExpPickup.Play();
        }
	}

	//obtained once levelling up the character
	public void LevelUp()
	{
        experience -= experienceRequired;

        //increase the experience required
        experienceRequired += 10;
        if (level > 10) { experienceRequired += 5; }
        if (level > 20) { experienceRequired += 5; }

        if (level == 10) { experienceRequired += 100; }
        if (!levelMenu.IsItemListEmpty())
        {

            GetTree().Paused = true;
            levelController.SwitchingToLevelUpState(true);
            levelMenu.Visible = true;
            levelMenu.GenerateLevelUpList();
        }
		
        level++;
        mainUI.SetExperienceValue(experience);
        mainUI.SetMaxExperienceValue(experienceRequired);

	}

	public void ChangeMagnetScale(float newScale)
	{
		magnet.Scale = new Vector2(newScale, newScale);
	}

    public void OnBodyExited(Node2D body)
    {
        if (body.IsInGroup("Enemy"))
        {
            EnemyParent collidedEnemy = body as EnemyParent;
            if (enemiesInContact.Contains(collidedEnemy))
            {
                enemiesInContact.Remove(collidedEnemy);
            }
        }
    }

    public void ChangeMaxHealth(double healthPercentAmount)
    {
        healthComponent.ChangeMaxHealth(healthPercentAmount);
    }

    public void ChangeHealthRegen(double changeAmount)
    {
        healthComponent.ChangeHealthRegen(changeAmount);
    }

    public HurtBoxComponent GetHurtbox()
    {
        return hurtBoxComponent;
    }

    public void OnDeath()
    {
        SetWeaponActivity(false);
        spriteMain.Visible = false;
        levelController.GenerateNewExplosion1(GlobalPosition);
        levelController.OnPlayerDeath();
    }
}
