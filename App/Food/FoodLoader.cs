using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using ShortServe.App.Core;

namespace ShortServe.App.Food
{
    public struct FoodItem
    {
        public string Name;
        public string Type;
        public Texture Texture;

        public FoodItem(string name, string type, Texture texture)
        {
            Name = name;
            Type = type;
            Texture = texture;
        }
    }
    
    public class FoodLoader
    {
        private readonly Godot.Collections.Dictionary<string, string> _drinks = new Godot.Collections.Dictionary<string, string>
        {
            {"Beer", "Beer.png"},
            {"Marmalade", "Marmalade.png"},
            {"Moonshine", "Moonshine.png"},
            {"Whiskey", "Whiskey.png"},
            {"Wine", "Wine.png"},
        };
        
        private readonly Godot.Collections.Dictionary<string, string> _sides = new Godot.Collections.Dictionary<string, string>
        {
            {"Bread", "Bread.png"},
            {"Brownie", "Brownie.png"},
            {"Cookie", "Cookie.png"},
            {"Eggs", "Eggs.png"},
            {"Pickle", "Pickle.png"},
            {"PieApple", "PieApple.png"},
            {"PiePumpkin", "PiePumpkin.png"},
            {"Pretzel", "Pretzel.png"},
            {"Roll", "Roll.png"},
            {"Sashimi", "Sashimi.png"},
            {"Sushi", "Sushi.png"},
            {"Tart", "Tart.png"},
            {"Waffles", "Waffles.png"}
        };

        private readonly Godot.Collections.Dictionary<string, string> _meats = new Godot.Collections.Dictionary<string, string>
        {
            {"Bacon", "Bacon.png"},
            {"Chicken", "Chicken.png"},
            {"ChickenLeg", "ChickenLeg.png"},
            {"Fish", "Fish.png"},
            {"FishFillet", "FishFillet.png"},
            {"FishSteak", "FishSteak.png"},
            {"Jerky", "Jerky.png"},
            {"Ribs", "Ribs.png"},
            {"Sausages", "Sausages.png"},
            {"Shrimp", "Shrimp.png"},
            {"Steak", "Steak.png"}
        };
        
        public List<FoodItem> MeatItems = new List<FoodItem>();
        public List<FoodItem> SideItems = new List<FoodItem>();
        public List<FoodItem> DrinkItems = new List<FoodItem>();

        public FoodLoader() {}
        public FoodLoader(bool load)
        {
            if(load)
                CreateSprites();
        }
        
        private static void CreateTextures(string type, Godot.Collections.Dictionary<string, string> foods, ICollection<FoodItem> foodItemTexturess, string assetPath)
        {
            foreach(var food in foods)
            {
                var imageTexture = (Texture) GD.Load($"{assetPath}/{food.Value}");
                imageTexture.Flags = (int) Texture.FlagsEnum.Mipmaps;

                foodItemTexturess.Add(new FoodItem(food.Key, type, imageTexture));
            }
        }

        public static Sprite CreateSprite(Texture texture)
        {
            var sprite = new Sprite();
            sprite.Texture = texture;
            sprite.Scale = new Vector2(2f, 2f);

            return sprite;
        }
        
        public void CreateSprites()
        {
            CreateTextures("Meats", _meats, MeatItems, "res://Assets/Art/Food/Meats");
            CreateTextures("Sides", _sides, SideItems, "res://Assets/Art/Food/Sides");
            CreateTextures("Drinks", _drinks, DrinkItems, "res://Assets/Art/Food/Drinks");
        }

        public List<FoodItem> RandomUniqueRange(int num)
        {
            if(num <= 0)
            {
                throw new ArgumentException("Can't have less than 1 as a value for RandomRange");
            }

            var list = new List<FoodItem>();
            while(true)
            {
                if(list.Count == num) break;

                var random = Random();
                if(list.Contains(random)) continue;
                
                list.Add(random);
            }
            
            return list;
        }

        public List<FoodItem> RandomUniqueRangeItems(int num, List<FoodSpawn> items)
        {
            if(num <= 0)
            {
                throw new ArgumentException("Can't have less than 1 as a value for RandomRange");
            }

            var list = new List<FoodItem>();
            var available = AvailableInList(items);
            while(true)
            {
                if(list.Count == num) break;

                var random = Global.RNG.Next(0, 2) == 1 ? RandomItem(available) : Random();
                if(list.Contains(random)) continue;
                
                list.Add(random);
            }

            return list;
        }

        public FoodItem Random()
        {
            var rn = Global.RNG.Next(0, 3);
            switch(rn)
            {
                case 0:
                    return RandomDrink();
                case 1:
                    return RandomMeat();
                case 2:
                    return RandomSide();
            }
            
            throw new InvalidDataException("food texture not found");
        }

        public FoodItem RandomItem(List<FoodItem> items)
        {
            if(items.Count == 0)
            {
                throw new IndexOutOfRangeException("No items available to randomize");
            }

            return items[Global.RNG.Next(0, items.Count)];
        }

        public FoodItem RandomDrink()
        {
            return DrinkItems[Global.RNG.Next(0, DrinkItems.Count)];
        }

        public FoodItem RandomMeat()
        {
            return MeatItems[Global.RNG.Next(0, MeatItems.Count)];
        }

        public FoodItem RandomSide()
        {
            return SideItems[Global.RNG.Next(0, SideItems.Count)];
        }

        public List<FoodItem> All()
        {
            var list = new List<FoodItem>(DrinkItems.Count + MeatItems.Count + SideItems.Count);
            list.AddRange(MeatItems);
            list.AddRange(SideItems);
            list.AddRange(DrinkItems);

            return list;
        }

        public List<FoodItem> AvailableInList(List<FoodSpawn> items)
        {
            var available = new List<FoodItem>();
            foreach(var food in All())
            {
                if(items.Any(i => food.Name == i.Name))
                {
                    available.Add(food);
                }
            }

            return available;
        }
        
        
    }
}
