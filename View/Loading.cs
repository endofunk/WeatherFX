//
// Loading.cs
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
using System.Linq;
using Endofunk.Monogame.UI;
using static Endofunk.Monogame.UI.Prelude;
using WeatherFX.Model;

namespace WeatherFX.View {
  public static class Loading {

    #region Loading Layout
    internal static readonly Func<Dictionary<string, Texture2D>, Dictionary<string, SpriteFont>, Vector2, IElement> Initialize = (t, f, s) => {
      var (logo, font) = (t["Endofunk"], f["20"]);
      var scale = s.X / logo.Width;
      var img = Image(logo, Color.White * 0.7f, Align.Left | Align.Top, new Vector2(s.X, logo.Height * scale), 0f, new Vector2(scale), Vector2.Zero);
      var initText = Label("", Color.White * 0.7f, Align.Top, font, new Vector2(s.X, 30));
      initText.UpdateEvent += InitText_UpdateEvent;
      var h = (s.Y - img.Bounding.Y - initText.Bounding.Y) / 2;
      var (spc1, spc2, spc3) = (Spacer(new Vector2(s.X, h)), Spacer(new Vector2(s.X, h - initText.Bounding.Y)), Spacer(new Vector2(s.X, initText.Bounding.Y)));
      return Panel(t["Loading"], Color.White, Align.Center | Align.Top, Vector2.Zero,
        VStack(spc1, img, spc2, initText, spc3)
      );
    };

    private static void InitText_UpdateEvent(object sender, GameTime gameTime) {
      if (sender is Label l) {
        var numdots = l.Text.ToCharArray().Count(c => c == '.');
        numdots = numdots >= 20 ? 0 : numdots + 1;
        var (dots, spc) = (Enumerable.Repeat(".", numdots).Join(""), Enumerable.Repeat(" ", 20 - numdots).Join(""));
        l.Text = $"{dots}{spc}";
      }
    }
    #endregion
  }

  #region Loading Extension Methods
  public static class LoadingExtension {
    public static void LoadingLayout(this State state) {
      Loading.Initialize
        .LiftM(state.Textures, state.Fonts, Value(state.ScreenSize))
        .Match(_ => { }, v => Current.Instance.Store.Dispatch(ActionState.QueueScreen.With(Screen(v))));
    }
  }
  #endregion
}
