using System.Collections.Generic;
using System.Linq;
using Godot;
using ShortServe.App.Core;
using ShortServe.App.Food;

namespace ShortServe.App
{
    public class Order : Node2D
    {
        [Signal]
        public delegate void SpawnOrder();

        public bool EmptyOrder = false;
        
        public bool MatchFound => _matchFound;
        private bool _matchFound;

        public bool CanBeDismissed => _canBeDismissed;
        private bool _canBeDismissed = true;
        
        public int Points => _points;
        private int _points;
        public static readonly Vector2 Dimensions = new Vector2(164, 86);
        
        private Node2D _items;
        private Label _pointsLabel;
        private AnimatedSprite _animatedSprite;
        private Timer _resetAnimationTimer;
        
        private List<FoodItem> _orderFoodItems = new List<FoodItem>();

        private const int ItemYPosition = 7 + 16;
        private const int ItemPadding = 10;
        
        private Godot.Dictionary<int, int[]> _possiblePoints = new Godot.Dictionary<int, int[]>
        {
            {1, new [] {10, 10, 10, 10, 10, 20, 20, 20, 30, 30}},
            {2, new [] {40, 40, 40, 40, 40, 50, 50, 50, 60, 60}},
            {3, new [] {70, 70, 70, 70, 80, 80, 80, 90, 90, 100}}
        };
        
        public override void _Ready()
        {
            _items = GetNode<Node2D>("AnimatedSprite/Items");
            _pointsLabel = GetNode<Label>("Points");
            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            
            _resetAnimationTimer = GetNode<Timer>("Timers/ResetAnimation");
            _resetAnimationTimer.Connect("timeout", this, nameof(ResetAnimation));

            if(EmptyOrder) return;
            
            _pointsLabel.Show();
            
            SelectRandomPoints();
            SetupItemPosition();
        }

        public void AddFoodItems(List<FoodItem> items)
        {
            _orderFoodItems = items;
        }

        private void SetupItemPosition()
        {
            var width = 0;
            for(var i = 0; i < _orderFoodItems.Count; i++)
            {
                var sprite = FoodLoader.CreateSprite(_orderFoodItems[i].Texture);
                sprite.Position = new Vector2(i * 32 + i * ItemPadding, 0);
                _items.AddChild(sprite);

                var padding = i == _orderFoodItems.Count - 1 ? 0 : ItemPadding;
                width += 32 + padding;
            }
            
            // position node2d
            var halfWidth = width / 2;
            var itemPosition = 16 + Dimensions.x / 2 - halfWidth;
            
            _items.Position = new Vector2(itemPosition, ItemYPosition);
        }

        private void SelectRandomPoints()
        {
            var possiblePoints = _possiblePoints[_orderFoodItems.Count];
            _points = possiblePoints[Global.RNG.Next(0, possiblePoints.Length)];
            _pointsLabel.Text = _points.ToString();
        }
        
        public bool IsInside(Vector2 point)
        {
            return new Rect2(GlobalPosition, Dimensions).HasPoint(point);
        }

        public bool CheckMatch(List<FoodSpawn> plateItems)
        {
            var list = _orderFoodItems.ToList();
            var count = 0;
            foreach(var item in plateItems)
            {
                if(list.All(i => i.Name != item.Name))
                {
                    break;
                }
                
                ++count;
            }

            if(count == plateItems.Count)
            {
                // change background to green and emit signal to spawn another order
                ChangeColor("correct");
                _matchFound = true;
                _resetAnimationTimer.Start();
                return true;
            }
            
            // change animation to red and run timer to switch it back
            ChangeColor("incorrect");
            _canBeDismissed = false;
            _resetAnimationTimer.Start();
            return false;
        }

        private void ChangeColor(string anim)
        {
            _animatedSprite.Animation = anim;
        }

        public void ResetAnimation()
        {
            _resetAnimationTimer.Stop();
            ChangeColor("default");

            if(_matchFound)
            {
                EmitSignal(nameof(SpawnOrder), this);
                return;
            }
            
            _canBeDismissed = true;
        }
    }
}