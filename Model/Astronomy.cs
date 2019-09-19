//
// Astronomy.cs
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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Endofunk.FX;
using Endofunk.FX.Net;

namespace WeatherFX.Model {
  using JP = JsonPropertyAttribute;

  public static partial class Here {
    public static class Astronomy {
      public static Result<Root> Get(string city) => Config.Here.URL(city, Config.Here.API.Astronomy).DownloadAndDeserialize(Create, filePath: Config.Here.Astronomy.AstronomyFilePath);

      public class Root {
        [JP("astronomy")] public readonly Location Location;
        [JP("feedCreation")] public readonly DateTimeOffset FeedCreation;
        [JP("metric")] public readonly bool Metric;
        public Root(Location location, DateTimeOffset feedCreation, bool metric) {
          Location = location;
          FeedCreation = feedCreation;
          Metric = metric;
        }
        public override string ToString() => $"Root: [Location: {Location}, FeedCreation: {FeedCreation}, Metric: {Metric}]";
      }

      public class Location {
        [JP("astronomy")] public readonly List<Phase> Phases;
        [JP("country")] public readonly string Country;
        [JP("state")] public readonly string State;
        [JP("city")] public readonly string City;
        [JP("latitude")] public readonly double Latitude;
        [JP("longitude")] public readonly double Longitude;
        [JP("timezone")] public readonly int Timezone;
        public Location(List<Phase> phases, string country, string state, string city, double latitude, double longitude, int timezone) {
          Phases = phases;
          Country = country;
          State = state;
          City = city;
          Latitude = latitude;
          Longitude = longitude;
          Timezone = timezone;
        }
        public override string ToString() {
          return $"Location: [Phases: {Phases.Map(x => x.ToString()).Join(",")}, Country: {Country}, State: {State}, City: {City}, Latitude: {Latitude}, Longitude: {Longitude}, Timezone: {Timezone}]";
        }
      }

      public class Phase {
        [JP("sunrise")] public readonly string Sunrise;
        [JP("sunset")] public readonly string Sunset;
        [JP("moonrise")] public readonly string Moonrise;
        [JP("moonset")] public readonly string Moonset;
        [JP("moonPhase")] public readonly double MoonPhase;
        [JP("moonPhaseDesc")] public readonly string MoonPhaseDesc;
        [JP("iconName")] public readonly string IconName;
        [JP("city")] public readonly string City;
        [JP("latitude")] public readonly double Latitude;
        [JP("longitude")] public readonly double Longitude;
        [JP("utcTime")] public readonly DateTimeOffset UtcTime;
        public Phase(string sunrise, string sunset, string moonrise, string moonset, double moonPhase, string moonPhaseDesc, string iconName, string city, double latitude, double longitude, DateTimeOffset utcTime) {
          Sunrise = sunrise;
          Sunset = sunset;
          Moonrise = moonrise;
          Moonset = moonset;
          MoonPhase = moonPhase;
          MoonPhaseDesc = moonPhaseDesc;
          IconName = iconName;
          City = city;
          Latitude = latitude;
          Longitude = longitude;
          UtcTime = utcTime;
        }
        public override string ToString()
          => $"Phase: [Sunrise: {Sunrise}, Sunset: {Sunset}, Moonrise: {Moonrise}, Moonset: {Moonset}, MoonPhase: {MoonPhase}, " +
            $"MoonPhaseDesc: {MoonPhaseDesc}, IconName: {IconName}, City: {City}, Latitude: {Latitude}, Longitude: {Longitude}, UtcTime: {UtcTime}]";
      }

      private static Root FromJson(string json) => JsonConvert.DeserializeObject<Root>(json, Converter.Settings);
      private static Func<string, Result<Root>> Create => json => FromJson(json).ToResult();

      private static class Converter {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
          MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
          DateParseHandling = DateParseHandling.None,
          Converters = {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
          }
        };
      }
    }
  }
}
