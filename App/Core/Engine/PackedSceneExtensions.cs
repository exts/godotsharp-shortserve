using Godot;

namespace ShortServe.App.Core.Engine
{
    public static class PackedSceneExtensions
    {
        public static T Instance<T>(this PackedScene resource) where T : Node
        {
            return (T) resource.Instance();
        }
    }
}