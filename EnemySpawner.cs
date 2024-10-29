using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	Game game;
	[Export] Node2D[] spawn_points;
	public float enemies_per_second = 1f;
	public float littleDemonSpeed = 100f;
	public float mediumDemonSpeed = 85f;
	public float bigDemonSpeed = 70f;
	public float mageSpeed = 0f;
	public float bossSpeed = 100f;
	public int enemiesSpawned = 0;

	private float spawn_rate;

	private float time_until_spawn;

	public override void _Ready()
	{
		game = (Game)GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame");
		spawn_rate = 1 / enemies_per_second;
	}

	public override void _Process(double delta)
	{
		if (time_until_spawn > spawn_rate)
		{
			Spawn();
			time_until_spawn = 0;
		}
		else
		{
			time_until_spawn += (float)delta;
		}
		spawn_rate = 1 / enemies_per_second;
	}

	private void Spawn()
	{
		if (enemiesSpawned < game.wave.totalEnemies && game.wave.currentWave % 2 != 0)
		{
			RandomNumberGenerator enemyRng = new();
			float randomFloat = enemyRng.RandfRange(0, 1);
			MediumEnemy medium = new();
			BigEnemy big = new();
			Mage mage = new();
			//Boss boss = new();
			
			if (randomFloat <= medium.spawnChance)
			{
				randomFloat = 0;
				RandomNumberGenerator rng = new RandomNumberGenerator();
				Vector2 location = spawn_points[rng.Randi() % spawn_points.Length].GlobalPosition;
				PackedScene monsterScene = ResourceLoader.Load<PackedScene>("res://MediumDemon.tscn");
				medium = (MediumEnemy)monsterScene.Instantiate();
				medium.GlobalPosition = location;
				GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/World").AddChild(medium);
				enemiesSpawned += 1;
			}
			if (randomFloat <= big.spawnChance)
			{
				RandomNumberGenerator rng = new RandomNumberGenerator();
				Vector2 location = spawn_points[rng.Randi() % spawn_points.Length].GlobalPosition;
				PackedScene monsterScene = ResourceLoader.Load<PackedScene>("res://BigDemon.tscn");
				big = (BigEnemy)monsterScene.Instantiate();
				big.GlobalPosition = location;
				GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/World").AddChild(big);
				enemiesSpawned += 1;
			}
			if (randomFloat <= mage.spawnChance)
			{
				RandomNumberGenerator rng = new RandomNumberGenerator();
				Vector2 location = spawn_points[rng.Randi() % spawn_points.Length].GlobalPosition;
				PackedScene monsterScene = ResourceLoader.Load<PackedScene>("res://Mage.tscn");
				mage = (Mage)monsterScene.Instantiate();
				mage.GlobalPosition = location;
				GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/World").AddChild(mage);
				enemiesSpawned += 1;
			}

			else
			{
				RandomNumberGenerator rng = new RandomNumberGenerator();
				Vector2 location = spawn_points[rng.Randi() % spawn_points.Length].GlobalPosition;
				PackedScene monsterScene = ResourceLoader.Load<PackedScene>("res://Demon.tscn");
				Enemy basic = (Enemy)monsterScene.Instantiate();
				basic.GlobalPosition = location;
				GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/World").AddChild(basic);
				enemiesSpawned += 1;
			}
		}
		if (game.wave.currentWave % 2 == 0)
		{
				Boss boss = new();
				RandomNumberGenerator rng = new RandomNumberGenerator();
				Vector2 location = spawn_points[0 % spawn_points.Length].GlobalPosition;
				PackedScene monsterScene = ResourceLoader.Load<PackedScene>("res://Boss.tscn");
				boss = (Boss)monsterScene.Instantiate();
				boss.GlobalPosition = location;
				GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame/World").AddChild(boss);
				//enemiesSpawned = 1;
		}
	}
}
