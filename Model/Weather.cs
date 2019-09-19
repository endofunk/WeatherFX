//
// Forecast7Day.cs
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
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using Endofunk.FX.Net;

namespace WeatherFX.Model {
  using JP = JsonPropertyAttribute;
  using JC = JsonConverterAttribute;

  public static partial class Here {
    public static class Weather {
      public class Extended7Day {
        [JP("forecasts")] public readonly Forecasts Forecasts;
        [JP("feedCreation")] public readonly DateTimeOffset FeedCreation;
        [JP("metric")] public readonly bool Metric;
        public Extended7Day(Forecasts forecasts, DateTimeOffset feedCreation, bool metric) {
          Forecasts = forecasts;
          FeedCreation = feedCreation;
          Metric = metric;
        }
        public override string ToString() => $"Extended7Day: [Forecasts: {Forecasts}, FeedCreation: {FeedCreation}, Metric: {Metric}]";

        public static Result<Extended7Day> Get(string city) => Config.Here.URL(city, Config.Here.API.Forecast7Days).DownloadAndDeserialize(Create, filePath: Config.Here.Weather.Forecast7DaysFilePath);
        private static Extended7Day FromJson(string json) => JsonConvert.DeserializeObject<Extended7Day>(json, ForecastConverter.Settings);
        private static Func<string, Result<Extended7Day>> Create => json => FromJson(json).ToResult();
      }

      public class Hourly {
        [JP("hourlyForecasts")] public readonly Forecasts Forecasts;
        [JP("feedCreation")] public readonly DateTimeOffset FeedCreation;
        [JP("metric")] public readonly bool Metric;
        public Hourly(Forecasts forecasts, DateTimeOffset feedCreation, bool metric) {
          Forecasts = forecasts;
          FeedCreation = feedCreation;
          Metric = metric;
        }
        public override string ToString() => $"Hourly: [Forecasts: {Forecasts}, FeedCreation: {FeedCreation}, Metric: {Metric}]";

        public static Result<Hourly> Get(string city) => Config.Here.URL(city, Config.Here.API.ForecastHourly).DownloadAndDeserialize(Create, filePath: Config.Here.Weather.ForecastHourlyFilePath);
        private static Hourly FromJson(string json) => JsonConvert.DeserializeObject<Hourly>(json, ForecastConverter.Settings);
        private static Func<string, Result<Hourly>> Create => json => FromJson(json).ToResult();
      }

      public class Forecasts {
        [JP("forecastLocation")] public readonly Location Location;
        public Forecasts(Location location) => Location = location;
        public override string ToString() => $"Forecasts: [Location: {Location}]";
      }

      public class Location {
        [JP("forecast")] public readonly Forecast[] Forecast;
        [JP("country")] public readonly string Country;
        [JP("state")] public readonly string State;
        [JP("city")] public readonly string City;
        [JP("latitude")] public readonly double Latitude;
        [JP("longitude")] public readonly double Longitude;
        [JP("distance")] public readonly long Distance;
        [JP("timezone")] public readonly long Timezone;
        public Location(Forecast[] forecast, string country, string state, string city, double latitude, double longitude, long distance, long timezone) {
          Forecast = forecast;
          Country = country;
          State = state;
          City = city;
          Latitude = latitude;
          Longitude = longitude;
          Distance = distance;
          Timezone = timezone;
        }
        public override string ToString()
          => $"Location: [Forecast: {Forecast.Join(", ")}, Country: {Country}, State: {State}, " +
            $"City: {City}, Latitude: {Latitude}, Longitude: {Longitude}, Distance: {Distance}, Timezone: {Timezone}]";
      }

