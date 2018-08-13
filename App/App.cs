using Godot;
using ShortServe.App.Core;

namespace ShortServe.App
{
    public class App : Node
    {
        private Node _scene;

        public override void _Ready()
        {
            // this singleton lets us access the root from everywhere in our application
            Global.Instance.SetRootViewport(GetTree().GetRoot());

            // grab the current scene that's loaded
            _scene = Global.CurrentScene();
        }
        
        public override void _Notification(int what)
        {
            if (what == MainLoop.NotificationWmQuitRequest)
                Quit();
        }

        public void Quit()
        {
            GetTree().Quit(); // default behavior
        }

        public void SwitchScene(string path)
        {
            CallDeferred(nameof(SwitchCurrentScene), path);
        }

        private void SwitchCurrentScene(string path)
        {
            _scene.Free();
            _scene = ((PackedScene) GD.Load(path)).Instance();
            
            Global.Root().AddChild(_scene);
            GetTree().SetCurrentScene(_scene);
        }
    }
}