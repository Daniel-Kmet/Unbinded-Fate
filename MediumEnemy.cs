using Godot;
using System;

public partial class MediumEnemy : Enemy
{
	public new float spawnChance = 0.3f;
	public Health playerHealth;

	public override void _Ready()
	{
		base._Ready();
		speed = enemySpawner.mediumDemonSpeed;
	}
	
	
	private void _on_area_2d_body_entered(Player player)
	{
		player._TakeDamage(15);
	}
}
