using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using ShortServe.App.Core;
using ShortServe.App.Core.Engine;
using ShortServe.App.Food;
using ShortServe.App.Orders;

namespace ShortServe.App
{
    public class Game : Node
    {
        private Plates _plates;
        private FoodLoader _foodLoader;
        private FoodSpawner _foodSpawner;
        private OrderCreator _orderCreator;
        
        private Timer _foodSpawnerTimer;
        private Node2D _foodContainer;
        private Node2D _orderContainer;
        private Node2D _scoreContainer;
        private Node2D _gameOverHover;

        private const int StartOrderCount = 6;
        
        private MouseHandler _mouseHandler = new MouseHandler();

        private int _points = 0;
        private bool _gameRunning;

        private int _highscore = 0;
        private Highscore _highscoreLoader = new Highscore();

        private AudioStreamPlayer _soundtrack;
        private AudioStreamPlayer _gameoverSoundtrack
            ;
        private AudioStreamPlayer _itemClick;
        private AudioStreamPlayer _buttonClick;

        private LinkButton _twitter1;
        private LinkButton _twitter2;

        public override void _Ready()
        {
            _foodLoader = new FoodLoader(true);
            _orderCreator = new OrderCreator(_foodLoader);
            
            _plates = GetNode<Plates>("Plates");

            _foodContainer = GetNode<Node2D>("FoodContainer");
            _orderContainer = GetNode<Node2D>("OrderContainer");
            
            _foodSpawnerTimer = GetNode<Timer>("Timers/FoodSpawner");
            _foodSpawnerTimer.Connect("timeout", this, nameof(SpawnFood));

            _gameOverHover = GetNode<Node2D>("GameOverHover");

            _scoreContainer = GetNode<Node2D>("ScoreContainer");
            _scoreContainer.GetNode<Button>("QuitGameButton").Connect("pressed", this, nameof(QuitGame));
            _scoreContainer.GetNode<Button>("NewGameButton").Connect("pressed", this, nameof(StartNewGame));

            _twitter1 = GetNode<LinkButton>("Twitter");
            _twitter2 = GetNode<LinkButton>("Twitter2");
            _twitter1.Connect("pressed", this, nameof(Twitter), new Array {"G4MR"});
            _twitter2.Connect("pressed", this, nameof(Twitter), new Array {"IMG4MR"});

            var audio = GetNode<Node>("Audio");
            _soundtrack = audio.GetNode<AudioStreamPlayer>("Soundtrack");
            _gameoverSoundtrack = audio.GetNode<AudioStreamPlayer>("Gameover");
            _itemClick = audio.GetNode<AudioStreamPlayer>("SFX/ItemClick");
            _buttonClick = audio.GetNode<AudioStreamPlayer>("SFX/ButtonClick");

            _foodSpawner = new FoodSpawner(_plates, _foodLoader, _foodContainer);
            
            SpawnEmptyStartOrders();

            LoadHighscore();
        }

        public override void _Process(float delta)
        {
            if(!_gameRunning) return;
            
            CheckGameOver();
            
            if(_mouseHandler.ButtonPressed(ButtonList.Left))
            {
                CheckOrderClick();
                _mouseHandler.ResetButton(ButtonList.Left);
            }

            if(_mouseHandler.ButtonPressed(ButtonList.Right))
            {
                DismissOrder();
                _mouseHandler.ResetButton(ButtonList.Right);
            }
        }

        public override void _Input(InputEvent @event)
        {
            if(!_gameRunning) return;
            
            if(@event is InputEventMouseButton button)
            {
                _mouseHandler.HandleMouseEvents(button);
            }
        }

        public void StartNewGame()
        {
            _points = 0;
            UpdateScore();
            
            _soundtrack.Play();
            _buttonClick.Play();
            
            _gameOverHover.GetNode<Label>("GameOver").Hide();
            _gameOverHover.GetNode<Label>("StartGame").Hide();
            _gameOverHover.GetNode<Sprite>("Background").Hide();

            _scoreContainer.GetNode<Button>("NewGameButton").Text = "Restart";

            _foodSpawner.DespawnAllPlates();
            PurgeOrders();
            SpawnStartOrders();
            
            _gameRunning = true;
            _foodSpawnerTimer.Start();
        }

        public void QuitGame()
        {
            _buttonClick.Play();
            Global.App().Quit();
        }
        
        public void SpawnFood()
        {
            _foodSpawner.SpawnPlateItem(FoodItems());
        }

