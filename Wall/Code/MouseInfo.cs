using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wall {
    public struct MouseInfo {
        
        public bool leftDown, middleDown, rightDown;
        public bool leftPressed, middlePressed, rightPressed;
        public Vector2 pos;
        public int scroll;
        
        public MouseInfo(MouseState state, bool leftChange, bool middleChange, bool rightChange, int scroll) {
            leftDown = state.LeftButton == ButtonState.Pressed;
            middleDown = state.MiddleButton == ButtonState.Pressed;
            rightDown = state.RightButton == ButtonState.Pressed;

            leftPressed = leftDown && leftChange;
            middlePressed = middleDown && middleChange;
            rightPressed = rightDown && rightChange;

            this.scroll = scroll;

            pos = new Vector2(state.X, state.Y);
        }
    }
}