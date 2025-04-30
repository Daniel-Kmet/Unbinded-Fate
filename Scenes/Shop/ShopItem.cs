using Godot;
using System;

public partial class ShopItem : Sprite2D
{
	[Export] private Label nameLabel;
	[Export] private Label priceLabel;
	[Export] private Label costLabel;
	[Export] private Sprite2D itemTexture;
	private int price;
	private string itemName;
	private Texture2D itemSprite;
	private Player player;

	public override void _Ready()
	{
		// Get the player reference
		player = GetTree().Root.GetNode<Player>("SceneManager/CurrentScene/MainGame/World/Player");

		var area2D = GetNodeOrNull<Area2D>("Area2D");
		if (area2D != null)
		{
			area2D.Connect("input_event", new Callable(this, "_OnShopItemClicked"));
		}

		// We'll set export properties if they're null
		if (nameLabel == null) nameLabel = GetNodeOrNull<Label>("HBoxContainer/Name");
		if (priceLabel == null) priceLabel = GetNodeOrNull<Label>("Price");
		if (costLabel == null) costLabel = GetNodeOrNull<Label>("Price/Cost");
		if (itemTexture == null) itemTexture = this; // Use this sprite if no specific texture node

		// Clear default values
		ClearDefaultValues();

		// Log node paths for debugging
		GD.Print($"ShopItem initialized. nameLabel: {nameLabel != null}, priceLabel: {priceLabel != null}, costLabel: {costLabel != null}, itemTexture: {itemTexture != null}");
	}

	private void ClearDefaultValues()
	{
		if (nameLabel != null) nameLabel.Text = "";
		if (priceLabel != null) priceLabel.Text = "Price:";
		if (costLabel != null) costLabel.Text = "";
		
		// Set this sprite's texture to null
		this.Texture = null;
	}

	public void SetItemName(string name)
	{
		GD.Print($"Setting item name to: {name}");
		itemName = name;
		if (nameLabel != null)
		{
			nameLabel.Text = name;
			GD.Print($"Name label set to: {nameLabel.Text}");
		}
		else
		{
			GD.PrintErr("nameLabel is null in SetItemName");
		}
	}

	public void SetItemPrice(int price)
	{
		GD.Print($"Setting item price to: {price}");
		this.price = price;
		if (priceLabel != null && costLabel != null)
		{
			priceLabel.Text = "Price:";
			costLabel.Text = price.ToString();
			GD.Print($"Price label set to: {costLabel.Text}");
		}
		else
		{
			GD.PrintErr("priceLabel or costLabel is null in SetItemPrice");
		}
	}

	public void SetItemTexture(Texture2D texture)
	{
		GD.Print($"Setting item texture: {texture != null}");
		itemSprite = texture;
		
		// Always set this sprite's texture
		this.Texture = texture;
		GD.Print($"Main sprite texture set: {this.Texture != null}");
	}

	private void _OnShopItemClicked(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && @event.IsPressed())
		{
			// Check if player has enough balance
			if (player.balance >= price)
			{
				GD.Print($"Player attempting to purchase {itemName} for {price} coins. Current balance: {player.balance}");
				
				// Deduct price from player's balance
				player.balance -= price;
				GD.Print($"Player balance after purchase: {player.balance}");
				
				// Create new weapon item
				var newWeapon = new Item();
				newWeapon.name = itemName;
				newWeapon.texture = itemSprite;
				newWeapon.damage = 1; // Default damage
				newWeapon.animationName = $"{itemName} Attack Animations";
				
				// Add weapon to player's available weapons
				var weapons = player.availableWeapons;
				if (weapons == null)
				{
					GD.Print("Player has no weapons, creating new array");
					weapons = new Item[0];
				}
				
				int newIndex = weapons.Length;
				var newWeapons = new Item[weapons.Length + 1];
				Array.Copy(weapons, newWeapons, weapons.Length);
				newWeapons[newIndex] = newWeapon;
				player.availableWeapons = newWeapons;
				
				GD.Print($"Added {itemName} to player's weapons at index {newIndex}");
				
				// Switch to the new weapon
				GD.Print($"Switching to weapon at index {newIndex}");
				player.SwitchWeapon(newIndex);
				
				// Update UI elements
				UpdateUIAfterPurchase();
				
				// Hide the shop item so it can't be purchased again
				this.Visible = false;
				
				GD.Print($"Successfully purchased {itemName} for {price} coins");
			}
			else
			{
				GD.Print($"Not enough coins to purchase this item. Required: {price}, Available: {player.balance}");
			}
		}
	}

	private void UpdateUIAfterPurchase()
	{
		// Find the balance display in the shop UI and update it
		var shop = GetParent() as Shop;
		if (shop != null)
		{
			var coinLabel = shop.GetNode<Control>("Control").GetNodeOrNull<Label>("Coins");
			if (coinLabel != null)
			{
				coinLabel.Text = player.balance.ToString();
				GD.Print($"Updated coin display to {player.balance}");
			}
		}
	}
}
