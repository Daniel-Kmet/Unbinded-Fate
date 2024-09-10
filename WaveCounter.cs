using Godot;
using System;

public partial class WaveCounter : Label
{
	Game game;
	public override void _Ready()
	{
		game = (Game)GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame");
	}
	public override void _Process(double delta)
	{
		this.Text = "Wave: " + game.wave.currentWave.ToString();
	}
}
