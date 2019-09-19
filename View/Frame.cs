//
// Frame.cs
//
// Author:  endofunk
//
// Copyright (c) 2019 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using WeatherFX.Model;

namespace WeatherFX.View {
  public static class Frame {
    public static State Next(this State state) {
      var keys = Keyboard.GetState().GetPressedKeys();
      if (keys.Contains(Keys.F) && (keys.Contains(Keys.LeftAlt) || keys.Contains(Keys.RightAlt)) && state.LastKeyPressed != Keys.F) {
        state.LastKeyPressed = Keys.F;
        Current.Instance.Store.DispatchAsync(ActionState.LoadForecast.With(state.City));
      } else if (keys.Contains(Keys.A) && state.LastKeyPressed != Keys.A) {
        state.LastKeyPressed = Keys.A;
        Current.Instance.Store.DispatchAsync(ActionState.LoadAstronomy.With(state.City));
      } else if (keys.Contains(Keys.M) && state.LastKeyPressed != Keys.M) {
        state.LastKeyPressed = Keys.M;
        Current.Instance.Store.Dispatch(ActionState.Menu.With());
      } else if (keys.Contains(Keys.R) && state.LastKeyPressed != Keys.R) {
        state.LastKeyPressed = Keys.R;
        Current.Instance.Store.Dispatch(ActionState.Reset.With());
      } else if (keys.Contains(Keys.Enter) && state.LastKeyPressed != Keys.Enter) {
        state.LastKeyPressed = Keys.Enter;
        Current.Instance.Store.Dispatch(ActionState.Refresh.With());
      } else if (keys.Contains(Keys.Escape) && state.LastKeyPressed != Keys.Escape) {
        state.LastKeyPressed = Keys.Escape;
        PreviousState.Save(state);
        Environment.Exit(0);
      }
      return state;
    }
  }
}
