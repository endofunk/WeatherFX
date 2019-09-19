//
// Game1.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Endofunk.FX;
using static Endofunk.FX.Prelude;
using WeatherFX.Model;
using Endofunk.Monogame.UI;
using static Endofunk.Monogame.UI.Prelude;
using System.Linq;

namespace WeatherFX {
  public class Controller : Game {
    internal readonly GraphicsDeviceManager Graphics;
    private SpriteBatch SpriteBatch;
    TimeSpan PreviousUpdate;
    Canvas Canvas1;
    Canvas Canvas2;
    SpriteSheet sp;
    Image image;
    Panel panel;

    public Controller() {
      Graphics = new GraphicsDeviceManager(this) {
        PreferredBackBufferWidth = 480,
        PreferredBackBufferHeight = 640
      };
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }

    protected override void Initialize() {
      Current.Instance.Store.Dispatch(ActionState.Initialize.With((this, Graphics)));
      base.Initialize();
    }

    protected override void LoadContent() {
      SpriteBatch = new SpriteBatch(GraphicsDevice);
      Canvas2 = new Canvas(this, new Point(480, 640));
      Canvas2.Graphics.Clear(System.Drawing.Color.Transparent);

      var bgtexture = Content.Load<Texture2D>("Images/Loading");

      var pen = new System.Drawing.Pen(System.Drawing.Brushes.OrangeRed, 2)
        .With(p => { p.DashPattern = new float[] { 2.0f, 2.0f }; });
      Canvas2.Graphics.DrawLine(pen, new System.Drawing.Point(120 - 80, 120), new System.Drawing.Point(120 + 80, 120));
      Canvas2.Graphics.DrawLine(pen, new System.Drawing.Point(120, 120 - 80), new System.Drawing.Point(120, 120 + 80));
      Canvas2.Save();

      var texture0 = Content.Load<Texture2D>("Images/Compas");
      image = Image(texture0, Color.White * 0.2f, Align.Center, new Vector2(480 / 2, 480 / 2), 0f, new Vector2(1f, 1f), Vector2.Zero);


      var texture = Content.Load<Texture2D>("Images/Wind");
      sp = SpriteSheet(texture, Color.White * 0.6f, Align.Center, new Vector2(480 / 2, 480 / 2), 0.0f, 0.4f, new Vector2(texture.Width / 14 / 2, texture.Height / 2), 14)[13];
      panel = Panel(bgtexture, Color.White * 0.5f, Align.Left | Align.Top, Vector2.Zero, Panel(texture0, Color.White * 0.3f, Align.Left | Align.Top, Vector2.Zero, Spacer(new Vector2(480 / 2, 480 / 2))));
      sp.UpdateEvent += Sp_UpdateEvent;

      Canvas1 = new Canvas(this, new Point(480, 270));
      //Canvas1.Graphics.Clear(System.Drawing.Color.FromArgb(255, 30, 16, 18));
      Canvas1.Graphics.Clear(System.Drawing.Color.Transparent);
      Canvas1.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

      // XY Grid
      Canvas1.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.White, 3), 20, 20, 20, 50 * 5 + 1);
      Canvas1.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.White, 3), 20 - 1, 50 * 5, 460, 50 * 5);

      for (var i = 5; i < 50; i += 5) {
        Canvas1.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.White, 1), 15, i * 5, 20, i * 5);
      }

      for (var i = 50; i <= 450; i += 30) {
        Canvas1.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.White, 1), i, 50 * 5, i, 50 * 5 + 5);
      }

      var xs = List(50, 80, 110, 140, 170, 200, 230, 270, 300);
      var ys = List(8, 16, 32, 21, 19, 16, 25, 33, 15);
      var points = xs.Zip(ys, (x, y) => new System.Drawing.PointF(x, 50 * 5 - y * 5)).ToArray();

      // Curved Line
      System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
      path.AddCurve(points);
      Canvas1.Graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.IndianRed, 2), path);
      path.Reset();

      // Circle Points
      points.ForEach(p => path.AddEllipse(p.X - 6, p.Y - 6, 12, 12));
      Canvas1.Graphics.FillPath(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 30, 16, 18)), path);
      Canvas1.Graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.White, 2), path);
      path.Reset();

      points.ForEach(p => path.AddEllipse(p.X - 8, p.Y - 8, 16, 16));
      Canvas1.Graphics.DrawPath(new System.Drawing.Pen(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 30, 16, 18)), 3), path);
      path.Reset();

      //points.ForEach(p => {
      //  path.AddString($"{p.Y / 10}°", System.Drawing.FontFamily.GenericSansSerif, 1, 15, new System.Drawing.PointF(p.X - 20, p.Y + 20), System.Drawing.StringFormat.GenericDefault);
      //});

      Canvas1.Graphics.DrawPath(new System.Drawing.Pen(System.Drawing.Brushes.Black, 1), path);

      Canvas1.Save();

      PreviousState.Init();
    }

    private void Sp_UpdateEvent(object sender, GameTime gameTime) {
      if (sender is SpriteSheet s) {
        s.Rotation += 0.1f;
      }
    }

    protected override void Update(GameTime gameTime) {
      Current.Instance.Store.Dispatch(ActionState.NextFrame.With());
      if ((gameTime.TotalGameTime - PreviousUpdate).Milliseconds > 60) {
        PreviousUpdate = gameTime.TotalGameTime;


      }

      Current.Instance.Store.Fold(s => s.ScreenManager.Update(gameTime));
      //Canvas1.Update(gameTime);

      //Canvas2.Update(gameTime);
      //sp.Update(gameTime);

      //image.Update(gameTime);
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
      GraphicsDevice.Clear(Color.TransparentBlack);
      SpriteBatch.Begin();
      Current.Instance.Store.Fold(s => s.ScreenManager.Draw(SpriteBatch));


      //panel.Draw(SpriteBatch, Vector2.Zero);

      //sp.Draw(SpriteBatch, new Vector2(0, 0));
      //Canvas1.Draw(SpriteBatch, new Vector2(0, 300));
      //Canvas2.Draw(SpriteBatch, new Vector2(0, 0));
      SpriteBatch.End();
      base.Draw(gameTime);
    }
  }
}