using Godot;
using System;

public partial class BossEncounterParameters : Resource
{
    [Export] public PackedScene bossEnemyScene;
    [Export] public Vector2 arenaSize;
    [Export] public Vector2 arenaCenterOffset;
    [Export] public Vector2 bossSpawnOffset;

    public BossEncounterParameters() : this(null, new Vector2(0,0), new Vector2(0, 0), new Vector2(0, 0)) { }

    public BossEncounterParameters(PackedScene newBoss, Vector2 newArenaSize, Vector2 newArenaCenterOffset, Vector2 newBossSpawnOffset)
    {
        bossEnemyScene = newBoss;
        arenaSize = newArenaSize;
        arenaCenterOffset = newArenaCenterOffset;
        bossSpawnOffset = newBossSpawnOffset;
    }
}
