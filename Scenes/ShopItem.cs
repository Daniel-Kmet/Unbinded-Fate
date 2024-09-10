using Godot;
using System;

public partial class ShopItem : Sprite2D
{
	private void _ShopItemClicked(Viewport viewport, InputEvent @event, int shape_idx)
	{
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && @event.IsPressed())
		{
			GD.Print("Hide");
			this.Visible = false;
		}
	}
}
