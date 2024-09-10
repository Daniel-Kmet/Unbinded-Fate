using Godot;
using System;

public partial class SceneManager : Node2D
{
	[Signal] public delegate void QuitToMainMenuEventHandler();
	PackedScene nextScene = ResourceLoader.Load<PackedScene>("res://Scenes/Game/Main.tscn");
	UI ui;
	TitleScreen titleScreen;
	TransitionScene transitionScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		transitionScene = GetNode<TransitionScene>("TransitionScene");
		transitionScene.Transitioned += _onTransitioned;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_right"))
		{
			transitionScene.Transition();
		}
	}

	private void _onTransitioned()
	{
		Node2D currentScene = GetNode<Node2D>("CurrentScene");
		currentScene.GetChild(0).QueueFree();
		currentScene.AddChild(nextScene.Instantiate());
	}
	private void _GameStarted()
	{
		transitionScene.Transition();
	}
}
