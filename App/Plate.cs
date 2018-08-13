using Godot;

namespace ShortServe.App
{
    public class Plate : Node2D
    {
        public override void _Ready()
        {
        }

        public bool IsInside(Vector2 point)
        {
            return new Rect2(GlobalPosition - new Vector2(32, 32), new Vector2(64, 64)).HasPoint(point);
        }

        public void SelectPlate(string animation = "selected")
        {
            var sprite = GetNode<AnimatedSprite>("AnimatedSprite");
            sprite.Animation = animation;
        }

        public void ResetPlate()
        {
            SelectPlate("default");
        }
    }
}