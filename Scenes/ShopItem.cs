using Godot;
using System;

public partial class ShopItem : Sprite2D
{
	private Label nameLabel;
	private Label priceLabel;
	private Sprite2D itemTexture;

	public override void _Ready()
	{
		nameLabel = GetNodeOrNull<Label>("HBoxContainer/Name");
		priceLabel = GetNodeOrNull<Label>("Price");
		itemTexture = GetNodeOrNull<Sprite2D>("Price/Coin");

		var area2D = GetNodeOrNull<Area2D>("Area2D");
		if (area2D != null)
		{
			area2D.Connect("input_event", new Callable(this, "_OnShopItemClicked"));
		}

		// Debugging output to confirm nodes are properly found
		GD.Print($"NameLabel: {(nameLabel != null ? "Found" : "Not Found")}");
		GD.Print($"PriceLabel: {(priceLabel != null ? "Found" : "Not Found")}");
		GD.Print($"ItemTexture: {(itemTexture != null ? "Found" : "Not Found")}");
	}

	public void SetItemName(string name)
	{
		if (nameLabel != null)
		{
			nameLabel.Text = name;
			GD.Print($"Set item name to: {name}");
		}
		else
		{
			GD.PrintErr("NameLabel is null, cannot set item name.");
		}
	}

	public void SetItemPrice(int price)
	{
		if (priceLabel != null)
		{
			priceLabel.Text = $"Price: {price}";
			GD.Print($"Set item price to: {price}");
		}
		else
		{
			GD.PrintErr("PriceLabel is null, cannot set item price.");
		}
	}

	public void SetItemTexture(Texture2D texture)
	{
		if (itemTexture != null)
		{
			this.Texture = texture;
			itemTexture.Texture = texture;
			GD.Print("Set item texture successfully.");
		}
		else
		{
			GD.PrintErr("ItemTexture is null, cannot set item texture.");
		}
	}

	private void _OnShopItemClicked(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && @event.IsPressed())
		{
			GD.Print("Item clicked, hiding it.");
			this.Visible = false;
		}
	}
}