        private void SpawnStartOrders()
        {
            var height = 0;
            for(var i = 0; i < StartOrderCount; i++)
            {
                CreateOrder(new Vector2(620, i * Order.Dimensions.y + i * 10));
                
                var padding = i == StartOrderCount - 1 ? 0 : 10;
                height += (int) Order.Dimensions.y + padding;
            }
            
            _orderContainer.Position = new Vector2(0, 1280/2 - height/2 - height/2);
        }

        private void SpawnEmptyStartOrders()
        {
            var height = 0;
            for(var i = 0; i < StartOrderCount; i++)
            {
                CreateEmptyOrder(new Vector2(620, i * Order.Dimensions.y + i * 10));
                
                var padding = i == StartOrderCount - 1 ? 0 : 10;
                height += (int) Order.Dimensions.y + padding;
            }
            
            _orderContainer.Position = new Vector2(0, 1280/2 - height/2 - height/2);
        }

        private void PurgeOrders()
        {
            foreach(var current in _orderContainer.GetChildren().ToList())
            {
                if(current is Order order)
                {
                    order.QueueFree();
                }
            }
        }

        public void SpawnOrder(object obj)
        {
            if(obj is Order order)
            {
                // add to score
                _points += order.Points;
                
                // create new sprite in its place
                var position = order.Position;
                _orderContainer.RemoveChild(order);
                CreateOrder(position);
                
                // delete food
                _foodSpawner.DespawnSelectedPlates();
                
                UpdateScore();
            }
        }

        private void CreateOrder(Vector2 position)
        {
            var order = _orderCreator.CreateOrder(_foodSpawner.FoodSpawns);
            order.Connect("SpawnOrder", this, nameof(SpawnOrder));
            order.Position = position;
            _orderContainer.AddChild(order);
        }

        private void CreateEmptyOrder(Vector2 position)
        {
            var order = _orderCreator.CreateEmptyOrder();
            order.Position = position;
            _orderContainer.AddChild(order);
        }

        private void CheckOrderClick()
        {
            var selectedPlates = _foodSpawner.GetSelectedPlates();
            if(!selectedPlates.Any()) return;
            
            _itemClick.Play();
            
            foreach(var iter in _orderContainer.GetChildren())
            {
                if(!(iter is Order order) || !order.IsInside(Global.MousePosition()) || order.MatchFound) continue;
                if(!order.CheckMatch(selectedPlates))
                {
                    _plates.DeselectAllPlates();
                }
                    
                break;
            }
        }

        private void DismissOrder()
        {
            foreach(var iter in _orderContainer.GetChildren())
            {
                if(!(iter is Order order) || !order.IsInside(Global.MousePosition()) || !order.CanBeDismissed) continue;

                // create new sprite in its place
                var position = order.Position;
                _orderContainer.RemoveChild(order);
                CreateOrder(position);
                break;
            }
        }

        private void UpdateScore()
        {
            GetNode<Label>("Score").Text = _points.ToString();
        }

        private void UpdateHighScore(int highscore)
        {
            _highscore = highscore;
            _scoreContainer.GetNode<Label>("HighscoreScore").Text = highscore.ToString();
        }

        private void CheckGameOver()
        {
            if(_foodSpawner.GetAvailableSpawns().Any()) return;
            
            _plates.DeselectAllPlates();
            
            _soundtrack.Stop();
            _gameoverSoundtrack.Play();
            
            _foodSpawnerTimer.Stop();
            
            _gameRunning = false;

            _gameOverHover.GetNode<Sprite>("Background").Show();
            _gameOverHover.GetNode<Label>("GameOver").Show();

            PurgeOrders();
            SpawnEmptyStartOrders();
                
            _foodSpawner.DespawnAllPlates();
                
            // update highscore
            if(_points <= _highscore) return;
            
            UpdateHighScore(_points);
            _highscoreLoader.SaveHighscore(_points);
        }

        private void LoadHighscore()
        {
            _highscore = _highscoreLoader.LoadHighscore();
            UpdateHighScore(_highscore);
        }

        private List<FoodItem> FoodItems()
        {
            var list = new List<FoodItem>();
            foreach(var item in _orderContainer.GetChildren().ToList())
            {
                if(item is Order order)
                {
                    list.AddRange(order.OrderFoodItems);
                }
            }

            return list;
        }

        public void Twitter(string handle)
        {
            OS.ShellOpen($"http://twitter.com/{handle}");
        }
        
    }
}