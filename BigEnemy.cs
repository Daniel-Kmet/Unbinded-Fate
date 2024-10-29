using Godot;
using System;

public partial class BigEnemy : Enemy
{
	public new float spawnChance = .15f;
	public Health playerHealth;

	public override void _Ready()
	{
		base._Ready();
		speed = enemySpawner.bigDemonSpeed;
	}
	
	
	private void _on_hitbox_body_entered(Player player)
	{
		player._TakeDamage(20);
	}
}
