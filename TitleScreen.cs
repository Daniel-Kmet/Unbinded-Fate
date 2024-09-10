using Godot;
using System;

public partial class TitleScreen : Control
{
	[Signal] public delegate void GameStartedEventHandler();
	public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
	}
	private void _onPlayPressed()
	{
		EmitSignal(SignalName.GameStarted);
		GD.Print("Pressed");
	}
	private void _onCreditsPressed()
	{
		
	}
	private void _onQuitPressed()
	{
		GetTree().Quit();
	}
}
