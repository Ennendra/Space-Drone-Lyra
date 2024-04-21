using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Godot.TextServer;

internal enum LevelGameState
{
    PLAYING,
    PAUSED,
    LEVELUP,
    DEATH,
    COMPLETE
}

internal enum LevelProgressionState
{
    TIMER,
    BOSSINTRO,
    BOSSFIGHT
}

public partial class LevelController : Node
{
	//A reference to the global variables and randomiser
	public GlobalScript globals;

    Player currentPlayer;

    protected AudioStreamPlayer audioBGM, audioBGMFinalFight;
    protected bool BGMFadeOut = false;

	//the list of enemies currently on the field
	public List<EnemyParent> enemies { get; private set; } = new List<EnemyParent>();
    public List<EnemyProjectileParent> enemyProjectiles { get; private set; } = new List<EnemyProjectileParent>();

    //The mission phase. Helps track when bossfights are happening and when to progress
    LevelGameState levelState = LevelGameState.PLAYING;
    internal LevelProgressionState levelProgressionState = LevelProgressionState.TIMER;
    //Tracks what point of the level we are in, ie. which bossfight we are in
    protected int missionPhase = 1;

    //Spawn timers
    protected Timer[] timers;
    //what the wait time on each timer will be (the length of this exported array will determine how many timers there are
    [Export] protected double[] timerDelays;
    

    //PackedScenes of all relevant enemies
    [Export] protected PackedScene[] mainEnemies;
    [Export] protected PackedScene[] eliteEnemies;
    [Export] protected BossEncounterParameters[] bossEncounters;

    //Boss-related
    PackedScene sBossIntroWarning = (PackedScene)ResourceLoader.Load("res://Objects/UI/BossIntroWarning.tscn");
    PackedScene sBossArena = (PackedScene)ResourceLoader.Load("res://Objects/ArenaWall.tscn");
    protected BossParent currentBoss;
    protected ArenaWall currentWall;
    
    //the experience pip scene to instantiate
    PackedScene sExperiencePip = (PackedScene)ResourceLoader.Load("res://Objects/ExperiencePip.tscn");
    //Set of explosions that can be generated
    PackedScene explosion1Scene = (PackedScene)ResourceLoader.Load("res://Objects/FX/Explosion1.tscn");
    PackedScene explosion2Scene = (PackedScene)ResourceLoader.Load("res://Objects/FX/Explosion2.tscn");
    PackedScene explosionSmall1Scene = (PackedScene)ResourceLoader.Load("res://Objects/FX/ExplosionSmall1.tscn");

    PackedScene teleportEffectScene = (PackedScene)ResourceLoader.Load("res://Objects/Enemies/EnemyTeleportEffect.tscn");

    //Menu scenes and references
    PackedScene mainMenuScene = (PackedScene)ResourceLoader.Load("res://MainScenes/MainMenu.tscn");
    PackedScene levelUpMenuScene = (PackedScene)ResourceLoader.Load("res://Objects/UI/LevelUpMenu.tscn");
    PackedScene pauseMenuScene = (PackedScene)ResourceLoader.Load("res://Objects/UI/PauseMenu.tscn");
    PackedScene deathMenuScene = (PackedScene)ResourceLoader.Load("res://Objects/UI/DeathMenu.tscn");
    PackedScene completeMenuScene = (PackedScene)ResourceLoader.Load("res://Objects/UI/CompleteMenu.tscn");
    PauseMenu pauseMenu;
    DeathMenu deathMenu;
    CompleteMenu completeMenu;

    //a list of the index numbers from the mainEnemies list when too many enemies are already on the field.
    internal List<int> enemyQueueIndex = new List<int>();
    internal int maxEnemyPopulation = 250;

    //Containers for certain groups of nodes
    Node enemyContainer, experienceContainer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //set the global script
        globals = GetNode<GlobalScript>("/root/Globals");
        globals.ResetLevel();

        enemyContainer = GetNode("EnemyContainer");
        experienceContainer = GetNode("ExperienceContainer");

