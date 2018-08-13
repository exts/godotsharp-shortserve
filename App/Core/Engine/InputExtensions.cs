using Godot;

namespace ShortServe.App.Core.Engine
{
    public static class InputExtensions
    {
        public static int Int(this ButtonList buttonList)
        {
            return (int) buttonList;
        }

        public static int Int(this KeyList keyList)
        {
            return (int) keyList;
        }
    }
    
}