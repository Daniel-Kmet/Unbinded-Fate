using Godot;
using System;

public partial class Game : Node
{
	AudioStreamPlayer battlemusiclol;
	public Wave wave = new();
	Chest chest;
	EnemySpawner es;
	Enemy enemy;

	public override void _Ready()
	{
		battlemusiclol = GetTree().Root.GetNode<AudioStreamPlayer>("SceneManager/CurrentScene/MainGame/Music");
		es = (EnemySpawner)GetTree().Root.GetNode("SceneManager/CurrentScene/MainGame").GetNode("EnemySpawner");
		wave.enemiesRemaining = wave.totalEnemies;
	}

	public override void _Process(double delta)
	{
		if (GetTree().Root.HasNode("SceneManager/CurrentScene/MainGame/World/Chest"))
		{
			chest = GetNode<Chest>("World/Chest");		
			CompleteWave();
		}
	}

	public void StartGame()
	{
		StartWave();
	}
	public void StartWave()
	{
		
	}
	public void ResetWave()
	{
		wave.enemiesRemaining = wave.totalEnemies;
		es.enemiesSpawned = 0;
		es.enemies_per_second *= 1.25f;
		es.littleDemonSpeed *= 1.25f;
		es.mediumDemonSpeed *= 1.05f;
		es.bigDemonSpeed *= 1.01f;
		battlemusiclol.Play();
	}
	public void CompleteWave()
	{
		if (wave.enemiesRemaining == 0 && chest.isShopClosed == true)
		{
			chest.QueueFree();
			wave.totalEnemies += 5;
			wave.currentWave += 1;	
			ResetWave();
		}
	}
}