        //Generate the selected player and a pause menu
        CallDeferred("CreatePlayer", globals.playerToCreate);
        CallDeferred("CreatePauseMenu");

        CallDeferred("InitiateTimers");

        //Set signals for keyboard input.
        AddUserSignal("ResumeGame");
        Connect("ResumeGame", new Callable(this, "ResumeGame"));
        AddUserSignal("QuitGame");
        Connect("QuitGame", new Callable(this, "QuitGame"));


        audioBGM = GetNode<AudioStreamPlayer>("BGM");
        audioBGMFinalFight = GetNode<AudioStreamPlayer>("BGMFinalFight");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        ProcessLevelTimer(delta);
        ManageLevelProgression();

        if (Input.IsActionJustPressed("Screenshot"))
        {

            var date = Time.GetDateStringFromSystem().Replace(".", "_");
            var time = Time.GetTimeStringFromSystem().Replace(":", "");
            var screenPath = "E:\\User file folder\\Pictures\\LyraScreenshots\\Screenshot_" + date + "_" + time + ".png";
            var image = GetViewport().GetTexture().GetImage();
            image.SavePng(screenPath);
        }

        if (BGMFadeOut) { audioBGM.VolumeDb -= (float)delta * 15; }

        if (enemyQueueIndex.Count > 0 && enemies.Count < maxEnemyPopulation)
        {
            int nextEnemy = enemyQueueIndex.First<int>();
            CreateEnemyFromQueue(nextEnemy, GenerateRandomSpawnPoint());
        }

