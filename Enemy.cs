using Godot;
using System;

public partial class Enemy : CharacterBody2D, IDamageable
{
	protected Player player;
	protected Game game;
	public float spawnChance;
	protected AudioStreamPlayer2D deathSfx;
	protected AnimatedSprite2D enemyAnim;
	protected AnimationPlayer _hitFlash;
	protected CpuParticles2D deathVfx;
	protected EnemySpawner enemySpawner;
	public Health enemyHealth;
	private ProgressBar _enemyHealthBar;
	private ProgressBar _enemyDamageBar;
	protected float speed;

	public override void _Ready()
	{
		// set health
		InitializeHealth();
		// set animations
		enemyAnim = GetNode<AnimatedSprite2D>("Enemy Animations");

		_hitFlash = GetNode<AnimationPlayer>("HitFlashAnimationPlayer");
		// find player node
		player = (Player)GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame").GetNode("Player");
		game = (Game)GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame");
		deathSfx = GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame").GetNode("Sfx").GetNode<AudioStreamPlayer2D>("Basic Enemy Death Sfx");
		enemySpawner = (EnemySpawner)GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/EnemySpawner");
	}

	public override void _Process(double delta)
	{
		SetHealthbar();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player != null)
		{
			Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * speed;
			enemyAnim.Play("Walking");
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	public void _TakeDamage(float damage)
	{
		if (enemyHealth.health > 0 && enemyHealth.isDead != true)
		{
			enemyHealth.health -= damage;
			_hitFlash.Play("hit_flash");
		}
		if (enemyHealth.health <= 0)
		{
			deathSfx.Play();
			_Die();
		}
	}

	public void InitializeHealth()
	{
		// initialize health
		enemyHealth = GetNode<Health>("Data/Health");
		enemyHealth._SetHealth();

		// initialize health bar
		_enemyHealthBar = GetNode<ProgressBar>("Data/Healthbar");
		_enemyHealthBar.MaxValue = enemyHealth.maxHealth;
		_enemyHealthBar.Value = enemyHealth.health;

		// initialize damage bar
		_enemyDamageBar = GetNode<ProgressBar>("Data/Healthbar/Damagebar");
		_enemyDamageBar.MaxValue = enemyHealth.maxHealth;
		_enemyDamageBar.Value = enemyHealth.health;
	}

	public void SetHealthbar()
	{
		_enemyHealthBar.Value = enemyHealth.health;
		var tween = CreateTween();
		tween.TweenProperty(_enemyDamageBar, "value", enemyHealth.health, .5);
	}

	public void _Die()
	{
		enemyHealth.isDead = true;
		game.wave.enemiesRemaining -= 1;
		_SpawnParticles();
		if (game.wave.enemiesRemaining > 0)
		{
			_SpawnLoot();
		}
		if (game.wave.enemiesRemaining == 0)
		{
			SpawnChest();
		}
		QueueFree();
	}
	private void _SpawnParticles()
	{
		PackedScene particles = ResourceLoader.Load<PackedScene>("res://EnemyAshEffect.tscn");
		deathVfx = (CpuParticles2D)particles.Instantiate();
		deathVfx.GlobalPosition = GlobalPosition;
		GetTree().Root.AddChild(deathVfx);
	}
	private void _SpawnLoot()
	{
		PackedScene loot = ResourceLoader.Load<PackedScene>("res://Coin.tscn");
		Coin item = (Coin)loot.Instantiate();
		item.GlobalPosition = GlobalPosition;
		GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame").AddChild(item);
	}
	private void SpawnChest()
	{
		PackedScene chest = ResourceLoader.Load<PackedScene>("res://Scenes/Game/Chest.tscn");
		Chest lootChest = (Chest)chest.Instantiate();
		lootChest.GlobalPosition = GlobalPosition;
		GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame").GetNode("World").AddChild(lootChest);
	}
}
