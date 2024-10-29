using Godot;
using System;

public partial class Mage : Enemy
{
	public new float spawnChance = .15f;

	public override void _Ready()
	{
		base._Ready();
		speed = enemySpawner.mageSpeed;
	}
}