        if (Input.IsActionJustPressed("PauseMenu"))
        {
            switch (levelState)
            {
                case LevelGameState.PLAYING:
                    {
                        OpenPauseMenu();
                        levelState = LevelGameState.PAUSED;
                        break;
                    }
                case LevelGameState.PAUSED:
                    {
                        CallDeferred("ResumeGame");
                        levelState = LevelGameState.PLAYING;
                        break;
                    }
            }
        }
    }

    //Time-elapsed/Level progression related functions
    public void ProcessLevelTimer(double delta)
    {
        //process the level timer
        if (levelProgressionState == LevelProgressionState.TIMER && levelState == LevelGameState.PLAYING) { globals.tSecFraction += delta; }

        //Increase the timer second and minutes if the values are high enough
        //Also trigger new phases with relevant minutes
        if (globals.tSecFraction >= 1)
        {
            globals.tSecFraction--;
            globals.tSeconds++;

            if (globals.tSeconds >= 60)
            {
                globals.tSeconds -= 60;
                globals.tMinutes++;
            }
            CheckLevelTimerTriggers();
        }

    }
    public virtual void CheckLevelTimerTriggers()
    {
        //This function will be overridden in relevant child scripts with triggers to start timers
    }
    public void ManageLevelProgression()
    {
        //change the UI timer text color based on phase
        if (levelProgressionState == LevelProgressionState.TIMER)
        {
            globals.playerCharacter.GetMainUI().SetGameTimerColor(new Color(1,1,1,1));
        }
        else
        {
            globals.playerCharacter.GetMainUI().SetGameTimerColor(new Color(1.0f, 0.5f, 0.5f, 1.0f));
        }


        if (levelProgressionState == LevelProgressionState.BOSSFIGHT)
        {
            if (!enemies.Contains(currentBoss))
            {
                globals.playerCharacter.GetMainUI().SetBossUIVisibility(false);
                CompleteBossFight();
            }
            else
            {
                globals.playerCharacter.GetMainUI().SetBossUIVisibility(true);
                globals.playerCharacter.GetMainUI().SetBossHealthValue((float)currentBoss.GetCurrentHealth());
                globals.playerCharacter.GetMainUI().SetBossMaxHealthValue((float)currentBoss.GetMaxHealth());
            }
        }
    }
    public virtual void CompleteBossFight()
    {
        //This function will be overridden in child scripts
        StartNewTimer(3);
        missionPhase = 1;
    }

    //Timer related functions
    public void InitiateTimers()
    {
        //Generate the timers and their delays
        Node spawnTimerContainer = GetNode("SpawnTimers");
        int amountOfTimers = timerDelays.Length;
        timers = new Timer[amountOfTimers];
        for (int i = 0; i < amountOfTimers; i++)
        {
            Timer newTimer = new Timer();
            newTimer.WaitTime = timerDelays[i];
            spawnTimerContainer.AddChild(newTimer);
            timers[i] = newTimer;

            //set the current loop index as a separate integer
            int newTimerIndex = i;
            newTimer.Timeout += () => OnSpawnTimerTimeout(index: newTimerIndex);;
        }
        
        StartNewTimer(0);
    }
    protected void StopAllTimers()
    {
        for (int i = 0; i < timers.Length; i++)
        {
            timers[i].Stop();
        }
    }
    protected void StartNewTimer(int index)
    {
        StopAllTimers();
        ClearEnemyQueue();
        timers[index].Start();
    }
    protected void StartMultipleTimers(int[] indexes)
    {
        StopAllTimers();
        ClearEnemyQueue();
        foreach (int index in indexes)
        {
            timers[index].Start();
        }
    }
    public void PauseTimers()
    {
        for (int i = 0; i < timers.Length; i++)
            { timers[i].Paused = true; }
    }
    public void ResumeTimers()
    {
        for (int i = 0; i < timers.Length; i++)
            { timers[i].Paused = false; }
    }

    //Player and menu creation functions
    protected void CreatePlayer(PackedScene playerScene)
    {
        Player newPlayer = (Player)playerScene.Instantiate();
        GetTree().CurrentScene.AddChild(newPlayer);
        globals.SetPlayer(newPlayer);
        newPlayer.levelController = this;
        currentPlayer = newPlayer;

        GenerateLevelUpMenu(newPlayer);
    }
    public void CreateDeathMenu()
    {
        deathMenu = (DeathMenu)deathMenuScene.Instantiate();
        GetTree().CurrentScene.AddChild(deathMenu);
        deathMenu.SetControllerNode(this);
        audioBGM.VolumeDb -= 10;
        PauseTimers();
        GetTree().Paused = true;
    }
    protected void CreatePauseMenu()
    {
        pauseMenu = (PauseMenu)pauseMenuScene.Instantiate();
        GetTree().CurrentScene.AddChild(pauseMenu);
        pauseMenu.SetControllerNode(this);
        pauseMenu.Visible = false;

    }
    protected void CreateCompleteMenu()
    {
        completeMenu = (CompleteMenu)completeMenuScene.Instantiate();
        GetTree().CurrentScene.AddChild(completeMenu);
        completeMenu.SetControllerNode(this);
        audioBGM.VolumeDb -= 10;
        PauseTimers();
        GetTree().Paused = true;
    }
    public void GenerateLevelUpMenu(Player playerLink)
    {
        Node newMenu = levelUpMenuScene.Instantiate();
        //playerLink.AddChild(newMenu);
        GetTree().CurrentScene.AddChild(newMenu);
        LevelUpMenu menuScript = newMenu.GetNode<LevelUpMenu>("CenterContainer/LevelUpMenu");
        menuScript.player = playerLink;

        playerLink.SetLevelUpMenu(menuScript);
        List<string> itemsToExclude = playerLink.InitiateItems();

        menuScript.GenerateWeaponsAndItems(itemsToExclude);
    }

    //Enemy generation functions
    public virtual Vector2 GenerateRandomSpawnPoint()
	{
        float randAngle = -Mathf.Pi + (GD.Randf() * Mathf.Pi * 2);
        Vector2 direction = Vector2.FromAngle(randAngle);
        direction = globals.playerCharacter.GlobalPosition + (direction * (800 + (-10 + (GD.Randf() * 20))));
		return direction;
    }
    public void AddEnemyToQueue(int enemyIndex)
    {
        enemyQueueIndex.Add(enemyIndex);
    }
    public void CreateEnemyFromQueue(int enemyIndex, Vector2 position)
    {
        CreateMainEnemy(enemyIndex, position);
        enemyQueueIndex.Remove(enemyIndex);
    }
    public void ClearEnemyQueue()
    {
        enemyQueueIndex.Clear();
    }
    public void CreateMainEnemy(int enemyIndex, Vector2 position)
    {
        if (enemies.Count < maxEnemyPopulation)
        {
            EnemyParent newEnemy = (EnemyParent)mainEnemies[enemyIndex].Instantiate();
            //GetTree().CurrentScene.AddChild(newEnemy);
            enemyContainer.AddChild(newEnemy);
            enemies.Add(newEnemy);
            newEnemy.GlobalPosition = position;
            newEnemy.playerTarget = globals.playerCharacter;
            newEnemy.levelController = this;
        }
        else
        {
            AddEnemyToQueue(enemyIndex);
        }
    }
    public void CreateEliteEnemy(int enemyIndex, Vector2 position)
    {

        EnemyParent newEnemy = (EnemyParent)eliteEnemies[enemyIndex].Instantiate();
        enemyContainer.AddChild(newEnemy);
        enemies.Add(newEnemy);
        newEnemy.GlobalPosition = position;
        newEnemy.playerTarget = globals.playerCharacter;
        newEnemy.levelController = this;
    }
    public async void CreateBossFightIntro(int bossIndex, bool isFinalFight)
    {
        ClearEnemyQueue();

        levelProgressionState = LevelProgressionState.BOSSINTRO;
        Node introScene = sBossIntroWarning.Instantiate();
        GetTree().CurrentScene.AddChild(introScene);

        if (isFinalFight ) { BGMFadeOut = true; }

        //set enemies to jump out during the 5 second wait
        foreach (EnemyParent enemy in enemies)
        {
            enemy.TeleportEnemyAway();
        }

        //create a delay before creating the boss itself
        SceneTreeTimer introTimer = GetTree().CreateTimer(5.0, false);
        await ToSignal(introTimer, SceneTreeTimer.SignalName.Timeout);

        //remove the warning label and create the boss
        introScene.QueueFree();
        CreateBossEnemy(bossIndex);

        var experiencePips = experienceContainer.GetChildren();
        foreach (Node experience in experiencePips)
        {
            ExperiencePip xpPipCast = experience as ExperiencePip;

            xpPipCast.AttractToPlayer(currentPlayer);
        }

        if (isFinalFight) 
        { 
            audioBGMFinalFight.Play();
        }
        //set the mission to the boss phase
        levelProgressionState = LevelProgressionState.BOSSFIGHT;
    }
    public void CreateBossEnemy(int bossIndex)
    {
        BossEncounterParameters newEncounter = bossEncounters[bossIndex];

        Vector2 newArenaSize = newEncounter.arenaSize;
        Vector2 newArenaCenter = globals.playerCharacter.GlobalPosition + newEncounter.arenaCenterOffset;


        BossParent newEnemy = (BossParent)newEncounter.bossEnemyScene.Instantiate();
        enemyContainer.AddChild(newEnemy);
        enemies.Add(newEnemy);
        newEnemy.GlobalPosition = newArenaCenter + newEncounter.bossSpawnOffset;
        newEnemy.playerTarget = globals.playerCharacter;
        newEnemy.levelController = this;

        currentBoss = newEnemy;

        //create the arena bounds
        ArenaWall newWall = (ArenaWall)sBossArena.Instantiate();
        GetTree().CurrentScene.AddChild(newWall);
        newWall.GlobalPosition = newArenaCenter;
        currentBoss.wall = newWall;

        //set the boss's arena center and size, and set the collision bounds on the walls
        currentBoss.arenaCenter = newArenaCenter;
        currentBoss.arenaSize = newArenaSize;
        newWall.SetArenaBounds(newArenaCenter, newArenaSize);
        currentBoss.InitiateSpecialBossBehaviours();
    }

    //Enemy and projectile list functions
    public void AddEProjectileToList(EnemyProjectileParent newProjectile)
    {
        enemyProjectiles.Add(newProjectile);
    }
    public void RemoveEProjectileFromList(EnemyProjectileParent newProjectile)
    {
        enemyProjectiles.Remove(newProjectile);
    }
    public void RemoveEnemy(EnemyParent enemy, bool isKilled)
    {
        enemies.Remove(enemy);
        if (isKilled) { globals.enemiesKilled++; }
    }

    //experience pip related functions
    public void CreateExperiencePip(Vector2 position, float xpAmount)
    {
        ExperiencePip newPip = (ExperiencePip)sExperiencePip.Instantiate();
        //GetTree().CurrentScene.AddChild(newPip);
        experienceContainer.AddChild(newPip);
        newPip.GlobalPosition = position;
        newPip.SetExperience(xpAmount * globals.gExperienceModifier);

    }

    //Misc scene instantiation functions

    public void GenerateNewExplosion1(Vector2 position)
    {
        Explosion1 newExplosion = (Explosion1)explosion1Scene.Instantiate();
        GetTree().CurrentScene.AddChild(newExplosion);
        newExplosion.GlobalPosition = position;
    }
    public void GenerateNewExplosion2(Vector2 position)
    {
        Explosion1 newExplosion = (Explosion1)explosion2Scene.Instantiate();
        GetTree().CurrentScene.AddChild(newExplosion);
        newExplosion.GlobalPosition = position;
    }
    public void GenerateNewExplosionSmall1(Vector2 position)
    {
        Explosion1 newExplosion = (Explosion1)explosionSmall1Scene.Instantiate();
        GetTree().CurrentScene.AddChild(newExplosion);
        newExplosion.GlobalPosition = position;
    }

    public void GenerateTeleportEffect(Vector2 position)
    {
        EnemyTeleportEffect teleportEffect = (EnemyTeleportEffect)teleportEffectScene.Instantiate();
        GetTree().CurrentScene.AddChild(teleportEffect);
        teleportEffect.GlobalPosition = position;
    }

    //Other state change functions
    public void SwitchingToLevelUpState(bool isLevelupState)
    {
        if (isLevelupState) 
        { 
            levelState = LevelGameState.LEVELUP;
            PauseTimers();
        }
        else 
        { 
            levelState = LevelGameState.PLAYING; 
            ResumeTimers();
        }
    }
    public async void OnPlayerDeath()
    {
        levelState = LevelGameState.DEATH;
        SceneTreeTimer deathTimer = GetTree().CreateTimer(3.0, false);
        await ToSignal(deathTimer, SceneTreeTimer.SignalName.Timeout);
        CreateDeathMenu();
    }
    public async void OnLevelCompletion()
    {
        levelState = LevelGameState.COMPLETE;
        SceneTreeTimer completeTimer = GetTree().CreateTimer(3.0, false);
        await ToSignal(completeTimer, SceneTreeTimer.SignalName.Timeout);
        CreateCompleteMenu();
    }
    public void OpenPauseMenu()
    {
        audioBGM.VolumeDb -= 10;
        levelState = LevelGameState.PAUSED;
        GetTree().Paused = true;
        PauseTimers();
        pauseMenu.Visible = true;
    }
    public void ResumeGame()
    {
        audioBGM.VolumeDb += 10;
        levelState = LevelGameState.PLAYING;
        GetTree().Paused = false;
        ResumeTimers();
        pauseMenu.Visible = false;
    }
    public void QuitGame()
    {
        ResumeTimers();
        GetTree().Paused = false;
        GetTree().ChangeSceneToPacked(mainMenuScene);
    }

    //Functions related to the spawn timers
    public virtual void CreateEnemySet(int index)
    {
        //This function will be overridden in child scripts, holding code for what enemies spawn on what timer index
    }

    public void OnSpawnTimerTimeout(int index)
    {
        CreateEnemySet(index);
        timers[index].Start();
    }

}
