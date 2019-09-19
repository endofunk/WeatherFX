//
// PreviousState.cs
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
using System.IO;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using Newtonsoft.Json;

namespace WeatherFX.Model {
  public class PreviousState {
    public string City { get; }
    public Status Status { get; }
    public PreviousState(string city, Status status) => (City, Status) = (city, status);
    public static Result<Unit> Save(State state) => Try(() => {
      var json = JsonConvert.SerializeObject(new PreviousState(state.City, state.Status));
      File.WriteAllText(Config.PreviousState.filepath, json);
      return Unit();
    });

    public static Result<PreviousState> Load() => Try(() => {
      var json = File.ReadAllText(Config.PreviousState.filepath);
      return JsonConvert.DeserializeObject<PreviousState>(json);
    });

    public static void Init() {
      var city = Current.Instance.Store.Fold(s => s.City);
      Load().Match(
        e => Current.Instance.Store.DispatchAsync(ActionState.Menu.With()),
        s => s.Switch(ps => ps.Status)
              .Case(Status.Astronomy, ps => Current.Instance.Store.DispatchAsync(ActionState.LoadAstronomy.With(ps.City == city ? ps.City : city)))
              .Case(Status.Forecast, ps => Current.Instance.Store.DispatchAsync(ActionState.LoadForecast.With(ps.City == city ? ps.City : city)))
              .Else(ps => Current.Instance.Store.DispatchAsync(ActionState.Menu.With()))
      );
    }
  }
}
