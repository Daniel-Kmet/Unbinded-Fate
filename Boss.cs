using Godot;
using System;

public partial class Boss : Enemy
{
	public new float spawnChance = .05f;

	public override void _Ready()
	{
		base._Ready();
		speed = enemySpawner.bossSpeed;
	}
}
