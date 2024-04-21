using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

//Data related to 
public enum UpgradeType
{
    NEWWEAPON,
    UPGRADEWEAPON,
    NEWITEM,
    UPGRADEITEM
}

//the structure which will hold also relevant info for upgrading or adding a weapon
internal struct NewItemDetails
{
    //what type of upgrade it is
    internal UpgradeType uType;
    internal Texture2D icon;
    internal string name;
    internal string description;
    //the weapon object to be made
    internal PackedScene newWeaponOrItem;
    //the weapon being upgraded if applicable
    internal WeaponParent weaponToUpgrade;
    //the item being upgraded if applicable
    internal ItemParent itemToUpgrade;
}

public partial class LevelUpMenu : ColorRect
{
    List<NewItemDetails> weaponsAndItems = new List<NewItemDetails>();

    //A reference to the global variables and randomiser
    GlobalScript globals;

    //a reference to the 4 buttons
    LevelUpButton[] buttons = new LevelUpButton[4];
    //a reference to each itemDetail that is on the buttons
    NewItemDetails[] buttonItems = new NewItemDetails[4];

    //A reference to all the sounds played by this menu
    AudioStreamPlayer audioLevelUp, audioUpgradeItem, audioNewItem, audioButtonHover;

    public Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

        audioLevelUp = GetNode<AudioStreamPlayer>("AudioLevelUp");
        audioNewItem = GetNode<AudioStreamPlayer>("AudioNewItem");
        audioUpgradeItem = GetNode<AudioStreamPlayer>("AudioUpgradeItem");
        audioButtonHover = GetNode<AudioStreamPlayer>("AudioButtonHover");

        //get the reference to the global variables
        globals = GetNode<GlobalScript>("/root/Globals");

        //set the references to each button
        buttons[0] = (LevelUpButton)FindChild("ButtonAction1");
        buttons[1] = (LevelUpButton)FindChild("ButtonAction2");
        buttons[2] = (LevelUpButton)FindChild("ButtonAction3");
        buttons[3] = (LevelUpButton)FindChild("ButtonAction4");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public bool IsItemListEmpty()
    {
        if (weaponsAndItems.Count == 0) { return true; }
        return false;
    }

	public void GenerateLevelUpList()
	{
        audioLevelUp.Play();

		//reset the buttons and items involved
		for (int i = 0; i < 4; i++)
		{
            buttons[i].Visible = false;
        }

        //determine how many upgrade buttons there will be
        int amountOfUpgrades = 3;
        if (weaponsAndItems.Count < amountOfUpgrades)
        {
            amountOfUpgrades = weaponsAndItems.Count;
        }

        //create a duplicate array of the full weaponsanditems list
        
        //generate a duplicate copy of the weapons and item list
        List<NewItemDetails> tempUpgradeList = new List<NewItemDetails> ();
        foreach (NewItemDetails item in weaponsAndItems)
        {
            tempUpgradeList.Add(item);
        }

        for (int i = 0; i < amountOfUpgrades; i++)
        {
            //pick an item from the list
            int itemIndex=0;
            if (tempUpgradeList.Count == 1) { itemIndex = 0;}
            itemIndex = ((int)GD.Randi()) % (tempUpgradeList.Count);
            if (itemIndex < 0) { itemIndex = -itemIndex; }
            
            //point to the chosen item and remove it from the temporary list
            NewItemDetails itemChosen = tempUpgradeList[itemIndex];
            tempUpgradeList.Remove(itemChosen);
            buttonItems[i] = itemChosen;
            buttons[i].Visible = true;

            //set the details on the button
            switch (itemChosen.uType)
            {
                case UpgradeType.NEWWEAPON:
                    {
                        buttons[i].icon.Texture = itemChosen.icon;
                        buttons[i].lblName.Text = itemChosen.name;
                        buttons[i].lblDescription.Text = itemChosen.description;
                        buttons[i].icon.Texture = itemChosen.icon;
                        buttons[i].lblLevel.Text = "New";

                        break;
                    }
                case UpgradeType.NEWITEM:
                    {
                        buttons[i].icon.Texture = itemChosen.icon;
                        buttons[i].lblName.Text = itemChosen.name;
                        buttons[i].lblDescription.Text = itemChosen.description;
                        buttons[i].icon.Texture = itemChosen.icon;
                        buttons[i].lblLevel.Text = "New";
                        break;
                    }
                case UpgradeType.UPGRADEWEAPON:
                    {
                        buttons[i].icon.Texture = itemChosen.weaponToUpgrade.weaponIcon;
                        buttons[i].lblName.Text = itemChosen.weaponToUpgrade.weaponName;
                        buttons[i].lblDescription.Text = itemChosen.weaponToUpgrade.weaponLevelUpDescriptions[itemChosen.weaponToUpgrade.weaponLevel+1];
                        buttons[i].icon.Texture = itemChosen.weaponToUpgrade.weaponIcon;
                        buttons[i].lblLevel.Text = "Level "+(itemChosen.weaponToUpgrade.weaponLevel + 1);
                        break;
                    }
                case UpgradeType.UPGRADEITEM:
                    {
                        buttons[i].icon.Texture = itemChosen.itemToUpgrade.itemIcon;
                        buttons[i].lblName.Text = itemChosen.itemToUpgrade.itemName;
                        buttons[i].lblDescription.Text = itemChosen.itemToUpgrade.itemLevelUpDescriptions[itemChosen.itemToUpgrade.itemLevel + 1];
                        buttons[i].icon.Texture = itemChosen.itemToUpgrade.itemIcon;
                        buttons[i].lblLevel.Text = "Level " + (itemChosen.itemToUpgrade.itemLevel + 1);
                        break;
                    }
            }


        }

    }

