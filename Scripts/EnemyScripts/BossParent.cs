using Godot;
using System;

public partial class BossParent : EnemyParent
{
    public Vector2 arenaCenter, arenaSize;
    public ArenaWall wall;

    public virtual void InitiateSpecialBossBehaviours()
    {
        //This function will be overridden as necessary in child scripts
    }
}
