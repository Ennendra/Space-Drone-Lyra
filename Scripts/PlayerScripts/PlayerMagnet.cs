using Godot;
using System;
public partial class PlayerMagnet : Area2D
{
    //the player instance it is linked to
	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//set the player to the parent object
		player = (Player)GetParent();
	}

	public void SetPlayer(Player tPlayer)
	{
		player = tPlayer;
	}

	public void OnAreaEntered(Node2D body)
	{
        //if the hit body is an enemy instance, make it take damage
        if (body.IsInGroup("ExperiencePips"))
        {
            ExperiencePip collidedBody = (ExperiencePip)body;
			collidedBody.AttractToPlayer(player);
        }
    }
}
