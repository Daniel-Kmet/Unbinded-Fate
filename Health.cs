using Godot;
using System;

public partial class Health : Node2D
{
	public float health;
	[Export] public float maxHealth = 100;
	[Export] public int armorHealth;
	[Export] public float armorDmgReduction = 25.00f;
	[Export] public bool isDead = false;
	
	public void _SetHealth()
	{
		health = maxHealth;
	}
}
