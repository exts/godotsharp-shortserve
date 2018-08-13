using System.Collections.Generic;
using System.Linq;
using Godot;
using ShortServe.App.Core;

namespace ShortServe.App.Food
{
    public struct FoodSpawn
    {
        public string Name;
        public Vector2 Position;
        public Sprite Sprite;
    }
    
    public class FoodSpawner
    {
        private Plates _plates;
        private FoodLoader _foodLoader;

        private Node2D _foodContainer;

        private List<Vector2> _spawns = new List<Vector2>();
        private List<FoodSpawn> _foodSpawns = new List<FoodSpawn>();

        public FoodSpawner(Plates plates, FoodLoader foodLoader, Node2D foodContainer)
        {
            _plates = plates;
            _foodLoader = foodLoader;
            _foodContainer = foodContainer;
        }

        public void SpawnPlateItem()
        {
            var spawns = GetAvailableSpawns();
            if(!spawns.Any()) return;

            var spawn = spawns[Global.RNG.Next(0, spawns.Count)];

            var foodItem = _foodLoader.Random();
            var plate = _plates.PlatesList[(int) spawn.x][(int) spawn.y];
            var sprite = FoodLoader.CreateSprite(foodItem.Texture);
            sprite.Position = plate.GlobalPosition;
            
            var foodSpawn = new FoodSpawn{Name = foodItem.Name, Position = spawn, Sprite = sprite};
            
            _spawns.Add(spawn);
            _foodSpawns.Add(foodSpawn);
            _plates.PlateSpawns.Add(spawn);
            _foodContainer.AddChild(sprite);
        }

        public List<Vector2> GetAvailableSpawns()
        {
            var spawns = new List<Vector2>();
            for(var r = 0; r < Plates.Rows; r++)
            {
                for(var c = 0; c < Plates.Cols; c++)
                {
                    var spawn = new Vector2(r, c);
                    if(_spawns.Contains(spawn)) continue;
                    
                    spawns.Add(spawn);
                }
            }

            return spawns;
        }

        public List<FoodSpawn> GetSelectedPlates()
        {
            var list = new List<FoodSpawn>();

            foreach(var foodSpawn in _foodSpawns.ToList())
            {
                if(!_plates.SelectedPlates.Contains(foodSpawn.Position)) continue;
                list.Add(foodSpawn);
            }

            return list;
        }

        public void DespawnSelectedPlates()
        {
            var foodSpawns = GetSelectedPlates();
            foreach(var foodSpawn in foodSpawns)
            {
                _foodContainer.RemoveChild(foodSpawn.Sprite);
                _spawns.Remove(foodSpawn.Position);
                _foodSpawns.Remove(foodSpawn);
            }
            
            _plates.DeselectAllPlates();
        }

        public void DespawnAllPlates()
        {
            foreach(var foodSpawn in _foodSpawns.ToList())
            {
                _plates.PlateSpawns.Remove(foodSpawn.Position);
                _spawns.Remove(foodSpawn.Position);
                _foodContainer.RemoveChild(foodSpawn.Sprite);
                _foodSpawns.Remove(foodSpawn);
            }
        }
    }
}