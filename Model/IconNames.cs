//
// IconNames.cs
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
using System.IO;
using System.Globalization;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JP = Newtonsoft.Json.JsonPropertyAttribute;

namespace WeatherFX.Model {
  public static class IconNames {
    public static Result<Root> Get() => Try(() => {
      var json = File.ReadAllText(Config.IconNames.filepath);
      return JsonConvert.DeserializeObject<Root>(json);
    });
    public static Root FromJson(string json) => JsonConvert.DeserializeObject<Root>(json, Converter.Settings);
    private static Func<string, Result<Root>> Create => json => FromJson(json).ToResult();

    public class Root {
      [JP("IconNames")] public readonly Icon[] Names;
      public Root(Icon[] names) => Names = names;
      public override string ToString() => $"Root: [IconNames: [{Names.Map(x => x.ToString()).Join(", ")}]]";
    }

    public class Icon {
      [JP("Id")] public readonly string Id;
      [JP("Image")] public readonly string Image;
      [JP("Index")] public readonly long Index;
      public Icon(string id, string image, long index) => (Id, Image, Index) = (id, image, index);
      public object IconName { get; internal set; }
      public override string ToString() => $"Icon: [Id: {Id}, Image: {Image}, Index: {Index}]";
    }

    internal static class Converter {
      public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = {
          new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
      };
    }
  }
}
