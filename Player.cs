using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D, IDamageable
{
	[Export] public Health playerHealth;
	[Export] public int Speed { get; set; } = 400;
	[Export] Item currentWeapon;
	AnimationPlayer combatAnimations;
	List<Enemy> enemiesColliding = new();
	private bool _inRange = false;

	public int balance = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerHealth._SetHealth();
		combatAnimations = GetNode<AnimationPlayer>("AnimationPlayer");
		combatAnimations.CurrentAnimation = currentWeapon.animationName;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		_GetInput();
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		_checkCollison();
	}

	public void _GetInput()
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = inputDirection * Speed;
	}

	public void _TakeDamage(float damage)
	{
		if (playerHealth.health > 0 && playerHealth.isDead != true)
		{
			playerHealth.health -= damage;
		}
		else if (playerHealth.health <= 0)
		{
			_Die();
		}
	}

	public void _Die()
	{
		playerHealth.isDead = true;
		QueueFree();
	}

	public void _onWeaponAreaEntered(Enemy enemy)
	{
		enemy._TakeDamage(currentWeapon.damage);
	}
	private void _checkCollison()
	{
		if (enemiesColliding.Count > 0)
		{
			combatAnimations.Play(currentWeapon.animationName);
		}
		else
		{
			combatAnimations.Stop();
		}
	}
	public void _onAttackRangeAreaEntered(Enemy enemy)
	{
		enemiesColliding.Add(enemy);
	}
	public void _onAttackRangeAreaExited(Enemy enemy)
	{
		enemiesColliding.Remove(enemy);
	}
}
