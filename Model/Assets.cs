//
// Assets.cs
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
using System.IO;
using System.Linq;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Xna.Framework;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace WeatherFX.Model {
  public static class Assets {
    public static Func<Root, Result<Dictionary<string, A>>> LoadResource<A>(Game game, Func<Root, Asset[]> f) => r => Try(() => f(r).ToDictionary(a => a.Id, a => game.Content.Load<A>(a.AssetName)));
    public static Result<Root> Get() => Try(() => JsonConvert.DeserializeObject<Root>(File.ReadAllText(Config.Assets.filepath)));
    public static Root FromJson(string json) => JsonConvert.DeserializeObject<Root>(json, Converter.Settings);
    public static string ToJson(this Root @this) => JsonConvert.SerializeObject(@this, Converter.Settings);

    public class Root {
      [J("Fonts")] public readonly Asset[] Fonts;
      [J("Textures")] public readonly Asset[] Textures;
      [J("Sounds")] public readonly Asset[] Sounds;
      public Root(Asset[] fonts, Asset[] textures, Asset[] sounds) => (Fonts, Textures, Sounds) = (fonts, textures, sounds);
      public override string ToString() => $"Root: [Fonts: [{Fonts.Map(x => x.ToString()).Join(", ")}], Textures: [{Textures.Map(x => x.ToString()).Join(", ")}], Sounds: [{Sounds.Map(x => x.ToString()).Join(", ")}]]";
    }

    public class Asset {
      [J("Id")] public readonly string Id;
      [J("AssetName")] public readonly string AssetName;
      public Asset(string id, string assetName) => (Id, AssetName) = (id, assetName);
      public override string ToString() => $"Asset: [Id: {Id}, AssetName: {AssetName}]";
    }

    internal static class Converter {
      public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = { new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }},
      };
    }
  }
}
