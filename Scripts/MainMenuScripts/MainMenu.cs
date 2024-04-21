using Godot;
using System;
using System.IO;

internal enum MenuState
{
	MAINMENU,
	EXITCONFIRM,
	CHARACTERSELECT,
	LEVELSELECT,
    CREDITS
}

//Details for each button in the character select menu, helps track which one is selected or requires to be unlocked
internal struct CharacterSelectButton
{
	internal MainMenuCharacterContainer container;
	internal bool isSelected;
	internal bool isUnlocked;
}
internal struct LevelSelectButton
{
    internal LevelSelectContainer container;
    internal bool isSelected;
    internal bool isUnlocked;
}

public partial class MainMenu : CanvasLayer
{

    GlobalScript globals;

	MenuState currentMenuState = MenuState.MAINMENU;

    [Export] PackedScene levelOneScene;


	//front menu
    Control exitConfirm, mainMenuButtons;
    Label titleLabel;

	//characterSelect
	Control characterSelectMenu;
	[Export] Control characterButtonParent, characterDescriptionParent;
	CharacterSelectButton[] characterContainer;
    Label characterSelectName, characterSelectDescription;
    CharacterSelectButton finalSelectedCharacter;

	//level select
	Control levelSelectMenu;
	[Export] Control levelButtonParent, levelDescriptionParent;
    LevelSelectButton[] levelContainer;
	Label levelSelectName, levelSelectDescription;

    Control creditsMenu;

    //Menu audio
    AudioStreamPlayer audioHover, audioConfirm, audioFinalConfirm;


    public override void _Ready()
	{
        //The globalscript, will be used to tell what player scene to instantiate when the level starts
        globals = GetNode<GlobalScript>("/root/Globals");

        //front menu
        exitConfirm = GetNode<Control>("ExitConfirmContainer");
		mainMenuButtons = GetNode<Control>("ButtonContainer");
        titleLabel = GetNode<Label>("TitleContainer/CenterContainer/Title");

		//character select
		characterSelectMenu = GetNode<Control>("CharacterSelectContainer");
		characterContainer = new CharacterSelectButton[characterButtonParent.GetChildCount()];
		for (int i = 0; i < characterContainer.Length; i++)
		{
			characterContainer[i] = new CharacterSelectButton();
			characterContainer[i].container = (MainMenuCharacterContainer)characterButtonParent.GetChild(i);
			characterContainer[i].isSelected = false;
			characterContainer[i].isUnlocked = true;
		}
        characterSelectName = characterDescriptionParent.GetNode<Label>("Name");
        characterSelectDescription = characterDescriptionParent.GetNode<Label>("Description");
		//SelectCharacterButton(0);

        //level select
        levelSelectMenu = GetNode<Control>("LevelSelectContainer");
		levelContainer = new LevelSelectButton[levelButtonParent.GetChildCount()];
        for (int i = 0; i < levelContainer.Length; i++)
        {
            levelContainer[i] = new LevelSelectButton();
            levelContainer[i].container = (LevelSelectContainer)levelButtonParent.GetChild(i);
            levelContainer[i].isSelected = false;
            levelContainer[i].isUnlocked = true;
        }
        levelSelectName = levelDescriptionParent.GetNode<Label>("Name");
        levelSelectDescription = levelDescriptionParent.GetNode<Label>("Description");
        //SelectLevelButton(0);

        //Credits
        creditsMenu = GetNode<Control>("CreditsContainer");

        //audio nodes
        audioHover = GetNode<AudioStreamPlayer>("AudioButtonHover");
        audioConfirm = GetNode<AudioStreamPlayer>("AudioButtonConfirm");
        audioFinalConfirm = GetNode<AudioStreamPlayer>("AudioButtonFinalConfirm");
    }

    void ChangeMenuState(MenuState newState)
    {
        HideAllElements();

        switch (newState)
        {
            case MenuState.MAINMENU: mainMenuButtons.Visible = true; titleLabel.Visible = true; break;
            case MenuState.EXITCONFIRM: exitConfirm.Visible = true; break;
            case MenuState.CHARACTERSELECT: characterSelectMenu.Visible = true; break;
            case MenuState.LEVELSELECT: levelSelectMenu.Visible = true; break;
            case MenuState.CREDITS: creditsMenu.Visible = true; break;
            default:
                mainMenuButtons.Visible = true;
                GD.Print("Main Menu navigation error!");
                break;
        }
        currentMenuState = newState;

    }

