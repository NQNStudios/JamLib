using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileGameLib
{
    public delegate void Process(Cursor cursor);

    public enum ArrowDirection
    {
        Horizontal,
        Vertical,
        BottomRight,
        BottomLeft,
        TopRight,
        TopLeft,
        Down,
        Left,
        Up,
        Right,
        None
    }

    public class Cursor
    {
        TileLayer layer;

        Texture2D texture;
        Texture2D arrowTexture;
        Texture2D overlayTexture;

        Entity selectedEntity;
        Point location;

        Dictionary<Buttons[], Process> processes = new Dictionary<Buttons[], Process>();

        public string Group = "";

        public void AddProcess(Buttons[] buttons, Process process)
        {
            processes.Add(buttons, process);
        }

        #region Constructor

        public Cursor(TileLayer layer, Texture2D texture, Texture2D arrowTexture, Texture2D overlayTexture, TimeSpan moveTime, Point? location, string group)
        {
            this.layer = layer;

            this.texture = texture;
            this.arrowTexture = arrowTexture;
            this.overlayTexture = overlayTexture;

            this.moveTime = moveTime;

            if (location.HasValue)
                this.location = location.Value;

            Group = group;

            AddProcess(new Buttons[] { Buttons.A }, new Process(onSelect));
            AddProcess(new Buttons[] { Buttons.X }, new Process(onTab));
            AddProcess(new Buttons[] { Buttons.B }, new Process(onExit));
        }

        #region Overloads

        public Cursor(TileLayer layer, Texture2D texture, Texture2D arrowTexture, Texture2D overlayTexture, Point? location, string group)
            : this(layer, texture, arrowTexture, overlayTexture, TimeSpan.FromSeconds(0.1), location, group) { }

        #endregion

        #endregion

        #region Properties

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set { selectedEntity = value; }
        }

        #endregion

        #region Update

        TimeSpan moveTime;
        TimeSpan elapsed = TimeSpan.Zero;

        GamePadState lastPadState;
        KeyboardState lastKeyState;

        public void Update(PlayerIndex index, GameTime gameTime, Camera camera)
        {
            GamePadState padState = GamePad.GetState(index);

            elapsed -= gameTime.ElapsedGameTime;

            if (selectedEntity != null && selectedEntity.Health.IsEmpty)
                selectedEntity = null;

            if (elapsed <= TimeSpan.Zero && (selectedEntity == null || !selectedEntity.Moving))
            {
                Vector2 tempCamPos = ScreenHelper.Camera.Position;

                if (padState.ThumbSticks.Left.X < 0 || Keyboard.GetState().IsKeyDown(Keys.Left)) //Left stick left
                {
                    --location.X;
                    clamp();
                    Rectangle rect = new Rectangle(location.X * layer.TileWidth, location.Y * layer.TileHeight, layer.TileWidth, layer.TileHeight);
                    
                    if (!camera.IsOnScreen(rect) && fits())
                        camera.Position.X -= layer.TileWidth;

                    elapsed = moveTime;
                }
                if (padState.ThumbSticks.Left.X > 0 || Keyboard.GetState().IsKeyDown(Keys.Right)) //Left stick right
                {
                    ++location.X;
                    clamp();
                    Rectangle rect = new Rectangle(location.X * layer.TileWidth, location.Y * layer.TileHeight, layer.TileWidth, layer.TileHeight);

                    if (!camera.IsOnScreen(rect) && fits())
                        camera.Position.X += layer.TileWidth;

                    elapsed = moveTime;
                }
                if (padState.ThumbSticks.Left.Y < 0 || Keyboard.GetState().IsKeyDown(Keys.Down)) //Left stick down
                {
                    ++location.Y;
                    clamp();
                    Rectangle rect = new Rectangle(location.X * layer.TileWidth, location.Y * layer.TileHeight, layer.TileWidth, layer.TileHeight);

                    if (!camera.IsOnScreen(rect) && fits())
                        camera.Position.Y += layer.TileHeight;

                    elapsed = moveTime;
                }
                if (padState.ThumbSticks.Left.Y > 0 || Keyboard.GetState().IsKeyDown(Keys.Up)) //Left stick up
                {
                    --location.Y;
                    clamp();
                    Rectangle rect = new Rectangle(location.X * layer.TileWidth, location.Y * layer.TileHeight, layer.TileWidth, layer.TileHeight);

                    if (!camera.IsOnScreen(rect) && fits())
                        camera.Position.Y -= layer.TileHeight;

                    elapsed = moveTime;
                }
            }

            foreach (Buttons[] buttons in processes.Keys)
            {
                foreach (Buttons button in buttons)
                {
                    if (padState.IsButtonDown(button) && lastPadState.IsButtonUp(button))
                    {
                        if (processes[buttons] != null)
                            processes[buttons](this);

                        break;
                    }

                    #if WINDOWS

                    if (Keyboard.GetState().IsKeyDown(keyForButton(button)) && lastKeyState.IsKeyUp(keyForButton(button)))
                    {
                        if (processes[buttons] != null)
                            processes[buttons](this);

                        break;
                    }

                    #endif
                }
            }
            lastPadState = padState;
            lastKeyState = Keyboard.GetState();
        }

        #endregion

        #region Draw

        public void DrawSquares(SpriteBatch spriteBatch)
        {
            if (selectedEntity != null)
            {
                switch (selectedEntity.Phase)
                {
                    case Phase.Move:
                        foreach (Point p in selectedEntity.MovePoints())
                        {
                            DrawOverlay(p, spriteBatch, Color.Blue);
                        }
                        break;

                    case Phase.Attack:
                        foreach (Point p in selectedEntity.AttackPoints())
                        {
                            DrawOverlay(p, spriteBatch, Color.Red);
                        }
                        break;

                    default:
                        break;
                }

                
            }
        }

        private void DrawOverlay(Point p, SpriteBatch spriteBatch, Color color)
        {
            Vector2 loc = Utility.ToVector2(ScreenHelper.Viewport.TitleSafeArea.Location) + new Vector2(p.X * layer.TileWidth, p.Y * layer.TileHeight);
            Rectangle source;

            if (color == Color.Blue)
                source = new Rectangle(0, 0, 32, 32);

            else if (color == Color.Red)
                source = new Rectangle(32, 0, 32, 32);

            else if (color == Color.Green)
                source = new Rectangle(64, 0, 32, 32);

            else
                source = Rectangle.Empty;

            spriteBatch.Draw(overlayTexture, loc, source, Color.White);
        }

        Point lastStart;
        Point lastEnd;
        List<Point> path;

        public void DrawArrow(SpriteBatch spriteBatch)
        {
            if (selectedEntity != null && selectedEntity.Phase == Phase.Move && location != selectedEntity.Position) //Draw arrow
            {
                if (!selectedEntity.CanMoveTo(location))
                    return;

                Point start = selectedEntity.Position;
                Point end = Location;

                if (start != lastStart || end != lastEnd)
                    path = layer.Pathfind.FindPath(start, end);

                for (int i = 0; i < path.Count; ++i)
                {
                    Point change = new Point(path[i].X - start.X, path[i].Y - start.Y);
                    bool finished = (i >= path.Count - 1);
                    ArrowDirection d = ArrowDirection.None;
                    if (!finished)
                    {
                        Point nextChange = new Point(path[i + 1].X - path[i].X, path[i + 1].Y - path[i].Y);
                        d = direction(change, nextChange, false);
                    }
                    else
                    {
                        d = direction(change, Point.Zero, true);
                    }

                    Rectangle source = arrowSource(d);

                    Vector2 loc1 = new Vector2(path[i].X * layer.TileWidth, path[i].Y * layer.TileHeight);

                    spriteBatch.Draw(arrowTexture, loc1, source, Color.White);

                    start = path[i];
                }

                lastStart = start;
                lastEnd = end;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 loc = new Vector2(location.X * layer.TileWidth, location.Y * layer.TileHeight) + Utility.ToVector2(ScreenHelper.Viewport.TitleSafeArea.Location);

            spriteBatch.Draw(texture, loc, Color.White);
        }

        #endregion

        #region Helpers

        #if WINDOWS

        private Keys keyForButton(Buttons button)
        {
            switch (button)
            {
                case Buttons.A:
                    return Keys.Space;

                case Buttons.X:
                    return Keys.Tab;

                case Buttons.B:
                    return Keys.Escape;

                default:
                    return Keys.Enter;
            }
        }

        #endif

        Rectangle arrowSource(ArrowDirection d)
        {
            switch (d)
            {
                case ArrowDirection.Horizontal:
                    return new Rectangle(0, 0, 32, 32);

                case ArrowDirection.Vertical:
                    return new Rectangle(32, 0, 32, 32);

                case ArrowDirection.BottomRight:
                    return new Rectangle(64, 0, 32, 32);

                case ArrowDirection.BottomLeft:
                    return new Rectangle(96, 0, 32, 32);

                case ArrowDirection.TopRight:
                    return new Rectangle(0, 32, 32, 32);

                case ArrowDirection.TopLeft:
                    return new Rectangle(32, 32, 32, 32);

                case ArrowDirection.Down:
                    return new Rectangle(0, 64, 32, 32);

                case ArrowDirection.Left:
                    return new Rectangle(96, 32, 32, 32);

                case ArrowDirection.Up:
                    return new Rectangle(64, 32, 32, 32);

                case ArrowDirection.Right:
                    return new Rectangle(32, 64, 32, 32);

                default:
                    return Rectangle.Empty;
            }
        }

        private ArrowDirection direction(Point p1, Point p2, bool finished)
        {
            if (!finished)
            {
                Point p3 = new Point(p2.X + p1.X, p2.Y + p1.Y);

                if (p1.Equals(new Point(1, 0)) && p2.Equals(new Point(1, 0)) || p1.Equals(new Point(-1, 0)) && p2.Equals(new Point(-1, 0)))
                {
                    return ArrowDirection.Horizontal;
                }

                if (p1.Equals(new Point(0, 1)) && p2.Equals(new Point(0, 1)) || p1.Equals(new Point(0, -1)) && p2.Equals(new Point(0, -1)))
                {
                    return ArrowDirection.Vertical;
                }

                if (p1.Equals(new Point(0, -1)) && p2.Equals(new Point(1, 0)) || p1.Equals(new Point(-1, 0)) && p2.Equals(new Point(0, 1)))
                {
                    return ArrowDirection.BottomRight;
                }

                if (p1.Equals(new Point(0, -1)) && p2.Equals(new Point(-1, 0)) || p1.Equals(new Point(1, 0)) && p2.Equals(new Point(0, 1)))
                {
                    return ArrowDirection.BottomLeft;
                }

                if (p1.Equals(new Point(-1, 0)) && p2.Equals(new Point(0, -1)) || p1.Equals(new Point(0, 1)) && p2.Equals(new Point(1, 0)))
                {
                    return ArrowDirection.TopRight;
                }

                if (p1.Equals(new Point(1, 0)) && p2.Equals(new Point(0, -1)) || p1.Equals(new Point(0, 1)) && p2.Equals(new Point(-1, 0)))
                {
                    return ArrowDirection.TopLeft;
                }
            }
            else
            {
                if (p1.Equals(new Point(1, 0)))
                {
                    return ArrowDirection.Right;
                }
                if (p1.Equals(new Point(-1, 0)))
                {
                    return ArrowDirection.Left;
                }
                if (p1.Equals(new Point(0, 1)))
                {
                    return ArrowDirection.Up;
                }
                if (p1.Equals(new Point(0, -1)))
                {
                    return ArrowDirection.Down;
                }
            }

            return ArrowDirection.None;
        }

        private void clamp()
        {
            if (location.X < 0)
                location.X = 0;
            if (location.Y < 0)
                location.Y = 0;
            if (location.X >= layer.Width())
                location.X = layer.Width() - 1;
            if (location.Y >= layer.Height())
                location.Y = layer.Height() - 1;
        }

        private bool fits()
        {
            return (location.X >= 0 && location.X < layer.Width() && location.Y >= 0 && location.Y < layer.Height());
        }

        #endregion

        #region Events

        void onTab(Cursor cursor)
        {
            Entity e = layer.Entities.EntityAt(cursor.Location);
            if (e != null && layer.Entities.CurrentGroup == Group && e.Group == Group)
            {
                e.EndPhase();
            }
        }

        void onExit(Cursor cursor)
        {
            cursor.SelectedEntity = null;
        }

        void onSelect(Cursor cursor)
        {
            if (layer.Entities.CurrentGroup != Group)
                return;

            Entity e = layer.Entities.EntityAt(cursor.Location);

            if (cursor.SelectedEntity != null)
            {
                switch (cursor.SelectedEntity.Phase)
                {
                    case Phase.Move:
                        if (!cursor.SelectedEntity.Moving && cursor.SelectedEntity.CanMoveTo(cursor.Location))
                        {
                            cursor.SelectedEntity.MoveTo(cursor.Location);
                            cursor.SelectedEntity.EndPhase();
                        }
                        break;

                    case Phase.Attack:
                        if (cursor.SelectedEntity.CanAttack(cursor.Location))
                        {
                            cursor.SelectedEntity.Attack(layer.Entities.EntityAt(cursor.Location));
                            cursor.SelectedEntity.EndPhase();
                        }
                        break;

                    case Phase.Finished:
                        break;
                }

                cursor.SelectedEntity = null;
            }
            else if (e != null && e.Group == Group && e.Phase != Phase.Finished)
            {
                cursor.SelectedEntity = e;
            }
        }

        #endregion
    }
}
