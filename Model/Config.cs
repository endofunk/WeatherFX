//
// Config.cs
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

namespace WeatherFX.Model {
  public static class Config {
    public static class Here {
      public enum API { Forecast7Days, Astronomy, ForecastHourly }
      private static readonly string appid = "f3rkzpvHW9VB6Rdbk02U";
      private static readonly string appcode = "3SHogIZBQP80wy5jeT8cgA";
      private static readonly string protocol = "https://";
      private static readonly string hostname = "weather.cit.api.here.com";
      private static readonly string urlpath = "/weather/1.0/report.json";
      public static Result<string> URL(string city, API api) => $"{protocol}{hostname}{urlpath}?product={api.Product()}&name={city}&app_id={appid}&app_code={appcode}".ToResult();

      public static class Weather {
        public static string Forecast7DaysFilePath = "Forecast_7days.json";
        public static string ForecastHourlyFilePath = "Forecast_hourly.json";
      }

      public static class Astronomy {
        public static string AstronomyFilePath = "Forecast_astronomy.json";
      }
    }

    internal static string Product(this Here.API api) {
      switch (api) {
        case Here.API.Forecast7Days: return "forecast_7days";
        case Here.API.Astronomy: return "forecast_astronomy";
        case Here.API.ForecastHourly: return "forecast_hourly";
        default: throw new ArgumentException($"Unknown API case: {api}");
      }
    }

    public static class Unsplash {
      private static readonly string clientid = "bb68b60aac86193d3fd7b2580aeb5e4a700f919b8ea675f568efc64db0907630";
      private static readonly string protocol = "https://";
      private static readonly string hostname = "api.unsplash.com";
      private static readonly string urlpath = "/search";
      public static Result<string> URL(string query) => $"{protocol}{hostname}{urlpath}?page=1&orientation=portrait&query={query}&&client_id={clientid}".ToResult();
    }

    public static class Icons {
      public static readonly string filepath = "Content/JSON/Icons.json";
    }

    public static class IconNames {
      public static readonly string filepath = "Content/JSON/IconNames.json";
    }

    public static class Assets {
      public static readonly string filepath = "Content/JSON/Assets.json";
    }

    public static class PreviousState {
      public static readonly string filepath = "SavedState.json";
    }

    public static class Background {
      public static readonly Func<State, string> filepath = s => $"{s.CurrentPath}/background.png";
    }
  }
}