      public class Forecast {
        [JP("daylight")] public readonly string Daylight;
        [JP("daySegment")] public readonly string DaySegment;
        [JP("description")] public readonly string Description;
        [JP("skyInfo")] [JC(typeof(IntConverter))] public readonly int SkyInfo;
        [JP("skyDescription")] public readonly string SkyDescription;
        [JP("temperature")] [JC(typeof(DoubleConverter))] public readonly double Temperature;
        [JP("temperatureDesc")] public readonly string TemperatureDesc;
        [JP("comfort")] [JC(typeof(DoubleConverter))] public readonly double Comfort;
        [JP("humidity")] [JC(typeof(DoubleConverter))] public readonly double Humidity;
        [JP("dewPoint")] [JC(typeof(DoubleConverter))] public readonly double DewPoint;
        [JP("precipitationProbability")] [JC(typeof(IntConverter))] public readonly int PrecipitationProbability;
        [JP("precipitationDesc")] public readonly string PrecipitationDesc;
        [JP("rainFall")] [JC(typeof(DoubleConverter))] public readonly double RainFall;
        [JP("snowFall")] [JC(typeof(DoubleConverter))] public readonly double SnowFall;
        [JP("airInfo")] [JC(typeof(IntConverter))] public readonly int AirInfo;
        [JP("airDescription")] public readonly string AirDescription;
        [JP("windSpeed")] [JC(typeof(DoubleConverter))] public readonly double WindSpeed;
        [JP("windDirection")] [JC(typeof(IntConverter))] public readonly int WindDirection;
        [JP("windDesc")] public readonly string WindDesc;
        [JP("windDescShort")] public readonly string WindDescShort;
        [JP("beaufortScale")] [JC(typeof(IntConverter))] public readonly int BeaufortScale;
        [JP("beaufortDescription")] public readonly string BeaufortDescription;
        [JP("visibility")] [JC(typeof(DoubleConverter))] public readonly double Visibility;
        [JP("iconName")] public readonly string IconName;
        [JP("iconLink")] public readonly Uri IconLink;
        [JP("dayOfWeek")] public readonly int DayOfWeek;
        [JP("weekday")] public readonly string Weekday;
        [JP("utcTime")] public readonly DateTimeOffset UtcTime;
        public Forecast(string daylight, string description, int skyInfo, string skyDescription, double temperature, string temperatureDesc, double comfort, double humidity, double dewPoint, int precipitationProbability, string precipitationDesc, double rainFall, double snowFall, int airInfo, string airDescription, double windSpeed, int windDirection, string windDesc, string windDescShort, double visibility, string iconName, Uri iconLink, int dayOfWeek, string weekday, DateTimeOffset utcTime, string daySegment = default, int beaufortScale = default, string beaufortDescription = default) {
          Daylight = daylight;
          DaySegment = daySegment;
          Description = description;
          SkyInfo = skyInfo;
          SkyDescription = skyDescription;
          Temperature = temperature;
          TemperatureDesc = temperatureDesc;
          Comfort = comfort;
          Humidity = humidity;
          DewPoint = dewPoint;
          PrecipitationProbability = precipitationProbability;
          PrecipitationDesc = precipitationDesc;
          RainFall = rainFall;
          SnowFall = snowFall;
          AirInfo = airInfo;
          AirDescription = airDescription;
          WindSpeed = windSpeed;
          WindDirection = windDirection;
          WindDesc = windDesc;
          WindDescShort = windDescShort;
          BeaufortScale = beaufortScale;
          BeaufortDescription = beaufortDescription;
          Visibility = visibility;
          IconName = iconName;
          IconLink = iconLink;
          DayOfWeek = dayOfWeek;
          Weekday = weekday;
          UtcTime = utcTime;
        }
        public override string ToString()
          => $"Forecast: [Daylight: {Daylight}, DaySegment: {DaySegment}, Description: {Description}, " +
            $"SkyInfo: {SkyInfo}, SkyDescription: {SkyDescription}, Temperature: {Temperature}, " +
            $"TemperatureDesc: {TemperatureDesc}, Comfort: {Comfort}, Humidity: {Humidity}, " +
            $"DewPoint: {DewPoint}, PrecipitationProbability: {PrecipitationProbability}, " +
            $"PrecipitationDesc: {PrecipitationDesc}, RainFall: {RainFall}, SnowFall: {SnowFall}, " +
            $"AirInfo: {AirInfo}, AirDescription: {AirDescription}, WindSpeed: {WindSpeed}, " +
            $"WindDirection: {WindDirection}, WindDesc: {WindDesc}, WindDescShort: {WindDescShort}, " +
            $"BeaufortScale: {BeaufortScale}, BeaufortDescription: {BeaufortDescription}, " +
            $"Visibility: {Visibility}, IconName: {IconName}, IconLink: {IconLink}, " +
            $"DayOfWeek: {DayOfWeek}, Weekday: {Weekday}, UtcTime: {UtcTime}]";
      }

      private static class ForecastConverter {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
          MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
          DateParseHandling = DateParseHandling.None,
          Converters = {
            DoubleConverter.Singleton,
            IntConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
          }
        };
      }

      private class DoubleConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(double);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
          if (reader.TokenType == JsonToken.Null) return -1.0d;
          var stringValue = serializer.Deserialize<string>(reader);
          if (stringValue == "*") return -1.0d;
          if (double.TryParse(stringValue, out double v)) return v;
          return -1.0d;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
          var v = (double)value;
          if (v.Equals(-1.0d)) { serializer.Serialize(writer, "*"); return; }
          serializer.Serialize(writer, v.ToString());
        }
        public static readonly DoubleConverter Singleton = new DoubleConverter();
      }

      private class IntConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(int);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
          if (reader.TokenType == JsonToken.Null) return -1;
          var stringValue = serializer.Deserialize<string>(reader);
          if (stringValue == "*") return -1;
          if (int.TryParse(stringValue, out int v)) return v;
          return -1;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
          var v = (int)value;
          if (v.Equals(-1)) { serializer.Serialize(writer, "*"); return; }
          serializer.Serialize(writer, v.ToString());
        }
        public static readonly IntConverter Singleton = new IntConverter();
      }
    }
  }
}