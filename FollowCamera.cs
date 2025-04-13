using Godot;
using System;

public partial class FollowCamera : Camera2D
{
	// Update from TileMap to Node2D to handle multiple TileMapLayer nodes
	[Export] public Node2D worldMap;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// If you need specific TileMapLayer functionality, you can get them like this:
		// var tileMapLayers = worldMap.GetChildren().OfType<TileMapLayer>().ToArray();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
