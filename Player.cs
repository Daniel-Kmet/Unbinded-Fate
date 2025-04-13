using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D, IDamageable
{
	[Export] public Health playerHealth;
	[Export] public int Speed { get; set; } = 400;
	[Export] Item currentWeapon;
	[Export] Item[] availableWeapons; // Array of all available weapons
	AnimationPlayer combatAnimations;
	List<Enemy> enemiesColliding = new();
	private bool _inRange = false;
	private ProgressBar playerHealthBar;
	private ProgressBar playerDamageBar;
	private Sprite2D weaponSprite;
	private Sprite2D playerSprite;
	private double _lastFireTime = 0;
	private const float TARGET_RANGE = 1000.0f; // Maximum range to look for enemies
	private int _currentWeaponIndex = 0;
	private Vector2 _rightStickDirection = Vector2.Zero;

	public int balance = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Player: Starting initialization...");
		InitializeHealth();
		InitializeWeaponSystem();
		GD.Print("Player: Initialization complete.");
	}

	private void InitializeWeaponSystem()
	{
		GD.Print("Player: Initializing weapon system...");
		
		// Get the animation player
		combatAnimations = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (combatAnimations == null)
		{
			GD.PrintErr("Player: AnimationPlayer node not found!");
			return;
		}
		GD.Print("Player: AnimationPlayer found successfully");

		// Get the player sprite first
		playerSprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (playerSprite == null)
		{
			GD.PrintErr("Player: Sprite2D node not found!");
			return;
		}
		GD.Print("Player: Player sprite found successfully");

		// Get the weapon sprite from under the player sprite
		weaponSprite = playerSprite.GetNodeOrNull<Sprite2D>("Weapon");
		if (weaponSprite == null)
		{
			GD.PrintErr("Player: Weapon node not found under Sprite2D!");
			return;
		}
		GD.Print("Player: Weapon sprite found successfully");

		// Check if we have any weapons available
		if (availableWeapons == null || availableWeapons.Length == 0)
		{
			GD.PrintErr("Player: No weapons available! Please assign weapons in the editor.");
			return;
		}
		GD.Print($"Player: Found {availableWeapons.Length} available weapons");

		// Set initial weapon
		GD.Print("Player: Setting initial weapon...");
		_currentWeaponIndex = 0;
		SwitchWeapon(_currentWeaponIndex);
		GD.Print("Player: Weapon system initialization complete");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		_GetInput();
		MoveAndSlide();
		SetHealthbar();
		_HandleWeaponSwitch();
		_HandleControllerAiming();
	}
	
	public void InitializeHealth()
	{
		GD.Print("Player: Initializing health system...");
		// initialize health
		playerHealth = GetNode<Health>("Data/Health");
		playerHealth._SetHealth();
		GD.Print($"Player: Health initialized with {playerHealth.health}/{playerHealth.maxHealth}");

		// initialize health bar
		playerHealthBar = GetNode<ProgressBar>("Data/Healthbar");
		playerHealthBar.MaxValue = playerHealth.maxHealth;
		playerHealthBar.Value = playerHealth.health;
		GD.Print("Player: Health bar initialized");

		// initialize damage bar
		playerDamageBar = GetNode<ProgressBar>("Data/Healthbar/Damagebar");
		playerDamageBar.MaxValue = playerHealth.maxHealth;
		playerDamageBar.Value = playerHealth.health;
		GD.Print("Player: Damage bar initialized");
	}
	
	public void SetHealthbar()
	{
		playerHealthBar.Value = playerHealth.health;
		var tween = CreateTween();
		tween.TweenProperty(playerDamageBar, "value", playerHealth.health, .5);
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

	private void _HandleControllerAiming()
	{
		// Get right stick input for aiming
		_rightStickDirection = Input.GetVector("right_stick_left", "right_stick_right", "right_stick_up", "right_stick_down");
		
		// Only update if there's significant stick movement
		if (_rightStickDirection.Length() > 0.3f)
		{
			GD.Print($"Player: Right stick direction: {_rightStickDirection}");
		}
	}

	private void _HandleWeaponSwitch()
	{
		if (availableWeapons == null || availableWeapons.Length == 0)
		{
			GD.PrintErr("Player: Cannot switch weapons - no weapons available");
			return;
		}

		// Handle weapon cycling with new controller inputs
		if (Input.IsActionJustPressed("weapon_next"))
		{
			CycleWeapon(1);
		}
		else if (Input.IsActionJustPressed("weapon_prev"))
		{
			CycleWeapon(-1);
		}

		// Switch weapons with number keys 1-9
		for (int i = 0; i < 9; i++)
		{
			if (Input.IsActionJustPressed($"number_{i + 1}") && i < availableWeapons.Length)
			{
				GD.Print($"Player: Attempting to switch to weapon {i + 1}");
				SwitchWeapon(i);
			}
		}

		// Handle projectile weapon firing
		if (currentWeapon != null)
		{
			GD.Print($"Player: Current weapon: {currentWeapon.name}, Projectile scene: {currentWeapon.projectileScene}");
			if (!string.IsNullOrEmpty(currentWeapon.projectileScene))
			{
				var currentTime = Time.GetTicksMsec() / 1000.0;
				var timeSinceLastFire = currentTime - _lastFireTime;
				GD.Print($"Player: Time since last fire: {timeSinceLastFire:F2}s, Fire rate: {currentWeapon.fireRate}s");
				
				if (timeSinceLastFire >= currentWeapon.fireRate)
				{
					GD.Print("Player: Attempting to fire projectile...");
					
					// If using the right stick, fire in that direction
					if (_rightStickDirection.Length() > 0.3f)
					{
						FireProjectileInDirection(_rightStickDirection.Normalized());
					}
					else
					{
						FireProjectileAtClosestEnemy();
					}
					
					_lastFireTime = currentTime;
				}
			}
			else
			{
				GD.PrintErr("Player: Current weapon has no projectile scene assigned!");
			}
		}
		else
		{
			GD.PrintErr("Player: No current weapon selected!");
		}
	}

	private void CycleWeapon(int direction)
	{
		if (availableWeapons == null || availableWeapons.Length == 0)
			return;
			
		// Calculate the new index with wrap-around
		_currentWeaponIndex = (_currentWeaponIndex + direction + availableWeapons.Length) % availableWeapons.Length;
		GD.Print($"Player: Cycling weapon {(direction > 0 ? "forward" : "backward")} to index {_currentWeaponIndex}");
		SwitchWeapon(_currentWeaponIndex);
	}

	private Enemy FindClosestEnemy()
	{
		Enemy closestEnemy = null;
		float closestDistance = float.MaxValue;

		// Get all enemies in the scene
		var enemies = GetTree().GetNodesInGroup("enemies");
		GD.Print($"Player: Found {enemies.Count} enemies in scene");
		
		foreach (var enemy in enemies)
		{
			if (enemy is Enemy enemyNode)
			{
				float distance = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);
				GD.Print($"Player: Enemy {enemyNode.Name} at distance {distance:F2}");
				
				if (distance < closestDistance && distance <= TARGET_RANGE)
				{
					closestDistance = distance;
					closestEnemy = enemyNode;
				}
			}
		}

		if (closestEnemy != null)
		{
			GD.Print($"Player: Closest enemy found: {closestEnemy.Name} at distance {closestDistance:F2}");
		}
		else
		{
			GD.Print("Player: No enemies found in range");
		}

		return closestEnemy;
	}

	private void FireProjectileInDirection(Vector2 direction)
	{
		if (currentWeapon == null || string.IsNullOrEmpty(currentWeapon.projectileScene))
		{
			GD.PrintErr("Player: Cannot fire - invalid weapon configuration");
			return;
		}

		try
		{
			GD.Print($"Player: Loading projectile scene from: {currentWeapon.projectileScene}");
			var projectileScene = GD.Load<PackedScene>(currentWeapon.projectileScene);
			if (projectileScene == null)
			{
				GD.PrintErr("Player: Failed to load projectile scene!");
				return;
			}

			GD.Print($"Player: Instantiating projectile in direction {direction}...");
			var projectile = projectileScene.Instantiate<Projectile>();
			if (projectile == null)
			{
				GD.PrintErr("Player: Failed to instantiate projectile!");
				return;
			}

			GetTree().Root.AddChild(projectile);
			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(direction);
			projectile.Damage = currentWeapon.damage;

			GD.Print($"Player: Successfully fired projectile in direction {direction}");
		}
		catch (Exception e)
		{
			GD.PrintErr($"Player: Error firing projectile: {e.Message}");
		}
	}

	private void FireProjectileAtClosestEnemy()
	{
		if (currentWeapon == null)
		{
			GD.PrintErr("Player: Cannot fire - no current weapon!");
			return;
		}

		if (string.IsNullOrEmpty(currentWeapon.projectileScene))
		{
			GD.PrintErr($"Player: Cannot fire - weapon {currentWeapon.name} has no projectile scene!");
			return;
		}

		var closestEnemy = FindClosestEnemy();
		if (closestEnemy == null)
		{
			GD.Print("Player: No enemies in range to fire at");
			return;
		}

		// Calculate direction to the closest enemy
		var direction = (closestEnemy.GlobalPosition - GlobalPosition).Normalized();
		FireProjectileInDirection(direction);
	}

	public void SwitchWeapon(int index)
	{
		if (availableWeapons == null || availableWeapons.Length == 0)
		{
			GD.PrintErr("Player: No weapons available to switch to!");
			return;
		}

		if (index < 0 || index >= availableWeapons.Length)
		{
			GD.PrintErr($"Player: Invalid weapon index: {index}");
			return;
		}

		if (availableWeapons[index] == null)
		{
			GD.PrintErr($"Player: Weapon at index {index} is null!");
			return;
		}

		_currentWeaponIndex = index;
		currentWeapon = availableWeapons[index];
		
		if (weaponSprite != null)
		{
			weaponSprite.Texture = currentWeapon.texture;
			GD.Print($"Player: Updated weapon sprite texture to {currentWeapon.name}");
		}

		if (combatAnimations != null)
		{
			combatAnimations.CurrentAnimation = currentWeapon.animationName;
			GD.Print($"Player: Updated animation to {currentWeapon.animationName}");
		}
	}

	public void _TakeDamage(float damage)
	{
		if (playerHealth.health > 0 && playerHealth.isDead != true)
		{
			playerHealth.health -= damage;
			GD.Print($"Player: Took {damage} damage. Current health: {playerHealth.health}");
		}
		else if (playerHealth.health <= 0)
		{
			GD.Print("Player: Health depleted, calling _Die()");
			_Die();
		}
	}

	public void _Die()
	{
		GD.Print("Player: Death sequence initiated");
		playerHealth.isDead = true;
		QueueFree();
	}

	public void _onWeaponAreaEntered(Enemy enemy)
	{
		if (currentWeapon != null)
		{
			GD.Print($"Player: Weapon hit enemy for {currentWeapon.damage} damage");
			enemy._TakeDamage(currentWeapon.damage);
		}
	}
	
	private void _checkCollison()
	{
		if (enemiesColliding.Count > 0 && currentWeapon != null && combatAnimations != null)
		{
			GD.Print($"Player: {enemiesColliding.Count} enemies in range, playing attack animation");
			combatAnimations.Play(currentWeapon.animationName);
		}
		else if (combatAnimations != null)
		{
			combatAnimations.Stop();
		}
	}
	
	public void _onAttackRangeAreaEntered(Enemy enemy)
	{
		GD.Print($"Player: Enemy entered attack range. Current enemies in range: {enemiesColliding.Count + 1}");
		enemiesColliding.Add(enemy);
	}
	
	public void _onAttackRangeAreaExited(Enemy enemy)
	{
		GD.Print($"Player: Enemy exited attack range. Current enemies in range: {enemiesColliding.Count - 1}");
		enemiesColliding.Remove(enemy);
	}
}
