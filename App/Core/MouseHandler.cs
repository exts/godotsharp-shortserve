using Godot;
using ShortServe.App.Core.Engine;

namespace ShortServe.App.Core
{
    public class MouseHandler
    {
        private bool _leftMousePressed;
        private bool _rightMousePressed;

        public bool ButtonPressed(params ButtonList[] buttons)
        {
            var count = 0;
            foreach(var button in buttons)
            {
                switch(button)
                {
                    case ButtonList.Left:
                        count += _leftMousePressed ? 1 : 0;
                        break;
                    case ButtonList.Right:
                        count += _rightMousePressed ? 1 : 0;
                        break;
                }
            }

            return count == buttons.Length;
        }

        public void ResetButton(params ButtonList[] buttons)
        {
            foreach(var button in buttons)
            {
                switch(button)
                {
                    case ButtonList.Left:
                        _leftMousePressed = false;
                        break;
                    case ButtonList.Right:
                        _rightMousePressed = false;
                        break;
                }
            }
        }
        
        public void HandleMouseEvents(InputEventMouseButton button)
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
    }
}