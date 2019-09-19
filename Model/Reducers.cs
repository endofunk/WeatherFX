//
// Reducers.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using WeatherFX.View;
using Endofunk.Monogame.UI;

namespace WeatherFX.Model {
  public static class ReducerExtensions {
    public static State Initialize(this State state, Game game, GraphicsDeviceManager graphics) {
      state.Game = game;
      state.Graphics = graphics;
      state.Status = Status.Initializing;
      state.ScreenSize = new Vector2(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
      Current.Instance.Store.Dispatch(ActionState.LoadAssets.With());
      state.LoadingLayout();
      return state;
    }

    public static State Reset(this State state) {
      state.LoadingLayout();
      state.Refresh();
      return state;
    }

    public static State Refresh(this State state) {
      switch (state.Status) {
        case Status.Forecast:
          Current.Instance.Store.DispatchAsync(ActionState.LoadForecast.With(state.City));
          break;
        case Status.Astronomy:
          Current.Instance.Store.DispatchAsync(ActionState.LoadAstronomy.With(state.City));
          break;
      }
      return state;
    }

    public static State Menu(this State state) {
      state.Status = Status.Menu;
      state.LastKeyPressed = Keys.M;
      state.LoadMenu();
      return state;
    }

    public static State LoadAssets(this State state) {
      state.Icons = Icons.Get();
      state.IconNames = IconNames.Get();
      state.Textures = Assets.Get().Bind(Assets.LoadResource<Texture2D>(state.Game, r => r.Textures));
      state.Fonts = Assets.Get().Bind(Assets.LoadResource<SpriteFont>(state.Game, r => r.Fonts));
      state.Sounds = Assets.Get().Bind(Assets.LoadResource<SoundEffect>(state.Game, r => r.Sounds));
      return state;
    }

    public static State LoadForecast(this State state, string city) {
      Console.WriteLine("Prepare Forecast");
      state.City = city;
      state.Extended7Day = Here.Weather.Extended7Day.Get(city);
      state.Background = Unsplash.Get(city)
       .Bind(Unsplash.FirstPhoto())
       .Bind(Unsplash.DownloadFile(Config.Background.filepath(state)))
       .Bind(Unsplash.OpenFileStreamAsTexture2D(state.Graphics));
      state.LayoutForecast();
      state.Status = Status.Forecast;
      return state;
    }

    public static State LoadAstronomy(this State state, string city) {
      state.Astronomy = Here.Astronomy.Get(city);
      state.Status = Status.Astronomy;
      state.CurrentView = Error<IElement>("Initial Error State");
      return state;
    }

    public static State EnqueueScreen(this State state, Screen screen) {
      state.ScreenManager.EnQueue(screen);
      return state;
    }
  }
}
