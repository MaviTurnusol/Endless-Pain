using Godot;
using System;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    PackedScene Fistscene;

    float _hspeed = 0f;
    float _dspeed = 0f;
    float _vspeed = 16f;
    float gravity = 0f;
    Vector2 velocity;
    bool dashready = true;
    string state = "idle";
    bool fistis = true;


    Vector2 upDirection = Vector2.Up;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Fistscene = GD.Load<PackedScene>("res://Scenes/Fist.tscn");

        Timer timer = this.GetNode<Timer>("Timer");
        timer.WaitTime = (float) 0.5f;
        timer.Connect("timeout", this, "on_timeout");

        Timer timer2 = this.GetNode<Timer>("Timer");
        timer2.WaitTime = (float)0.5f;
        timer2.Connect("timeout", this, "on_timeout2");

        TextureProgress stamina = GetParent().GetNode<CanvasLayer>("CanvasLayer").GetChild<TextureProgress>(0);
        stamina.Value = 1000.0;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        TextureProgress stamina = GetParent().GetNode<CanvasLayer>("CanvasLayer").GetChild<TextureProgress>(0);
        stamina.Value += 1;

        if (Input.IsActionPressed("ui_left"))
        {
            _hspeed = -10f;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            _hspeed = 10f;
        }
        else if(_hspeed > 0)
        {
            _hspeed -= 0.5f;
        }
        else if(_hspeed < 0)
        {
            _hspeed += 0.5f;
        }
        if(IsOnFloor())
        {
            GD.Print(_hspeed);
        }

        
        //jump
      //  if (Input.IsKeyPressed((int)KeyList.X) && IsOnFloor() == true)
       // {
      //       _vspeed = -1400f;
      //  }
      //  if (Input.IsKeyPressed((int)KeyList.Down))
      //  {
      //      _vspeed = 0f;
      //  }

        //dash
        Timer timer = this.GetNode<Timer>("Timer");
        if(Input.IsActionJustPressed("dash") && dashready == true && stamina.Value > 333)
        {
            stamina.Value -= 333;
            if(_hspeed >= 0f)
            _dspeed = 10f;
            else {_dspeed = -10f;}
            dashready = false;
            state = "dashing";
            gravity = 0f;
            timer.Start();
        }

        //gravity
        if(!IsOnFloor() && gravity < 640f)
        {
            gravity += 6f;
            _vspeed = 0f;
        } else {
            gravity = 640f;
            if(_vspeed < 0)
            {
                _vspeed += 24f;
            }
        }

        velocity = MoveAndSlide(new Vector2((_hspeed + _dspeed) * 50, (_vspeed + gravity)), Vector2.Up);
    }

    public void on_timeout()
    {
        if(state == "dashing")
        {
            _dspeed = 0f;
            state = "idle";
            gravity = 640f;
        }

        if(IsOnFloor())
        {
            dashready = true;
        }
    }
    public void on_timeout2()
    {
        fistis = true;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.X && IsOnFloor() == true)
            {
                _vspeed = -1400f;
            }
            if (eventKey.IsActionReleased("Jump") && eventKey.Scancode == (int)KeyList.X)
            {
                _vspeed = 0f;
            }
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.C && fistis)
            {
                Timer timer2 = this.GetNode<Timer>("Timer");
                Fist fist = (Fist)Fistscene.Instance();
                fist.Position = Position;
                fist.Rotation = Rotation;
                GetParent().AddChild(fist);
                GetTree().SetInputAsHandled();
                fistis = false;
                timer2.Start();
            }
        }

    }
}
