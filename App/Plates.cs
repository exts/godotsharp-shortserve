using System.Collections.Generic;
using System.Linq;
using Godot;
using ShortServe.App.Core;
using ShortServe.App.Core.Engine;
using ShortServe.App.Food;

namespace ShortServe.App
{
    public class Plates : Node2D
    {
        [Signal]
        public delegate void FoundPlate();
        
        private Node2D _container;
        private PackedScene _plate;

        public const int Cols = 7;
        public const int Rows = 7;
        private const int Width = 64;
        private const int Height = 64;
        private const int PaddingCol = 10;
        private const int PaddingRow = 10;

        public List<List<Plate>> PlatesList => _plates;
        private List<List<Plate>> _plates = new List<List<Plate>>();
        
        // keep track of plates select
        public List<Vector2> PlateSpawns = new List<Vector2>();
        public List<Vector2> SelectedPlates => _selectedPlates; 
        private const int PlateLimit = 3;
        private List<Vector2> _selectedPlates = new List<Vector2>();

        private bool _leftShiftPressed;
        private bool _leftMousePressed;
        private bool _rightMousePressed;

        private AudioStreamPlayer _plateClick;

        public override void _Ready()
        {
            _plate = (PackedScene) GD.Load($"{Global.ResDataObjects}/Plate.tscn");
            _container = GetNode<Node2D>("Container");
            _plateClick = GetNode<AudioStreamPlayer>("../Audio/SFX/ButtonClick");
            
            SetupPlateGrid(out var size);
            _container.Position = new Vector2(100, (float)720/2 - size.y/2);
        }

        public override void _Process(float delta)
        {
            if(_leftShiftPressed && _rightMousePressed)
            {
                DeselectAllPlates();
                _rightMousePressed = false;
            }
            
            if(_leftMousePressed)
            {
                SelectPlates();
                _leftMousePressed = false;
            }

            if(_rightMousePressed)
            {
                DeselectLastPlate();
                _rightMousePressed = false;
            }
        }

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton button)
            {
                HandleMouseEvents(button);
            }

            if(@event is InputEventKey key)
            {
                HandleKeyboardEvents(key);
            }
        }

        private void SetupPlateGrid(out Vector2 size)
        {
            size = new Vector2();
            var keepCount = true;
            for(var r = 0; r < Rows; r++)
            {
                var row = new List<Plate>();
                for(var c = 0; c < Cols; c++)
                {
                    var colPadding = c < Cols ? c * PaddingCol : 0;
                    var rowPadding = r < Rows ? r * PaddingRow : 0;
                    var position = new Vector2(c * Width + colPadding, r * Height + rowPadding);
                    var sizeData = new Vector2(Width + colPadding, Height + rowPadding);
                    
                    var plate = _plate.Instance<Plate>();
                    plate.Position = position;
                    row.Add(plate);
                    _container.AddChild(plate);
                    
                    if(keepCount)
                        size += sizeData;
                }

                keepCount = false;
                _plates.Add(row);
            }
        }

        private void SelectPlates()
        {
            if(_selectedPlates.Count == PlateLimit) goto Exit;
            
            for(var r = 0; r < Rows; r++)
            {
                for(var c = 0; c < Cols; c++)
                {
                    var pos = new Vector2(r, c);
                    var plate = _plates[r][c];

                    if(!PlateSpawns.Contains(pos)) continue;
                    if(_selectedPlates.Contains(pos)) continue;
                    if(!plate.IsInside(Global.MousePosition())) continue;
                    
                    plate.SelectPlate();
                    _selectedPlates.Add(pos);
                    _plateClick.Play();
                    goto Exit;
                }
            }
            
            Exit:;
        }

        private void DeselectLastPlate()
        {
            if(_selectedPlates.ToList().Count == 0) return;

            var last = _selectedPlates[_selectedPlates.Count - 1];
            var plate = _plates[(int)last.x][(int)last.y];
            plate.ResetPlate();

            _selectedPlates.Remove(last);
            _plateClick.Play();
        }

        public void DeselectAllPlates()
        {
            if(_selectedPlates.Count == 0) return;

            foreach(var pos in _selectedPlates.ToList())
            {
                var plate = _plates[(int)pos.x][(int)pos.y];
                plate.ResetPlate();
                _selectedPlates.Remove(pos);
            }
        }

        private void HandleMouseEvents(InputEventMouseButton button)
        {
            if(button.ButtonIndex == ButtonList.Left.Int())
            {
                if(button.IsPressed())
                {
                    _leftMousePressed = true;
                } else if(!button.IsPressed() && _leftMousePressed)
                {
                    _leftMousePressed = false;
                }
            }
            
            if(button.ButtonIndex == ButtonList.Right.Int())
            {
                if(button.IsPressed())
                {
                    _rightMousePressed = true;
                } else if(!button.IsPressed() && _rightMousePressed)
                {
                    _rightMousePressed = false;
                }
            }
        }

        private void HandleKeyboardEvents(InputEventKey key)
        {
            if(key.Scancode == KeyList.Shift.Int())
            {
                if(key.IsPressed())
                {
                    _leftShiftPressed = true;
                } else if(!key.IsPressed() && _leftShiftPressed)
                {
                    _leftShiftPressed = false;
                }
            }
        }
        
    }
}