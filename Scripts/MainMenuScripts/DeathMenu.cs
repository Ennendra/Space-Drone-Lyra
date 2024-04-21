using Godot;
using System;

public partial class DeathMenu : CanvasLayer
{
    Node controllerNode;

    //audio nodes
    AudioStreamPlayer audioOpen, audioHover, audioConfirm;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

        //audio nodes
        audioOpen = GetNode<AudioStreamPlayer>("AudioMenuOpen");
        audioHover = GetNode<AudioStreamPlayer>("AudioButtonHover");
        audioConfirm = GetNode<AudioStreamPlayer>("AudioButtonConfirm");

        audioOpen.Play();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SetControllerNode(Node node)
    {
        controllerNode = node;
    }

    public void OnButtonHover()
    {
        audioHover.Play();
    }

    public void OnRetryPress()
    {
        audioConfirm.Play();
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    public void OnQuitPress()
    {
        audioConfirm.Play();
        controllerNode.EmitSignal("QuitGame");
    }
}