    void HideAllElements()
    {
        titleLabel.Visible = false;

        exitConfirm.Visible = false;
        mainMenuButtons.Visible = false;
        characterSelectMenu.Visible = false;
        levelSelectMenu.Visible = false;
        creditsMenu.Visible = false;
    }

    public void OnButtonHover()
    {
        audioHover.Play();
    }


    //character select menu functions ---
    public void SelectCharacterButton(int index)
	{
        characterContainer[index].isSelected = true;
        //TODO: Something here to add a visual cue that one of the character buttons is selected
        SetCharacterSelectInfo(characterContainer[index].container);
    }

	public void SetCharacterSelectInfo(MainMenuCharacterContainer container)
	{
        characterSelectName.Text = container.GetPlayerInfoName();
        characterSelectDescription.Text = container.GetPlayerInfoDescription();
    }


    //Level select menu functions ---
    void SelectLevelButton(int index)
    {
        levelContainer[index].isSelected = true;
        SetLevelSelectInfo(levelContainer[index].container);
    }
    
    public void SetLevelSelectInfo(LevelSelectContainer container)
    {
        levelSelectName.Text = container.GetLevelInfoName();
        levelSelectDescription.Text = container.GetLevelInfoDescription();
    }


    //Main menu buttons
    public void OnStartPress()
    {
        audioConfirm.Play();
		ChangeMenuState(MenuState.CHARACTERSELECT);
    }

    public void OnCreditsPress()
    {
        audioConfirm.Play();
        ChangeMenuState(MenuState.CREDITS);
    }

    public void OnExitPress()
	{
        if (currentMenuState == MenuState.MAINMENU)
		{
            audioConfirm.Play();
            ChangeMenuState(MenuState.EXITCONFIRM);
        }
	}

    
    //Exit confirm buttons
    public void OnExitConfirmYesPress()
	{
        if (currentMenuState == MenuState.EXITCONFIRM)
        {
            audioConfirm.Play();
            GetTree().Quit();
        }
    }

    public void OnExitConfirmNoPress()
    {
        if (currentMenuState == MenuState.EXITCONFIRM)
        {
            audioConfirm.Play();
            ChangeMenuState(MenuState.MAINMENU);
        }
    }


	//character select buttons
	public void OnCharacterSelectButtonPress(int index)
	{
		CharacterSelectButton selectedButton = characterContainer[index];
		if (selectedButton.isSelected)
		{
            audioFinalConfirm.Play();
            finalSelectedCharacter = selectedButton;
			ChangeMenuState(MenuState.LEVELSELECT);
		}
		else
		{
            audioConfirm.Play();
            for (int i = 0; i < characterContainer.Length; i++)
            {
                characterContainer[i].isSelected = false;
            }
			SelectCharacterButton(index);
        }
	}

	public void OnCSBackButtonPress()
	{
		if (currentMenuState == MenuState.CHARACTERSELECT)
		{
            audioConfirm.Play();
            ChangeMenuState(MenuState.MAINMENU);
		}
	}

    public void OnCreditsBackButtonPress()
    {
        audioConfirm.Play();
        ChangeMenuState(MenuState.MAINMENU);
    }


	//level select buttons
    public void OnLevelSelectButtonPress(int index)
    {
        LevelSelectButton selectedButton = levelContainer[index];
        if (selectedButton.isSelected)
        {
            audioFinalConfirm.Play();
            //ChangeMenuState(MenuState.LEVELSELECT);
            globals.playerToCreate = finalSelectedCharacter.container.GetPlayerScene();
            GetTree().ChangeSceneToPacked(selectedButton.container.GetLevelScene());
        }
        else
        {
            audioConfirm.Play();
            for (int i = 0; i < levelContainer.Length; i++)
            {
                levelContainer[i].isSelected = false;
            }
            SelectLevelButton(index);
        }
    }

    public void OnLSBackButtonPress()
    {
        if (currentMenuState == MenuState.LEVELSELECT)
        {
            audioConfirm.Play();
            ChangeMenuState(MenuState.CHARACTERSELECT);
        }
    }
}
