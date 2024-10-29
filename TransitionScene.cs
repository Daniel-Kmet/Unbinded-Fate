using Godot;
using System;

public partial class TransitionScene : CanvasLayer
{
	AnimationPlayer transitionAnim;
	
	[Signal] public delegate void TransitionedEventHandler();

	public override void _Ready()
	{
		transitionAnim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{
	}

	public void Transition()
	{
		transitionAnim.Play("Fade To Black");
	}
	private void _onAnimationFinished(string animName)
	{
		if (animName == "Fade To Black")
		{
			EmitSignal(SignalName.Transitioned);
			transitionAnim.Play("Fade To Normal");
		}
	}
}