    //Generate the upgrade list for ALL items except those in the itemsToIgnore list
    //Will run the GenerateWeapons and GenerateItems functions separately, to help with readability a bit
    public void GenerateWeaponsAndItems(List<string> itemsToIgnore)
    {
        GenerateWeapons(itemsToIgnore);
        GenerateItems(itemsToIgnore);
    }
    //Linked to the above function, runs for each WEAPON
    public void GenerateWeapons(List<string> itemsToIgnore)
    {
        NewItemDetails newItem;
        //Weapon - SMG
        if (!IsItemToBeIgnored("SMG", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWWEAPON;
            newItem.name = "SMG";
            newItem.description = "Rapidly fires at the nearest enemy";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/WeaponIcons/IconSMG.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Weapons/WeaponSMG.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Weapon - Shotgun
        if (!IsItemToBeIgnored("Shotgun", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWWEAPON;
            newItem.name = "Shotgun";
            newItem.description = "Fires a wave of pellets at the nearest enemy";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/WeaponIcons/IconShotgun.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Weapons/WeaponShotgun.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Weapon - Sniper
        if (!IsItemToBeIgnored("Sniper", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWWEAPON;
            newItem.name = "Sniper";
            newItem.description = "Targets the enemy with the most current HP, dealing heavy damage";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/WeaponIcons/IconSniper.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Weapons/WeaponSniper.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Weapon - ADS
        if (!IsItemToBeIgnored("ADS", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWWEAPON;
            newItem.name = "ADS";
            newItem.description = "Neutralises enemy projectiles";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/WeaponIcons/IconADS.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Weapons/WeaponADS.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Weapon - Grenade
        if (!IsItemToBeIgnored("Grenade", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWWEAPON;
            newItem.name = "Grenade";
            newItem.description = "Fires towards a random enemy, detonates after short delay";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/WeaponIcons/IconGrenade.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Weapons/WeaponGrenade.tscn");
            weaponsAndItems.Add(newItem);
        }
    }
    public void GenerateItems(List<string> itemsToIgnore)
    {
        NewItemDetails newItem;
        //Item - Damage Modifier
        if (!IsItemToBeIgnored("Caliber Enhancer", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Caliber Enhancer";
            newItem.description = "Increases damage dealt by 10%";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconDamage.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemDamage.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Item - Scale Modifier
        if (!IsItemToBeIgnored("Holo-amplifier", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Holo-amplifier";
            newItem.description = "Increases projectile size by 10%";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconScale.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemScale.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Item - Fire Rate Modifier
        if (!IsItemToBeIgnored("Autoloader", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Autoloader";
            newItem.description = "Increase all weapon's rate of fire by 10%";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconFireRate.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemFireRate.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Item - Reload Modifier
        if (!IsItemToBeIgnored("Ammo Autocycling", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Ammunition Autocycling";
            newItem.description = "Increase all weapon's reload speed by 10%";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconReloadSpeed.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemReloadSpeed.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Item - Experience Modifier
        if (!IsItemToBeIgnored("Data Recorder", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Data Recorder";
            newItem.description = "Increase Experience gain by 10%";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconExperience.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemExperience.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Item - Max HP Modifier
        if (!IsItemToBeIgnored("Gel Armor Casing", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Gel Armor Casing";
            newItem.description = "Increase maximum health by 20%";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconMaxHealth.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemMaxHealth.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Item - HP Regen Modifier
        if (!IsItemToBeIgnored("Internal Nanites", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Internal Nanites";
            newItem.description = "Increase health regeneration by 0.2 per second";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconHealthRegen.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemHealthRegen.tscn");
            weaponsAndItems.Add(newItem);
        }
        //Item - XP Magnet Modifier
        if (!IsItemToBeIgnored("Data Scanner", itemsToIgnore))
        {
            newItem = new NewItemDetails();
            newItem.uType = UpgradeType.NEWITEM;
            newItem.name = "Data Scanner";
            newItem.description = "Increases experience magnetisation range by 20%";
            newItem.icon = (Texture2D)GD.Load("res://Textures/Icons/ItemIcons/IconExperienceMagnet.png");
            newItem.newWeaponOrItem = (PackedScene)ResourceLoader.Load("res://Objects/Items/ItemExperienceMagnet.tscn");
            weaponsAndItems.Add(newItem);
        }
    }

    //removes all itemdetail objects that are of type NEWWEAPON. Mainly used when weapon limit has been reached
    public void ClearNewWeaponLinks()
    {
        //generate the list of items to be removed
        List<NewItemDetails> itemsToClear = new List<NewItemDetails>();
        foreach (NewItemDetails item in weaponsAndItems)
        {
            if (item.uType == UpgradeType.NEWWEAPON) 
            { 
                itemsToClear.Add(item);
            }
        }
        //remove each item from the main list
        foreach (NewItemDetails item in itemsToClear)
        {
            weaponsAndItems.Remove(item);
        }
    }
    //removes all itemdetail objects that are of type NEWITEM. Mainly used when item limit has been reached
    public void ClearNewItemLinks()
    {
        //generate the list of items to be removed
        List<NewItemDetails> itemsToClear = new List<NewItemDetails>();
        foreach (NewItemDetails item in weaponsAndItems)
        {
            if (item.uType == UpgradeType.NEWITEM)
            {
                itemsToClear.Add(item);
            }
        }
        //remove each item from the main list
        foreach (NewItemDetails item in itemsToClear)
        {
            weaponsAndItems.Remove(item);
        }
    }

    //check if the item name matches any of the ignore list. If it does, return true so it won't be added as a potential upgrade
    public bool IsItemToBeIgnored(string item, List<string> itemsToIgnore)
    {
        foreach (string itemName in itemsToIgnore)
        {
            if (itemName == item) { return true; }
        }
        return false;
    }

    //creating a link to an existing weapon as an "upgrade"
    public void GenerateWeaponUpgradeLink(WeaponParent weapon)
    {
        NewItemDetails newItem = new NewItemDetails();
        newItem.uType = UpgradeType.UPGRADEWEAPON;
        newItem.weaponToUpgrade = weapon;
        weaponsAndItems.Add(newItem);
    }
    //creating a link to an existing item as an "upgrade"
    public void GenerateItemUpgradeLink(ItemParent item)
    {
        NewItemDetails newItem = new NewItemDetails();
        newItem.uType= UpgradeType.UPGRADEITEM;
        newItem.itemToUpgrade = item;
        weaponsAndItems.Add(newItem);
    }
    

	public void ProcessButtonPress(int index)
	{
        //create a reference to the button details
        NewItemDetails buttonItem = buttonItems[index];

        //Check what type of button prompt it is
        switch (buttonItem.uType)
        {
            case UpgradeType.NEWWEAPON:
                {
                    //create the new weapon and add it to the player
                    WeaponParent newWeapon = (WeaponParent)buttonItem.newWeaponOrItem.Instantiate();
                    player.weaponFolder.AddChild(newWeapon);
                    newWeapon.GlobalPosition = player.GlobalPosition;
                    player.AddNewWeaponToList(newWeapon);
                    //remove the weapon from the upgrade list and add an upgrading option instead
                    weaponsAndItems.Remove(buttonItem);
                    //play the new item sound
                    audioNewItem.Play();

                    //Emit this signal so that if this weapon is the ADS, it can
                    newWeapon.EmitSignal("LinkToHurtbox",player.GetHurtbox());

                    break;
                }
            case UpgradeType.NEWITEM:
                {
                    //create the new item and add it to the player
                    ItemParent newItem = (ItemParent)buttonItem.newWeaponOrItem.Instantiate();
                    player.itemFolder.AddChild(newItem);
                    //remove the item from the list and add it's upgrading option instead
                    weaponsAndItems.Remove(buttonItem);
                    //run the level up function for the item once to initiate it as 'level 1' and add the first modifier stack
                    newItem.LevelUpItem();
                    //play the new item sound
                    audioNewItem.Play();

                    break;
                }
            case UpgradeType.UPGRADEWEAPON:
                {
                    //level up the weapon and remove this option from the levelup list if the weapon is maxed
                    buttonItem.weaponToUpgrade.LevelUpWeapon();
                    audioUpgradeItem.Play();
                    if (buttonItem.weaponToUpgrade.weaponLevel == buttonItem.weaponToUpgrade.maxWeaponLevel)
                    {
                        weaponsAndItems.Remove(buttonItem);
                    }
                    break;
                }
            case UpgradeType.UPGRADEITEM:
                {
                    //TODO - change to item things
                    buttonItem.itemToUpgrade.LevelUpItem();
                    audioUpgradeItem.Play();
                    if (buttonItem.itemToUpgrade.itemLevel == buttonItem.itemToUpgrade.maxItemLevel)
                    {
                        weaponsAndItems.Remove(buttonItem);
                    }
                    break;
                }
        }

        this.Visible = false;
        GetTree().Paused = false;
        player.levelController.SwitchingToLevelUpState(false);
    }
	//signals for each button when pressed
	public void OnButton1Pressed()
	{
        CallDeferred("ProcessButtonPress",0);
	}
    public void OnButton2Pressed()
    {
        CallDeferred("ProcessButtonPress", 1);
    }
    public void OnButton3Pressed()
    {
        CallDeferred("ProcessButtonPress", 2);
    }
    public void OnButton4Pressed()
    {
        CallDeferred("ProcessButtonPress", 3);
    }

    public void OnButton1Hover()
    {
        audioButtonHover.Play();
    }

    public void OnButton2Hover()
    {
        audioButtonHover.Play();
    }

    public void OnButton3Hover()
    {
        audioButtonHover.Play();
    }

    public void OnButton4Hover()
    {
        audioButtonHover.Play();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Key1 && buttons[0].Visible == true)
            {
                ProcessButtonPress(0);
            }
            if (keyEvent.Keycode == Key.Key2 && buttons[1].Visible == true)
            {
                ProcessButtonPress(1);
            }
            if (keyEvent.Keycode == Key.Key3 && buttons[2].Visible == true)
            {
                ProcessButtonPress(2);
            }
            if (keyEvent.Keycode == Key.Key4 && buttons[3].Visible == true)
            {
                ProcessButtonPress(3);
            }
        }
    }
}
