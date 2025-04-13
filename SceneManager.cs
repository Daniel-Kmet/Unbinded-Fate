using Godot;
using System;

public partial class SceneManager : Node
{
	[Export] TransitionScene transitionScene;
	[Export] string nextScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (transitionScene != null)
		{
			transitionScene.Connect("Transitioned", new Callable(this, "_onTransitioned"));
			GD.Print("TransitionScene connected successfully");
		}
		else
		{
			GD.PrintErr("TransitionScene reference is null! Please assign it in the editor.");
			
			// Try to find it in the scene if not assigned
			transitionScene = GetNode<TransitionScene>("TransitionScene");
			if (transitionScene != null)
			{
				transitionScene.Connect("Transitioned", new Callable(this, "_onTransitioned"));
				GD.Print("TransitionScene found and connected automatically");
			}
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Only trigger transition with dedicated scene_transition action (space/enter/start/back)
		if (transitionScene != null && Input.IsActionJustPressed("scene_transition"))
		{
			transitionScene.Transition();
		}
	}
	
	private void _onTransitioned()
	{
		if (string.IsNullOrEmpty(nextScene))
		{
			GD.PrintErr("No next scene path specified! Please set the nextScene property in the editor.");
			return;
		}
		
		var nextSceneResource = GD.Load<PackedScene>(nextScene);
		if (nextSceneResource == null)
		{
			GD.PrintErr($"Failed to load scene: {nextScene}");
			return;
		}
		
		var nextSceneInstance = nextSceneResource.Instantiate<Node>();
		
		var CurrentScene = GetTree().Root.GetNode("SceneManager/CurrentScene");
		if (CurrentScene == null)
		{
			GD.PrintErr("Could not find CurrentScene node!");
			return;
		}
		
		foreach (var child in CurrentScene.GetChildren())
		{
			child.QueueFree();
		}
		
		CurrentScene.AddChild(nextSceneInstance);
	}
}
