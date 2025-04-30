using Godot;
using System;

public partial class TestChunk : Node2D
{
	[Export] public Texture2D TreeTexture { get; set; }
	[Export] public Texture2D RockTexture { get; set; }
	[Export] public int ChunkSeed { get; set; } = -1; // -1 means use random seed
	
	private const int MIN_OBJECTS = 5;
	private const int MAX_OBJECTS = 15;
	private const float CHUNK_SIZE = 256;
	
	// Define different biome probabilities
	private const float DENSE_VEGETATION_PROB = 0.2f;
	private const float SPARSE_VEGETATION_PROB = 0.3f;
	private const float ROCKY_TERRAIN_PROB = 0.2f;
	
	public override void _Ready()
	{
		var rng = new RandomNumberGenerator();
		
		// Use specified seed if provided, otherwise randomize
		if (ChunkSeed >= 0)
		{
			rng.Seed = (ulong)ChunkSeed;
		}
		else
		{
		rng.Randomize();
		}
		
		// Determine biome type for this chunk
		float biomeRoll = rng.Randf();
		int minObjects = MIN_OBJECTS;
		int maxObjects = MAX_OBJECTS;
		float treeProb = 0.5f; // Default even distribution
		
		if (biomeRoll < DENSE_VEGETATION_PROB)
		{
			// Dense forest biome - more trees, more objects
			minObjects = MAX_OBJECTS;
			maxObjects = MAX_OBJECTS * 2;
			treeProb = 0.8f;
		}
		else if (biomeRoll < DENSE_VEGETATION_PROB + SPARSE_VEGETATION_PROB)
		{
			// Sparse vegetation - fewer objects
			minObjects = MIN_OBJECTS / 2;
			maxObjects = MIN_OBJECTS;
			treeProb = 0.7f;
		}
		else if (biomeRoll < DENSE_VEGETATION_PROB + SPARSE_VEGETATION_PROB + ROCKY_TERRAIN_PROB)
		{
			// Rocky terrain - more rocks
			treeProb = 0.2f;
		}
		
		// Generate random number of objects
		int numObjects = rng.RandiRange(minObjects, maxObjects);
		
		for (int i = 0; i < numObjects; i++)
		{
			// Determine object type based on biome
			var isTree = rng.Randf() < treeProb;
			var texture = isTree ? TreeTexture : RockTexture;
			if (texture == null) continue;
			
			// Create random position within chunk bounds with padding to avoid objects on the edge
			var padding = 20.0f;
			var position = new Vector2(
				rng.RandfRange(padding, CHUNK_SIZE - padding),
				rng.RandfRange(padding, CHUNK_SIZE - padding)
			);
			
			try
			{
				// Create the StaticBody2D
				var body = new StaticBody2D();
				body.Position = position;
				
				// Set collision layer properly for obstacles
				body.CollisionLayer = 1; // Update this based on your game's collision layers
				
				// Add the sprite
				var sprite = new Sprite2D();
				sprite.Texture = texture;
				body.AddChild(sprite);
				
				// Add collision shape
				var shape = new CollisionShape2D();
				var circle = new CircleShape2D();
				circle.Radius = isTree ? 16 : 12; // Trees are slightly larger than rocks
				shape.Shape = circle;
				body.AddChild(shape);
				
				// Random rotation
				body.RotationDegrees = rng.RandfRange(0, 360);
				
				// Random scale variation
				float scale = rng.RandfRange(0.8f, 1.2f);
				body.Scale = new Vector2(scale, scale);
				
				AddChild(body);
			}
			catch (Exception e)
			{
				GD.PrintErr($"Error creating object: {e.Message}");
			}
		}
	}
} 
