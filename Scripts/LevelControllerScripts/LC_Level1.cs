using Godot;
using System;

public partial class LC_Level1 : LevelController
{
    public override void _Ready()
    {
        base._Ready();

        missionPhase = 1;
    }

    public override void CheckLevelTimerTriggers()
    {
        base.CheckLevelTimerTriggers();
        if (globals.tMinutes == 1 && globals.tSeconds == 0)
        {
            StartNewTimer(1);
        }
        if (globals.tMinutes == 2 && globals.tSeconds == 0)
        {
            StartNewTimer(2);
            CreateEliteEnemy(0, GenerateRandomSpawnPoint());
        }
        if (globals.tMinutes == 3 && globals.tSeconds == 0)
        {
            StartNewTimer(3);
        }
        if (globals.tMinutes == 4 && globals.tSeconds == 0)
        {
            StopAllTimers();
            CreateBossFightIntro(0,false); 
        }
        if (globals.tMinutes == 5 && globals.tSeconds == 0)
        {
            StartNewTimer(6);
        }
        if (globals.tMinutes == 6 && globals.tSeconds == 0)
        {
            StartMultipleTimers(new int[2] { 7, 8 });
        }
        if (globals.tMinutes == 7 && globals.tSeconds == 0)
        {
            StartMultipleTimers(new int[2] { 9, 10 });
        }
        if (globals.tMinutes == 8 && globals.tSeconds == 0)
        {
            StopAllTimers();
            CreateBossFightIntro(1,true);
        }

    }

    public override void CompleteBossFight()
    {


        if (missionPhase == 1)
        {
            StartMultipleTimers(new int[2] { 4, 5 });
            missionPhase = 2;
            levelProgressionState = LevelProgressionState.TIMER;
        }
        else if (missionPhase == 2)
        {
            OnLevelCompletion();
            missionPhase = 3;
        }
    }

    public override void CreateEnemySet(int index)
    {
        switch(index)
        {
            case 0:
                for (int i = 0; i < 5; i++)
                {
                    CreateMainEnemy(0, GenerateRandomSpawnPoint());
                }
                break;

            case 1:
                for (int i = 0; i < 3; i++)
                {
                    CreateMainEnemy(0, GenerateRandomSpawnPoint());
                }
                CreateMainEnemy(1, GenerateRandomSpawnPoint());
                break;

            case 2:
                for (int i = 0; i < 7; i++)
                {
                    CreateMainEnemy(2, GenerateRandomSpawnPoint());
                }
                break;

            case 3:
                for (int i = 0; i < 7; i++)
                {
                    CreateMainEnemy(0, GenerateRandomSpawnPoint());
                }
                for (int i = 0; i < 7; i++)
                {
                    CreateMainEnemy(2, GenerateRandomSpawnPoint());
                }
                CreateMainEnemy(1, GenerateRandomSpawnPoint());
                CreateMainEnemy(1, GenerateRandomSpawnPoint());
                break;

            case 4:
                for (int i = 0; i < 6; i++)
                {
                    CreateMainEnemy(0, GenerateRandomSpawnPoint());
                }
                for (int i = 0; i < 3; i++)
                {
                    CreateMainEnemy(3, GenerateRandomSpawnPoint());
                }
                break;

            case 5:
                for (int i = 0; i < 3; i++)
                {
                    CreateMainEnemy(4, GenerateRandomSpawnPoint());
                }
                break;

            case 6:
                for (int i = 0; i < 7; i++)
                {
                    CreateMainEnemy(2, GenerateRandomSpawnPoint());
                }
                for (int i = 0; i < 4; i++)
                {
                    CreateMainEnemy(5, GenerateRandomSpawnPoint());
                }
                break;

            case 7:
                for (int i = 0; i < 4; i++)
                {
                    CreateMainEnemy(0, GenerateRandomSpawnPoint());
                    CreateMainEnemy(2, GenerateRandomSpawnPoint());
                    CreateMainEnemy(3, GenerateRandomSpawnPoint());
                }
                break;

            case 8:
                for (int i = 0; i < 2; i++)
                {
                    CreateMainEnemy(1, GenerateRandomSpawnPoint());
                    CreateMainEnemy(4, GenerateRandomSpawnPoint());
                    CreateMainEnemy(5, GenerateRandomSpawnPoint());
                }
                break;

            case 9:
                for (int i = 0; i < 3; i++)
                {
                    CreateMainEnemy(5, GenerateRandomSpawnPoint());
                    CreateMainEnemy(3, GenerateRandomSpawnPoint());
                    CreateMainEnemy(3, GenerateRandomSpawnPoint());
                }
                break;

            case 10:
                for (int i = 0; i < 4; i++)
                {
                    CreateMainEnemy(6, GenerateRandomSpawnPoint());
                }
                break;

        }
    }

}
