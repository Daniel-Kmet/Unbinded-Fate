using Godot;
using System;

public partial class LittleEnemy : Enemy
{
    public override void _Ready()
    {
        base._Ready();
		speed = enemySpawner.littleDemonSpeed;
    }
}
