using Godot;
using System;

public partial class Rock : StaticBody2D
{
	public override void _Ready()
	{
		// Add collision shape
		var shape = new CollisionShape2D();
		var circle = new CircleShape2D();
		circle.Radius = 12; // Slightly smaller than trees
		shape.Shape = circle;
		AddChild(shape);
	}
} 
