using Godot;
using System;

public partial class Coin : Area2D
{
	AnimatedSprite2D spinAnim;
	public int value = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spinAnim = GetNode<AnimatedSprite2D>("Coin Animations");	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		spinAnim.Play("Spinning Coin");
	}
	private void _on_body_entered(Player player)
	{
		if (player.Name == "Player")
		{
			player.balance += value;
			QueueFree();
		}
	}
}
