using Godot;
using System;

public partial class Shop : CanvasLayer
{
	private Chest chest; // This manages the overall chest and should be exclusive to Shop.cs
	private AudioStreamPlayer shopBackgroundMusic; // Manages background music for the shop
	private AnimatedSprite2D shopCoin; // Manages the coin animation in the shop UI
	private AudioStreamPlayer battlemusic; // Manages the battle music for transitions
	private Godot.Collections.Array weapons; // Manages a list of weapons in the shop
	private PackedScene itemScene; // Reference to 'item.tscn' scene for shop items

	public override void _Ready()
	{
		// Initialize nodes specific to Shop.cs
		shopCoin = GetNode<Control>("Control").GetNode<AnimatedSprite2D>("CoinUI");
		shopBackgroundMusic = GetNode<AudioStreamPlayer>("Shop Music");
		battlemusic = GetTree().Root.GetNode<AudioStreamPlayer>("SceneManager/CurrentScene/MainGame/Music");
		chest = GetTree().Root.GetNode<Chest>("SceneManager/CurrentScene/MainGame/World/Chest");

		shopCoin.Play();
		shopBackgroundMusic.Play();
		battlemusic.Stop();

		// Load item scene and populate shop slots
		LoadWeaponsAndItems();
	}

	private void LoadWeaponsAndItems()
	{
		// Load weapons and item scene logic, ensuring the scene path is correct
		var itemShopScript = (GDScript)GD.Load("res://ItemShopCollection.gd");
		if (itemShopScript == null)
		{
			GD.PrintErr("Failed to load ItemShopCollection.gd as a script.");
			return;
		}

		var itemShopCollection = new Node();
		itemShopCollection.SetScript(itemShopScript);
		GetTree().Root.AddChild(itemShopCollection);

		weapons = (Godot.Collections.Array)itemShopCollection.Call("get_items");
		if (weapons == null || weapons.Count == 0)
		{
			GD.PrintErr("Weapons array is null or empty.");
			return;
		}
		GD.Print("Weapons Array Size: ", weapons.Count);

		itemScene = (PackedScene)GD.Load("res://Scenes/Item.tscn");
		if (itemScene == null)
		{
			GD.PrintErr("Failed to load item.tscn as PackedScene.");
			return;
		}

		PopulateShopSlots();
	}

	private void PopulateShopSlots()
{
	Random random = new Random();
	var controlNode = GetNode<Control>("Control");

	for (int i = 0; i < 4; i++) // Example: Create 4 items
	{
		var itemInstance = (ShopItem)itemScene.Instantiate();
		if (itemInstance == null)
		{
			GD.PrintErr("Failed to instance item.");
			continue;
		}

		var randomWeapon = (Godot.Collections.Dictionary)weapons[random.Next(weapons.Count)];

		// Add item to scene first, so _Ready() is called and nodes are set up properly
		controlNode.AddChild(itemInstance);

		// After adding to the scene, set the properties
		itemInstance.CallDeferred("SetItemName", randomWeapon["name"].ToString());
		itemInstance.CallDeferred("SetItemPrice", (int)randomWeapon["price"]);
		itemInstance.CallDeferred("SetItemTexture", (Texture2D)GD.Load(randomWeapon["sprite_path"].ToString()));

		GD.Print($"Added item with {randomWeapon["name"]}");
	}
}

	public void _onButtonPressed()
	{
		chest.isShopClosed = true;
		QueueFree();
	}
}
