//
// Forecast.cs
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
  using MT1 = ValueTuple<string, string, double, double, DateTimeOffset>;
  using MT2 = ValueTuple<string, string, long, double, double>;
  using TD1 = ValueTuple<double, double, string, string, Here.Weather.Forecast, DateTimeOffset>;

  public static class Forecast {
    #region Forecast Layout Combine 
    internal static readonly Func<Texture2D, IElement, IElement, IElement, Vector2, IElement> Combine = (b, h, t, m, s) => {
      var graph = Spacer(new Vector2(s.X, s.Y - h.Bounding.Y - t.Bounding.Y - m.Bounding.Y));
      return Panel(b, Color.DarkSalmon * 0.6f, Align.Left | Align.Top, Vector2.Zero, VStack(h, t, graph, m));
    };
    #endregion

    #region MidTerm Layout
    public static class MidTerm {
      private static readonly Func<Dictionary<string, Texture2D>, Dictionary<string, SpriteFont>, Vector2, int, Func<MT2, IElement>> Column = (t, f, s, c) => mt2 => {
        var (texture, font, scale) = (t[mt2.Item2], c < 6 ? f["12"] : f["10"], c < 6 ? 0.28f : 0.25f);
        var opacity = 0.6f;
        return Panel(t["MidTerm"], new Color(60, 33, 37) * 0.7f, Align.Top | Align.Left, Vector2.Zero,
          VStack(
            Spacer(new Vector2(s.X / c, 5)),
            SpriteSheet(texture, Color.White * opacity, Align.Center, new Vector2(s.X / c, texture.Height * scale), 0f, scale, Vector2.Zero, 24, (int)mt2.Item3),
            Label(mt2.Item1.Substring(0, 3).ToUpper(), Color.White * opacity, Align.Center, font, new Vector2(s.X / c, 20)),
            Label($"{(int)mt2.Item4}° / {(int)mt2.Item5}°", Color.White * opacity, Align.Center, font, new Vector2(s.X / c, 20)),
            Spacer(new Vector2(s.X / c, 5))
          )
        );
      };

      internal static readonly Func<IEnumerable<MT1>, IconNames.Root, Dictionary<string, Texture2D>, Dictionary<string, SpriteFont>, Vector2, IElement> Stack = (fc, i, t, f, s) => {
        var opacity = 0.6f;
        return VStack(
          Panel(t["TitleBG"], new Color(15, 8, 9) * 0.95f, Align.Left | Align.Top, Vector2.Zero,
            HStack(
              Spacer(new Vector2(5f, 30f)),
              Label("Mid Term Forecast", Color.White * opacity, Align.Left | Align.Center, f["10"], new Vector2(s.X, 30f))
            )
          ),
          HStack(fc
            .Join(i.Names, ok => ok.Item2, ik => ik.Id, (a, b) => (a.Item1, b.Image, b.Index, a.Item3, a.Item4))
            .Map(Column(t, f, s, fc.Count()))
            .ToArray()
          )
        );
      };

      internal static MT1 Summarize(IGrouping<string, Here.Weather.Forecast> forecasts) {
        return (forecasts.First().Weekday, forecasts.First().IconName, forecasts.Min(v => v.Temperature), forecasts.Max(v => v.Temperature), forecasts.First().UtcTime);
      }
    }
    #endregion

    #region TodayDetails Layout
    public static class TodayDetails {
      internal static readonly Func<TD1, Dictionary<string, Texture2D>, Dictionary<string, SpriteFont>, Vector2, IElement> DetailsStack = (td1, t, f, s) => {
        var opacity = 0.8f;
        var rotation = MathHelper.ToRadians(td1.Item5.WindDirection);
        var Wind = t["Wind"];

        Stack RightDetails = HStack(
          Spacer(new Vector2(10, 20)),
          VStack(
            Label($"{"Visibility:",14} {td1.Item5.Visibility}%", Color.White * opacity, Align.Left, f["10"], new Vector2(s.X / 2, 20)),
            Label($"{"Humidity:",14} {td1.Item5.Humidity}%", Color.White * opacity, Align.Left, f["10"], new Vector2(s.X / 2, 20)),
            Label($"{"Precipitation:",14} {td1.Item5.PrecipitationProbability}%", Color.White * opacity, Align.Left, f["10"], new Vector2(s.X / 2, 20)),
            Label($"{"Latitude:",14} {td1.Item1}", Color.White * opacity, Align.Left, f["10"], new Vector2(s.X / 2, 20)),
            Label($"{"Longitude:",14} {td1.Item2}", Color.White * opacity, Align.Left, f["10"], new Vector2(s.X / 2, 20)),
            Label($"{"Wind:",14} {td1.Item5.WindDesc}", Color.White * opacity, Align.Left, f["10"], new Vector2(s.X / 2, 20)),
            Label($"{"Beaufort:",14} {td1.Item5.BeaufortDescription}", Color.White * opacity, Align.Left, f["10"], new Vector2(s.X / 2, 20)),
            Spacer(new Vector2(s.X / 2, 10))
          ),
          Spacer(new Vector2(10, 20))
        );

        Stack Rightside = VStack(
          Spacer(new Vector2(s.X / 2, 0)),
          Label($"{td1.Item5.Temperature}°", Color.White * opacity, Align.Center, f["50"], new Vector2(s.X / 2, s.X / 4)),
          Panel(t["TitleBG"], new Color(60, 33, 37) * 0.7f, Align.Left | Align.Top, Vector2.Zero,
            VStack(
              Panel(t["TitleBG"], new Color(60, 33, 37) * 0.7f, Align.Left | Align.Top, Vector2.Zero,
                VStack(
                  Spacer(new Vector2(s.X / 2, 10)),
                  Label($"{td1.Item5.Description}", Color.White * opacity, Align.Center, f["10"], new Vector2(s.X / 2, 20)),
                  Spacer(new Vector2(s.X / 2, 10))
                )
              ),
              RightDetails
            )
          )
        );

        Stack Leftside = VStack(
          Spacer(new Vector2(s.X / 2, 50)),
          Panel(t["Compas"], Color.White * opacity, Align.Left | Align.Top, Vector2.Zero,
            SpriteSheet(Wind, new Color(60, 33, 37) * 1f, Align.Center, new Vector2(s.X / 2, s.X / 2f), rotation, 0.25f, new Vector2(Wind.Width / 14 / 2, Wind.Width / 14 / 2), 14)[13]
          )
        );
        return HStack(Leftside, Rightside);
      };
    }
    #endregion
  }

  #region Forecast Extension Methods
  public static class ForecastExtension {
    public static Func<Dictionary<string, Texture2D>, Dictionary<string, SpriteFont>, Here.Weather.Extended7Day, Panel> HeaderLayout(this State state) => (t, f, w) => {
      var opacity = 0.7f;
      var menu = new FlatButton("", f["10"], t["Menu"], Color.White * opacity, Align.Center, new Vector2(32, 32), 0f, 0.6f, Vector2.Zero, 2);
      menu.Clicked += Menu_Clicked; 
      var city = Label($"{w.Forecasts.Location.City}, {w.Forecasts.Location.Country}", Color.White * opacity, Align.Left, f["10"], new Vector2(120, 32));
      var time = Label(DateTime.Now.ToShortTimeString(), Color.White * opacity, Align.Right, f["10"], new Vector2(100, 32));
      time.UpdateEvent += Time_UpdateEvent;
      return Panel(t["TitleBG"], new Color(15, 8, 9) * 1f, Align.Left | Align.Top, Vector2.Zero, HStack(menu, Spacer(new Vector2(10, 32)), city, Spacer(new Vector2(state.ScreenSize.X - menu.Bounding.X - 9 - city.Bounding.X - time.Bounding.X - 5, 32)), time, Spacer(new Vector2(5, 32))));
    };

    private static void Menu_Clicked(object sender, EventArgs e) {
      Current.Instance.Store.DispatchAsync(ActionState.Menu.With());
    }

    private static void Time_UpdateEvent(object sender, GameTime gameTime) {
      if (sender is Label l) l.Text = DateTime.Now.ToShortTimeString();
    }

    public static Result<IElement> TodayDetailsLayout(this State state) {
      var location = state.Extended7Day
      .Map(f => {
        var l = f.Forecasts.Location;
        return (l.Latitude, l.Longitude, l.City, l.Country, l.Forecast.First(), f.FeedCreation);
      });
      return Forecast.TodayDetails.DetailsStack.LiftM(location, state.Textures, state.Fonts, Value(state.ScreenSize));
    }

    public static Result<IElement> MidTermLayout(this State state) {
      var now = DateTime.Now;
      var days = state.Extended7Day
        .Map(f => f.Forecasts.Location.Forecast
        .GroupBy(g => g.Weekday)
        .Map(Forecast.MidTerm.Summarize)
        .Filter(mt1 => mt1.Item5 > new DateTime(now.Year, now.Month, now.Day))
        .Take(6));
      return Forecast.MidTerm.Stack.LiftM(days, state.IconNames, state.Textures, state.Fonts, Value(state.ScreenSize));
    }

    public static void LayoutForecast(this State state) {
      var head = state.HeaderLayout().LiftM(state.Textures, state.Fonts, state.Extended7Day);
      Forecast.Combine
        .LiftM(state.Background, head, state.TodayDetailsLayout(), state.MidTermLayout(), Value(state.ScreenSize))
        .Match(
          e => { },
          v => Current.Instance.Store.Dispatch(ActionState.QueueScreen.With(Screen(v, Transition.EaseOut, Direction.Left)))
        );
    }
  }
  #endregion

}

