using Godot;
using System;

public partial class MediumEnemy : Enemy
{
    public new float spawnChance = 0.3f;

    public override void _Ready()
    {
        base._Ready();
		speed = enemySpawner.mediumDemonSpeed;
    }
}
