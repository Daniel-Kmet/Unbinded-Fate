using Godot;
using System;

public partial class Balance : Label
{
	Player player;
	public override void _Ready()
	{
		player = (Player)GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/Player");
	}
	public override void _Process(double delta)
	{
		this.Text = "Coins: " + player.balance.ToString();
	}
}
