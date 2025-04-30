using Godot;
using System;
using System.Collections.Generic;

public partial class InfiniteWorldGenerator : Node2D
{
	[Export] public PackedScene VillageChunkScene { get; set; }
	[Export] public Godot.Collections.Array<PackedScene> RandomChunkScenes { get; set; } = new();
	
	[Export] public Vector2 ChunkSize { get; set; } = new Vector2(256, 256);
	[Export] public int ChunkViewRange { get; set; } = 3;
	[Export] public int MaxChunkViewRange { get; set; } = 5; // Maximum view range for load balancing
	[Export] public int WorldSeed { get; set; } = 12345;
	
	[Export] public NodePath PlayerPath { get; set; }
	
	private Node2D _player;
	private readonly Dictionary<Vector2I, Node2D> _spawnedChunks = new();
	private Vector2I _lastPlayerChunkCoord = Vector2I.Zero;
	private readonly RandomNumberGenerator _rng = new();
	
	// For performance monitoring
	private int _totalChunksGenerated = 0;
	private double _lastChunkCleanupTime = 0;
	private const double CLEANUP_INTERVAL = 5.0; // Check for chunks to unload every 5 seconds
	
	// Flag to ensure village is spawned first
	private bool _villageChunkSpawned = false;
	
	public override void _Ready()
	{
		// Set up RNG with the world seed
		_rng.Seed = (ulong)WorldSeed;
		
		// Find the player node
		if (!PlayerPath.IsEmpty)
		{
			_player = GetNode<Node2D>(PlayerPath);
		}
		else
		{
			// Try to find player in the scene if path not provided
			_player = GetTree().GetFirstNodeInGroup("Player") as Node2D;
		}
		
		if (_player == null)
		{
			GD.PrintErr("InfiniteWorldGenerator: Player not found!");
			return;
		}
		
		// Always spawn the village chunk at (0,0) first
		EnsureVillageChunkSpawned();
		
		// Generate initial chunks around the player
		GenerateChunksAroundPlayer();
		
		// Check if player is not in the village and teleport them if necessary
		if (_player != null)
		{
			Vector2I playerChunkCoord = WorldToChunkCoord(_player.GlobalPosition);
			if (playerChunkCoord != Vector2I.Zero && !ShouldPlayerStartAtRandomPosition())
			{
				// Move player to village center
				_player.GlobalPosition = new Vector2(ChunkSize.X / 2, ChunkSize.Y / 2);
				GD.Print("Player position reset to village center");
			}
		}
	}
	
	// Override this method in derived classes to allow random starting positions
	protected virtual bool ShouldPlayerStartAtRandomPosition()
	{
		return false; // By default, always start at village
	}
	
	private void EnsureVillageChunkSpawned()
	{
		GD.Print("Checking if village chunk needs to be spawned...");
		
		if (_villageChunkSpawned) {
			GD.Print("Village chunk already spawned, skipping");
			return;
		}
		
		// Village chunk is always at (0,0)
		if (VillageChunkScene != null)
		{
			GD.Print("Village chunk scene is valid, spawning at (0,0)");
			SpawnVillageChunk();
			_villageChunkSpawned = true;
		}
		else
		{
			GD.PrintErr("InfiniteWorldGenerator: Village chunk scene is null! Make sure to assign VillageChunkScene in the inspector.");
			// Try to recover by finding village chunk scene resource
			GD.Print("Attempting to recover by loading village chunk scene resource directly...");
			var villageChunkPath = "res://Environment/Chunks/Village/Village_Chunk.tscn";
			var recoveredScene = GD.Load<PackedScene>(villageChunkPath);
			if (recoveredScene != null)
			{
				GD.Print("Successfully recovered village chunk scene");
				VillageChunkScene = recoveredScene;
				SpawnVillageChunk();
				_villageChunkSpawned = true;
			}
			else
			{
				GD.PrintErr("Failed to recover village chunk scene. Village will not spawn.");
			}
		}
	}
	
	private void SpawnVillageChunk()
	{
		Vector2I villageCoord = Vector2I.Zero;
		
		// Skip if already spawned
		if (_spawnedChunks.ContainsKey(villageCoord)) {
			GD.Print("Village chunk already exists in _spawnedChunks dictionary, skipping spawn");
			return;
		}
		
		GD.Print("Attempting to instantiate village chunk from VillageChunkScene: " + (VillageChunkScene != null));
		
		// Instantiate the village chunk
		var villageChunk = VillageChunkScene.Instantiate<Node2D>();
		
		if (villageChunk == null) {
			GD.PrintErr("Failed to instantiate VillageChunk");
			return;
		}
		
		GD.Print("Successfully instantiated VillageChunk");
		
		villageChunk.Position = ChunkCoordToWorldPosition(villageCoord);
		villageChunk.ZIndex = 1; // Village above regular chunks but below player
		
		// Add it to the scene
		AddChild(villageChunk);
		_spawnedChunks[villageCoord] = villageChunk;
		_totalChunksGenerated++;
		
		GD.Print("Village chunk spawned at coordinates (0,0), world position: " + villageChunk.Position);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_player == null) return;
		
		// Make sure village is always spawned
		EnsureVillageChunkSpawned();
		
		var playerChunkCoord = WorldToChunkCoord(_player.GlobalPosition);
		
		// Only update chunks if player has moved to a different chunk
		if (playerChunkCoord != _lastPlayerChunkCoord)
		{
			GenerateChunksAroundPlayer();
			_lastPlayerChunkCoord = playerChunkCoord;
		}
		
