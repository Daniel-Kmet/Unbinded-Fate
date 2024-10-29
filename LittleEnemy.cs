using Godot;
using System;



public partial class LittleEnemy : Enemy, IDamageable
{
	
	public Health playerHealth;
	
	public override void _Ready()
	{
		base._Ready();
		speed = enemySpawner.littleDemonSpeed;
	}
	
	
	private void _on_area_2d_body_entered(Player player)
	{
		player._TakeDamage(10);
	}
}
