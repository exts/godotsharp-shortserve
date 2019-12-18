using Godot;
using ShortServe.App.Core;

namespace ShortServe.App
{
    public class Splash : Node
    {
        [Export]
        public float StartAfter = 1.5f;
        
        private float _counter;
        private ColorRect _fadeTransition;
        private AnimationPlayer _animationPlayer;
        
        private bool _skipAnimation;

        public override void _Ready()
        {
            _fadeTransition = GetNode<ColorRect>("Fade");
            _fadeTransition.Show();
            
            _animationPlayer = _fadeTransition.GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Connect("animation_finished", this, nameof(SwitchScene));
        }

        public override void _Process(float delta)
        {
            PlayAnimation(delta);
        }

        public override void _Input(InputEvent @event)
        {
            if(!_skipAnimation && (@event is InputEventKey key && key.IsPressed() 
               || @event is InputEventMouseButton button && button.IsPressed()))
            {
                _skipAnimation = true;
                SwitchScene();
            }
        }

        public void SwitchScene(string name = null)
        {
            _animationPlayer.Stop();
            Global.Instance.LoadStartScene();
        }

        private void PlayAnimation(float delta)
        {
            if(_skipAnimation) return;
            if(_animationPlayer.IsPlaying()) return;
            
            if(_counter < StartAfter)
            {
                _counter += delta;
                GD.Print(_counter);
            }

            if(_counter >= StartAfter)
            {
                GD.Print("play animation");
                _animationPlayer.Play("Fade");
            }
        }
    }
}
