//
// State.cs
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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Endofunk.Monogame.UI;
using static Endofunk.Monogame.UI.Prelude;
using WeatherFX.View;
using Newtonsoft.Json;

namespace WeatherFX.Model {
  using WeatherAction = Tagged<ActionState, (Game, GraphicsDeviceManager), string, Screen>;
  using MiddleWare = Func<Func<State>, Action<Tagged<ActionState, (Game, GraphicsDeviceManager), string, Screen>>, Func<Action<Tagged<ActionState, (Game, GraphicsDeviceManager), string, Screen>>, Action<Tagged<ActionState, (Game, GraphicsDeviceManager), string, Screen>>>>;
  public enum Status { Forecast, Astronomy, Initializing, Menu }

  public class State {
    public Result<Icons.Root> Icons;
    public Result<IconNames.Root> IconNames;
    public Result<Dictionary<string, Texture2D>> Textures;
    public Result<Dictionary<string, SpriteFont>> Fonts;
    public Result<Dictionary<string, SoundEffect>> Sounds;
    public Result<Here.Weather.Extended7Day> Extended7Day;
    public Result<Here.Weather.Hourly> Hourly;
    public Result<Here.Astronomy.Root> Astronomy;
    public Result<Texture2D> Background;
    public Status Status;
    public string City = "Beijing";
    public string CurrentPath;
    public Game Game;
    public GraphicsDeviceManager Graphics;
    public Vector2 ScreenSize;
    public Result<IElement> CurrentView;
    public Result<IElement> IncomingView;
    public Keys LastKeyPressed = Keys.Space;
    public ScreenManager ScreenManager;
    private State() { }
    private State(string currentPath) {
      Textures = new Dictionary<string, Texture2D>();
      Fonts = new Dictionary<string, SpriteFont>();
      Sounds = new Dictionary<string, SoundEffect>();
      Status = Status.Initializing;
      Extended7Day = Error<Here.Weather.Extended7Day>("Initial Error State");
      Background = Error<Texture2D>("Initial Error State");
      Astronomy = Error<Here.Astronomy.Root>("Initial Error State");
      CurrentView = Error<IElement>("Initial Error State");
      CurrentPath = currentPath;
      IncomingView = Error<IElement>("Initial Error State");
      ScreenManager = ScreenManager(Screen(Spacer(Vector2.Zero), Transition.None));
    }

    public static Store<State, WeatherAction> Initial(string path) => Store(Reducer, new State(path), TestLog);
    public static Reducer<State, WeatherAction> Reducer = Reducer<State, WeatherAction>((state, tag) => {
      return tag.Switch<WeatherAction, ActionState, State>(p => p.Tag)
         .Case(ActionState.Initialize, t => state.Initialize(t.Value1.Item1, t.Value1.Item2))
         .Case(ActionState.Reset, _ => state.Reset())
         .Case(ActionState.Menu, _ => state.Menu())
         .Case(ActionState.NextFrame, _ => state.Next())
         .Case(ActionState.LoadAssets, t => state.LoadAssets())
         .Case(ActionState.LoadForecast, t => state.LoadForecast(t.Value2))
         .Case(ActionState.LoadAstronomy, t => state.LoadAstronomy(t.Value2))
         .Case(ActionState.QueueScreen, t => state.EnqueueScreen(t.Value3))
         .Else(_ => state);
    });

    public static MiddleWare TestLog = (getState, _) => dispatch => action => {
      //if (!a.Tag.HasFlag(ActionState.NextFrame)) {
      //  Console.WriteLine($"Tagged: {action}");
      //}
      dispatch(action);
      //if (!a.Tag.HasFlag(ActionState.NextFrame)) {
      //  Console.WriteLine($"State: {json}");
      //}
    };
  }
}
