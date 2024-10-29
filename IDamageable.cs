using Godot;
using System;

public interface IDamageable
{
	void _TakeDamage(float damage);

	void _Die();
}
