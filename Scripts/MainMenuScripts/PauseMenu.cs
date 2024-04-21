using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	Node controllerNode;

	bool isQuitButtonComfirmed = false;

	[Export] Button quitButton;

    //audio nodes
    AudioStreamPlayer audioOpen, audioHover, audioConfirm, audioFinalConfirm;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //audio nodes
        audioOpen = GetNode<AudioStreamPlayer>("AudioMenuOpen");
        audioHover = GetNode<AudioStreamPlayer>("AudioButtonHover");
        audioConfirm = GetNode<AudioStreamPlayer>("AudioButtonConfirm");
        audioFinalConfirm = GetNode<AudioStreamPlayer>("AudioButtonFinalConfirm");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (isQuitButtonComfirmed)
		{
			quitButton.Text = "Confirm?";
		}
		else
		{
			quitButton.Text = "Quit";
		}

	}

	public void SetControllerNode(Node node)
	{
		controllerNode = node;
	}

	public void ResetQuitConfirmation()
	{
		isQuitButtonComfirmed = false;
	}

	public void OnButtonHover()
	{
		audioHover.Play();
	}

	public void OnResumePress()
	{
		audioConfirm.Play();
		controllerNode.EmitSignal("ResumeGame");
	}

	public void OnQuitPress()
	{
		if (isQuitButtonComfirmed)
		{
            audioFinalConfirm.Play();
            controllerNode.EmitSignal("QuitGame");
		}
		else
		{
            audioConfirm.Play();
            isQuitButtonComfirmed = true;
		}
	}

	public void OnVisibilityChanged()
	{
		if (Visible)
		{
            audioOpen.Play();
        }
	}
}
