using Godot;
using System;

public partial class Tree : StaticBody2D
{
	public override void _Ready()
	{
		// Add collision shape
		var shape = new CollisionShape2D();
		var circle = new CircleShape2D();
		circle.Radius = 16; // Adjust based on your sprite size
		shape.Shape = circle;
		AddChild(shape);
	}
} 
