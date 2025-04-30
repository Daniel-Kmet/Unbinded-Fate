using Godot;
using System;

public partial class Projectile : Area2D
{
    [Export] public float Speed = 500;
    [Export] public float Damage = 10;
    [Export] public float Lifetime = 2.0f; // How long the projectile lives before being destroyed
    [Export] public float PierceCount = 1; // How many enemies the projectile can hit before being destroyed
    [Export] public Color ProjectileColor = new Color(1.0f, 0.8f, 0.0f);
    
    private Vector2 _direction;
    private int _currentPierceCount = 0;
    private Sprite2D _sprite;
    private ColorRect _colorRect;
    private bool _initialized = false;

    public override void _Ready()
    {
        // Set up sprite and visual effects
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _colorRect = GetNode<ColorRect>("Sprite2D/ColorRect");
        
        // Apply visual effect
        _sprite.Modulate = ProjectileColor;
        _colorRect.Color = new Color(ProjectileColor.R, ProjectileColor.G, ProjectileColor.B, 0.8f);
        
        // Create trail effect
        CreateTrailEffect();
        
        // Set up collision detection
        Connect("body_entered", new Callable(this, nameof(_OnBodyEntered)));
        
        // Set up lifetime
        var timer = GetNode<Timer>("LifetimeTimer");
        if (timer == null)
        {
            return;
        }
        
        timer.WaitTime = Lifetime;
        timer.Connect("timeout", new Callable(this, nameof(_OnLifetimeExpired)));
        timer.Start();
    }

    private void CreateTrailEffect()
    {
        // Create a simple trail using CPUParticles2D
        var particles = new CpuParticles2D();
        AddChild(particles);
        
        // Configure particle properties
        particles.Amount = 20;
        particles.Lifetime = 0.5f;
        particles.OneShot = false;
        particles.Explosiveness = 0.0f;
        particles.Direction = new Vector2(0, 0);
        particles.Spread = 45.0f;
        particles.Gravity = new Vector2(0, 0);
        particles.InitialVelocityMin = 20.0f;
        particles.InitialVelocityMax = 30.0f;
        particles.Scale = new Vector2(1.0f, 1.0f);
        particles.Color = ProjectileColor;
        particles.Emitting = true;
    }

    public void Initialize(Vector2 direction)
    {
        _direction = direction.Normalized();
        Rotation = _direction.Angle();
        _initialized = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_initialized) return;
        
        Position += _direction * Speed * (float)delta;
        
        // Add slight rotation to make it more dynamic
        _sprite.Rotation += (float)delta * 4.0f;
    }

    private void _OnBodyEntered(Node2D body)
    {
        if (body is Enemy enemy)
        {
            enemy._TakeDamage(Damage);
            _currentPierceCount++;

            // Create hit effect
            CreateHitEffect();

            if (_currentPierceCount >= PierceCount)
            {
                QueueFree();
            }
        }
    }
    
    private void CreateHitEffect()
    {
        // Create a simple hit effect
        var particles = new CpuParticles2D();
        GetTree().Root.AddChild(particles);
        particles.GlobalPosition = GlobalPosition;
        
        // Configure particle properties for explosion effect
        particles.Amount = 15;
        particles.Lifetime = 0.5f;
        particles.OneShot = true;
        particles.Explosiveness = 1.0f;
        particles.Direction = new Vector2(0, 0);
        particles.Spread = 180.0f;
        particles.Gravity = new Vector2(0, 0);
        particles.InitialVelocityMin = 50.0f;
        particles.InitialVelocityMax = 80.0f;
        particles.Scale = new Vector2(1.5f, 1.5f);
        particles.Color = ProjectileColor;
        particles.Emitting = true;
        
        // Auto-delete after emission
        var timer = new Timer();
        particles.AddChild(timer);
        timer.WaitTime = 0.6f;
        timer.Connect("timeout", new Callable(particles, "queue_free"));
        timer.Start();
    }

    private void _OnLifetimeExpired()
    {
        QueueFree();
    }
} 