		// Periodically check for chunks to unload
		if (GetProcessDeltaTime() > _lastChunkCleanupTime + CLEANUP_INTERVAL)
		{
			UnloadDistantChunks();
			_lastChunkCleanupTime = GetProcessDeltaTime();
		}
	}

	private void GenerateChunksAroundPlayer()
	{
		if (_player == null) return;
		
		// Always make sure village exists first
		EnsureVillageChunkSpawned();
		
		var playerChunkCoord = WorldToChunkCoord(_player.GlobalPosition);
		
		// Adjust view range based on performance (lower range if too many chunks)
		int currentViewRange = _totalChunksGenerated > 100 ? 
			Mathf.Min(ChunkViewRange, MaxChunkViewRange) : ChunkViewRange;
		
		// Generate chunks in a spiral pattern outward from the player
		// This ensures that closer chunks load first
		for (int layer = 0; layer <= currentViewRange; layer++)
		{
			// If layer is 0, just spawn the chunk the player is in
			if (layer == 0)
			{
				SpawnChunk(playerChunkCoord);
				continue;
			}
			
			// Spawn chunks in a square ring around the player
			// Top and bottom rows
			for (int x = -layer; x <= layer; x++)
			{
				// Top row
				SpawnChunk(new Vector2I(playerChunkCoord.X + x, playerChunkCoord.Y - layer));
				
				// Bottom row (skip corners to avoid duplicates)
				if (layer > 0)
				{
					SpawnChunk(new Vector2I(playerChunkCoord.X + x, playerChunkCoord.Y + layer));
				}
			}
			
			// Left and right columns (skip corners to avoid duplicates)
			for (int y = -layer + 1; y <= layer - 1; y++)
			{
				// Left column
				SpawnChunk(new Vector2I(playerChunkCoord.X - layer, playerChunkCoord.Y + y));
				
				// Right column
				SpawnChunk(new Vector2I(playerChunkCoord.X + layer, playerChunkCoord.Y + y));
			}
		}
	}
	
	private void SpawnChunk(Vector2I chunkCoord)
	{
		// Skip if the chunk is already spawned
		if (_spawnedChunks.ContainsKey(chunkCoord)) return;
		
		// If this is the village coordinates (0,0), use the village scene
		if (chunkCoord == Vector2I.Zero)
		{
			SpawnVillageChunk();
			return;
		}
		
		// For all other locations, use a random chunk based on the coordinates
		PackedScene chunkScene = GetChunkSceneForCoord(chunkCoord);
		if (chunkScene == null) return;
		
		// Instantiate the chunk
		var newChunk = chunkScene.Instantiate<Node2D>();
		
		// Set the position based on the chunk coordinates
		newChunk.Position = ChunkCoordToWorldPosition(chunkCoord);
		newChunk.ZIndex = -1; // Regular chunks below village and player
		
		// Try to set the chunk seed for consistent generation
		if (newChunk is TestChunk testChunk)
		{
			testChunk.ChunkSeed = GetChunkSeed(chunkCoord);
		}
		
		// Add the chunk to the scene and track it
		AddChild(newChunk);
		_spawnedChunks[chunkCoord] = newChunk;
		_totalChunksGenerated++;
	}
	
	private void UnloadDistantChunks()
	{
		if (_player == null) return;
		
		var playerChunkCoord = WorldToChunkCoord(_player.GlobalPosition);
		int unloadDistance = ChunkViewRange + 2; // Keep a buffer of chunks
		
		List<Vector2I> chunksToRemove = new();
		
		// Find chunks that are too far from the player
		foreach (var entry in _spawnedChunks)
		{
			var chunkCoord = entry.Key;
			
			// Never unload the village chunk
			if (chunkCoord == Vector2I.Zero) continue;
			
			var distance = Mathf.Abs(chunkCoord.X - playerChunkCoord.X) + 
						   Mathf.Abs(chunkCoord.Y - playerChunkCoord.Y);
			
			if (distance > unloadDistance)
			{
				chunksToRemove.Add(chunkCoord);
			}
		}
		
		// Remove the identified chunks
		foreach (var coord in chunksToRemove)
		{
			if (_spawnedChunks.TryGetValue(coord, out Node2D chunk))
			{
				chunk.QueueFree();
				_spawnedChunks.Remove(coord);
			}
		}
	}
	
	private PackedScene GetChunkSceneForCoord(Vector2I chunkCoord)
	{
		if (RandomChunkScenes.Count == 0)
		{
			GD.PrintErr("InfiniteWorldGenerator: No random chunk scenes assigned!");
			return null;
		}
		
		// Set a deterministic seed based on the chunk coordinates
		int seed = GetChunkSeed(chunkCoord);
		_rng.Seed = (ulong)seed;
		
		// Select a random chunk scene
		int index = (int)(_rng.Randi() % RandomChunkScenes.Count);
		return RandomChunkScenes[index];
	}
	
	private int GetChunkSeed(Vector2I chunkCoord)
	{
		// Combine the world seed with the chunk coordinates to get a unique, deterministic seed
		int x = chunkCoord.X;
		int y = chunkCoord.Y;
		
		// Use prime numbers to avoid patterns
		return WorldSeed + x * 73856093 + y * 19349663;
	}
	
	private Vector2I WorldToChunkCoord(Vector2 worldPos)
	{
		// Convert a world position to chunk coordinates
		int x = (int)Mathf.Floor(worldPos.X / ChunkSize.X);
		int y = (int)Mathf.Floor(worldPos.Y / ChunkSize.Y);
		return new Vector2I(x, y);
	}
	
	private Vector2 ChunkCoordToWorldPosition(Vector2I chunkCoord)
	{
		// Convert chunk coordinates to world position
		return new Vector2(
			chunkCoord.X * ChunkSize.X,
			chunkCoord.Y * ChunkSize.Y
		);
	}
} 
