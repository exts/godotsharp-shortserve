using System;
using Godot;

namespace ShortServe.App.Core
{
    public partial class Global
    {
        public static readonly Random RNG = new Random(); 
            
        private Viewport _root;
        public static Global Instance => _instance ?? (_instance = new Global());
        private static Global _instance;

        public enum Scenes
        {
            Game,
            StartMenu
        }

        private const Scenes StartScene = Scenes.StartMenu;

        private readonly Godot.Collections.Dictionary<string, string> _scenePaths = new Godot.Collections.Dictionary<string, string>
        {
            {"Splash", "Splash.tscn"},
            {"Game", "Game.tscn"}
        };

        private const string SceneDirectory = "res://Data/Scenes";


        public void SetRootViewport(Viewport root)
        {
            _root = root;
        }

        public static Viewport Root()
        {
            return Instance._root ?? throw new Exception("The root viewport hasn't been set");
        }

        public static App App()
        {
            return Root().GetNode<App>("App");
        }
        
        public static Node CurrentScene()
        {
            return Root().GetChild(Root().GetChildCount() - 1);
        }

        public static Vector2 MousePosition()
        {
            return Root().GetMousePosition();
        }

        public void LoadStartScene()
        {
            ChangeScene(Scenes.Game);
        }

        public void ChangeScene(Scenes scene)
        {
            App().SwitchScene(GetScenePath(scene));
        }

        private string GetScenePath(Scenes scene)
        {
            var path = _scenePaths[scene.ToString("G")];
            return $"{SceneDirectory}/{path}";
        }
        
    }
}
