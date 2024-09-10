using Godot;
using System;

public partial class UI : CanvasLayer
{
	TextureRect pauseMenu;
	public override void _Ready()
	{
		pauseMenu = GetNode<TextureRect>("Control/PauseMenu");
		pauseMenu.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel") && GetTree().Paused == false)
		{
			pauseMenu.Visible = true;
			GetTree().Paused = true;
		}
	}

	private void _PausedButtonPressed()
	{
		pauseMenu.Visible = true;
		GetTree().Paused = true;
	}
	private void _ResumeButtonPressed()
	{
		pauseMenu.Visible = false;
		GetTree().Paused = false;
	}
	private void _SaveAndQuitPressed()
	{
		GetTree().Quit();
	}
}
