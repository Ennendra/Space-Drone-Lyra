using Godot;
using System;
using System.Collections.Generic;

public partial class ExperiencePip : Area2D
{

    //how much XP the pip gives
    //[Export] public float experience;
    [Export] public float experience;
    //a check to see if the pip is moving to the player
    bool isTrackingPlayer = false;
	//the player instance (when moving towards it)
	Player player;

	float moveSpeed = 300.0f;

    AnimatedSprite2D innerSprite, outerSprite;

    public override void _Ready()
    {
        base._Ready();

        innerSprite = GetNode<AnimatedSprite2D>("XPInner");
        outerSprite = GetNode<AnimatedSprite2D>("XPOuter");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        outerSprite.Rotation += (2 * Mathf.Pi) * (float)delta * 1.5f;

        if (isTrackingPlayer)
		{
			GlobalPosition = GlobalPosition.MoveToward(player.GlobalPosition, moveSpeed*(float)delta);
		}
	}

	public void AttractToPlayer(Player target)
	{
		//start tracking player if not doing so already
		if (!isTrackingPlayer)
		{
            player = target;
            isTrackingPlayer = true;

			//run this initial check in case the pip spawned inside the player collision circle
			if (GlobalPosition.DistanceTo(player.GlobalPosition) < 18)
			{
                player.GainExperience(experience);
                QueueFree();
            }
        }
	}

    public void SetExperience(float newExperience)
    {
        experience = newExperience;
        if (experience <= 10)
        {
            innerSprite.Frame = 0;
            outerSprite.Frame = 0;
        }
        if (experience > 10)
        {
            innerSprite.Frame = 1;
            outerSprite.Frame = 1;
        }
        if (experience > 20)
        {
            innerSprite.Frame = 2;
            outerSprite.Frame = 2;
        }
        if (experience > 40)
        {
            innerSprite.Frame = 3;
            outerSprite.Frame = 3;
        }
        if (experience > 80)
        {
            innerSprite.Frame = 4;
            outerSprite.Frame = 4;
        }
    }

	public void OnBodyEntered(Node2D body)
	{
        //if the hit body is the player, give it XP
        if (body.IsInGroup("Player") && player!=null)
        {
            player.GainExperience(experience);
			QueueFree();
        }
    }

    public void OnViewportExit()
    {
        var pipGroup = GetParent().GetChildren();

        Node currentClosestPip = null;
        float currentClosestDistance = -1;
        float nextDistance;

        foreach (Node2D p in pipGroup)
        {

            //check nextDistance>1 to make sure it doesn't check itself
            nextDistance = GlobalPosition.DistanceTo(p.GlobalPosition);
            if (currentClosestPip == null && nextDistance>1)
            {
                currentClosestPip = p;
                currentClosestDistance = nextDistance;
            }
            else if (nextDistance<currentClosestDistance && nextDistance > 1)
            {
                currentClosestPip = p;
                currentClosestDistance = nextDistance;
            }
        }

        if (currentClosestPip != null)
        {
            ExperiencePip pipToCombine = (ExperiencePip)currentClosestPip;
            pipToCombine.SetExperience(pipToCombine.experience + experience);
            QueueFree();
        }
    }
}
