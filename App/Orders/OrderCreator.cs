using System.Collections.Generic;
using Godot;
using ShortServe.App.Core;
using ShortServe.App.Core.Engine;
using ShortServe.App.Food;

namespace ShortServe.App.Orders
{
    public struct OrderItems
    {
        public List<FoodItem> Items;
    }
    
    public class OrderCreator
    {
        private FoodLoader _foodLoader;
        private PackedScene _orderObject;
        
        public OrderCreator(FoodLoader foodLoader)
        {
            _foodLoader = foodLoader;
            _orderObject = (PackedScene) GD.Load($"{Global.ResDataObjects}/Order.tscn");
        }

        public Order CreateOrder(List<FoodSpawn> foodSpawns)
        {
            var rng = Global.RNG.Next(1, 4);
            var items = foodSpawns.Count > 3
                ? _foodLoader.RandomUniqueRangeItems(rng, foodSpawns)
                : _foodLoader.RandomUniqueRange(rng);
            
            var order = _orderObject.Instance<Order>();
            order.AddFoodItems(items);

            return order;
        }

        public Order CreateEmptyOrder()
        {
            var order = _orderObject.Instance<Order>();
            order.EmptyOrder = true;

            return order;
        }
    }
}