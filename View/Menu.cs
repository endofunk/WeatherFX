//
// Menu.cs
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
using System.Collections.Generic;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Endofunk.Monogame.UI;
using static Endofunk.Monogame.UI.Prelude;
using WeatherFX.Model;

namespace WeatherFX.View {
  public static class Menu {
    #region Forecast Layout Combine 
    internal static readonly Func<Dictionary<string, Texture2D>, IElement, IElement, Vector2, IElement> Combine = (b, h, body, s) => {
      var graph = Spacer(new Vector2(s.X, s.Y - h.Bounding.Y - body.Bounding.Y));
      return Panel(b["Loading"], Color.DarkSalmon * 0.6f, Align.Left | Align.Top, Vector2.Zero, VStack(h, body));
    };
    #endregion


    private static Func<Dictionary<string, Texture2D>, Dictionary<string, SpriteFont>, Panel> MenuHeaderLayout(this State state) => (t, f) => {
      var opacity = 0.7f;
      var menu = FlatButton(t["Menu"], Color.White * opacity, Align.Center, new Vector2(32, 32), 0f, 0.6f, Vector2.Zero, 2);
      menu.Clicked += Menu_Clicked;
      var city = Label(state.Extended7Day.Fold(r => $"{"Menu"}"), Color.White * opacity, Align.Left, f["10"], new Vector2(120, 32));
      var time = Label(DateTime.Now.ToShortTimeString(), Color.White * opacity, Align.Right, f["10"], new Vector2(100, 32));
      time.UpdateEvent += Time_UpdateEvent;
      return Panel(t["TitleBG"], new Color(15, 8, 9) * 1f, Align.Left | Align.Top, Vector2.Zero,
              HStack(menu, Spacer(new Vector2(10, 32)), city, Spacer(new Vector2(state.ScreenSize.X - menu.Bounding.X - 9 - city.Bounding.X - time.Bounding.X - 5, 32)), time, Spacer(new Vector2(5, 32))));
    };

    private static void Menu_Clicked(object sender, EventArgs e) {
      Current.Instance.Store.DispatchAsync(ActionState.LoadForecast.With("London"));
    }

    private static void Time_UpdateEvent(object sender, GameTime gameTime) {
      if (sender is Label l) l.Text = DateTime.Now.ToShortTimeString();
    }


    private static Func<Dictionary<string, SpriteFont>, Vector2, IElement> LayoutMenu(this State state) => (f, s) => {
      return Label("Menu", Color.White, Align.Center, f["70"], s);
    };

    public static void LoadMenu(this State state) {
      Console.WriteLine("LoadMenu");
      var body = state.LayoutMenu().LiftM(state.Fonts, Value(state.ScreenSize));
      var header = state.MenuHeaderLayout().LiftM(state.Textures, state.Fonts);

      Combine
        .LiftM(state.Textures, header, body, Value(state.ScreenSize))
        .Match(NOP, s => Current.Instance.Store.DispatchAsync(ActionState.QueueScreen.With(Screen(s))));
    }
  }
}
