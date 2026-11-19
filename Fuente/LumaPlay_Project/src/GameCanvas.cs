using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LumaPlay
{
    public sealed class GameCanvas : Control
    {
        private enum GameState { Ready, Playing, Paused, Over }

        private sealed class Obstacle
        {
            public int Lane;
            public float Y;
        }

        private readonly Timer _timer;
        private readonly Random _random = new Random();
        private readonly List<Obstacle> _obstacles = new List<Obstacle>();
        private readonly HashSet<Keys> _pressed = new HashSet<Keys>();
        private readonly float[] _starsX = new float[65];
        private readonly float[] _starsY = new float[65];
        private readonly int _bestAtStart;
        private GameState _state = GameState.Ready;
        private int _lane = 2;
        private int _score;
        private int _ticks;
        private int _spawnCounter;
        private float _speed = 0.008F;
        private bool _reported;

        public event Action<int> RunCompleted;
        public event Action ExitRequested;

        public GameCanvas(int bestScore)
        {
            _bestAtStart = bestScore;
            DoubleBuffered = true;
            BackColor = Color.FromArgb(9, 12, 24);
            TabStop = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            for (int i = 0; i < _starsX.Length; i++)
            {
                _starsX[i] = (float)_random.NextDouble();
                _starsY[i] = (float)_random.NextDouble();
            }
            _timer = new Timer { Interval = 16 };
            _timer.Tick += Timer_Tick;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int w = Math.Max(1, ClientSize.Width);
            int h = Math.Max(1, ClientSize.Height);

            using (LinearGradientBrush bg = new LinearGradientBrush(ClientRectangle, Color.FromArgb(8, 12, 27), Color.FromArgb(20, 17, 43), 90F))
                g.FillRectangle(bg, ClientRectangle);

            for (int i = 0; i < _starsX.Length; i++)
            {
                int sx = (int)(_starsX[i] * w);
                int sy = (int)(_starsY[i] * h);
                using (SolidBrush star = new SolidBrush(Color.FromArgb(70 + (i % 5) * 25, 160, 173, 223)))
                    g.FillEllipse(star, sx, sy, 2, 2);
            }

            PointF[] road = {
                new PointF(w * 0.24F, h * 0.05F),
                new PointF(w * 0.76F, h * 0.05F),
                new PointF(w * 0.96F, h),
                new PointF(w * 0.04F, h)
            };
            using (SolidBrush roadBrush = new SolidBrush(Color.FromArgb(23, 27, 51))) g.FillPolygon(roadBrush, road);
            using (Pen edge = new Pen(Color.FromArgb(110, 111, 93, 255), 2F))
            {
                g.DrawLine(edge, road[0], road[3]);
                g.DrawLine(edge, road[1], road[2]);
            }

            for (int lane = 1; lane <= 4; lane++)
            {
                float xTop = w * (0.24F + lane * 0.104F);
                float xBottom = w * (0.04F + lane * 0.184F);
                using (Pen divider = new Pen(Color.FromArgb(55, 139, 150, 203), 1.2F))
                {
                    divider.DashPattern = new float[] { 9F, 13F };
                    g.DrawLine(divider, xTop, h * 0.05F, xBottom, h);
                }
            }

            foreach (Obstacle obstacle in _obstacles)
            {
                float centerX = LaneX(obstacle.Lane, w);
                float y = obstacle.Y * h;
                float bw = w * 0.075F;
                float bh = h * 0.075F;
                RectangleF rect = new RectangleF(centerX - bw / 2F, y, bw, bh);
                using (SolidBrush shadow = new SolidBrush(Color.FromArgb(65, 255, 66, 120)))
                    g.FillEllipse(shadow, rect.X - 7, rect.Y - 4, rect.Width + 14, rect.Height + 11);
                using (LinearGradientBrush block = new LinearGradientBrush(rect, Color.FromArgb(255, 87, 117), Color.FromArgb(155, 55, 210), 45F))
                    g.FillRectangle(block, rect);
                using (Pen outline = new Pen(Color.FromArgb(255, 178, 207), 2F)) g.DrawRectangle(outline, rect.X, rect.Y, rect.Width, rect.Height);
            }

            DrawPlayer(g, w, h);
            DrawHud(g, w);

            if (_state != GameState.Playing) DrawOverlay(g, w, h);
        }

        private float LaneX(int lane, int width)
        {
            float t = lane / 4F;
            return width * (0.24F + 0.52F * t);
        }

        private void DrawPlayer(Graphics g, int w, int h)
        {
            float x = LaneX(_lane, w);
            float y = h * 0.82F;
            PointF[] ship = {
                new PointF(x, y - h * 0.055F),
                new PointF(x - w * 0.035F, y + h * 0.035F),
                new PointF(x, y + h * 0.020F),
                new PointF(x + w * 0.035F, y + h * 0.035F)
            };
            using (SolidBrush glow = new SolidBrush(Color.FromArgb(65, 84, 229, 255)))
                g.FillEllipse(glow, x - w * 0.055F, y - h * 0.02F, w * 0.11F, h * 0.11F);
            using (LinearGradientBrush shipBrush = new LinearGradientBrush(new RectangleF(x - 30, y - 40, 60, 80), Color.FromArgb(220, 250, 255), Color.FromArgb(71, 179, 255), 90F))
                g.FillPolygon(shipBrush, ship);
            using (Pen outline = new Pen(Color.FromArgb(190, 231, 255), 2F)) g.DrawPolygon(outline, ship);
        }

        private void DrawHud(Graphics g, int w)
        {
            using (Font small = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            using (Font big = new Font("Segoe UI Semibold", 19F, FontStyle.Bold))
            using (SolidBrush muted = new SolidBrush(Color.FromArgb(160, 174, 205)))
            using (SolidBrush white = new SolidBrush(Color.White))
            {
                g.DrawString("PUNTOS", small, muted, 24, 20);
                g.DrawString(_score.ToString("000000"), big, white, 22, 38);
                g.DrawString("RÉCORD  " + Math.Max(_bestAtStart, _score).ToString("000000"), small, muted, w - 175, 28);
            }
        }

        private void DrawOverlay(Graphics g, int w, int h)
        {
            using (SolidBrush shade = new SolidBrush(Color.FromArgb(175, 5, 8, 19))) g.FillRectangle(shade, ClientRectangle);
            int boxW = Math.Min(520, w - 40);
            int boxH = 230;
            Rectangle box = new Rectangle((w - boxW) / 2, (h - boxH) / 2, boxW, boxH);
            using (SolidBrush card = new SolidBrush(Color.FromArgb(242, 19, 23, 43))) g.FillRectangle(card, box);
            using (Pen border = new Pen(Color.FromArgb(105, 126, 100, 255), 1.5F)) g.DrawRectangle(border, box);
            using (Font title = new Font("Segoe UI Semibold", 25F, FontStyle.Bold))
            using (Font body = new Font("Segoe UI", 10F))
            using (Font hint = new Font("Segoe UI Semibold", 9F, FontStyle.Bold))
            using (SolidBrush white = new SolidBrush(Color.White))
            using (SolidBrush lavender = new SolidBrush(Color.FromArgb(183, 169, 255)))
            using (SolidBrush muted = new SolidBrush(Color.FromArgb(172, 182, 211)))
            {
                string heading = _state == GameState.Ready ? "NEON DRIFT" : (_state == GameState.Paused ? "PAUSA" : "FIN DE PARTIDA");
                SizeF titleSize = g.MeasureString(heading, title);
                g.DrawString(heading, title, white, (w - titleSize.Width) / 2F, box.Y + 36);
                string description = _state == GameState.Ready
                    ? "Cinco carriles. Reflejos rápidos. Tu mejor marca."
                    : (_state == GameState.Paused ? "La carrera está detenida." : "Puntuación final: " + _score);
                SizeF bodySize = g.MeasureString(description, body);
                g.DrawString(description, body, muted, (w - bodySize.Width) / 2F, box.Y + 100);
                string action = _state == GameState.Paused ? "ESPACIO  ·  CONTINUAR" : "ESPACIO  ·  JUGAR";
                if (_state == GameState.Over) action = "ENTER / ESPACIO  ·  VOLVER A JUGAR";
                SizeF hintSize = g.MeasureString(action, hint);
                g.DrawString(action, hint, lavender, (w - hintSize.Width) / 2F, box.Y + 153);
                string controls = "← → o A / D para moverte     ·     ESC para salir";
                SizeF controlsSize = g.MeasureString(controls, body);
                g.DrawString(controls, body, muted, (w - controlsSize.Width) / 2F, box.Y + 188);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_state != GameState.Playing) return;
            _ticks++;
            if (_ticks % 8 == 0) _score++;
            _speed = 0.008F + Math.Min(0.010F, _score / 18000F);
            _spawnCounter++;
            int spawnEvery = Math.Max(24, 62 - _score / 30);
            if (_spawnCounter >= spawnEvery)
            {
                _spawnCounter = 0;
                int lane = _random.Next(0, 5);
                _obstacles.Add(new Obstacle { Lane = lane, Y = -0.10F });
            }

            for (int i = _obstacles.Count - 1; i >= 0; i--)
            {
                _obstacles[i].Y += _speed;
                if (_obstacles[i].Y > 1.08F) _obstacles.RemoveAt(i);
                else if (_obstacles[i].Lane == _lane && _obstacles[i].Y > 0.755F && _obstacles[i].Y < 0.91F)
                {
                    EndGame();
                    break;
                }
            }
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!_pressed.Add(e.KeyCode)) return;
            if (e.KeyCode == Keys.Escape)
            {
                if (ExitRequested != null) ExitRequested();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Space)
            {
                if (_state == GameState.Ready || _state == GameState.Over) StartGame();
                else if (_state == GameState.Playing) _state = GameState.Paused;
                else if (_state == GameState.Paused) _state = GameState.Playing;
                Invalidate();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Enter && _state == GameState.Over)
            {
                StartGame();
                e.Handled = true;
                return;
            }
            if (_state == GameState.Playing)
            {
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A) _lane = Math.Max(0, _lane - 1);
                if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D) _lane = Math.Min(4, _lane + 1);
                Invalidate();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _pressed.Remove(e.KeyCode);
            base.OnKeyUp(e);
        }

        private void StartGame()
        {
            _score = 0;
            _ticks = 0;
            _spawnCounter = 0;
            _speed = 0.008F;
            _lane = 2;
            _reported = false;
            _obstacles.Clear();
            _state = GameState.Playing;
            _timer.Start();
            Focus();
            Invalidate();
        }

        private void EndGame()
        {
            _timer.Stop();
            _state = GameState.Over;
            if (!_reported)
            {
                _reported = true;
                if (RunCompleted != null) RunCompleted(_score);
            }
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer.Dispose();
            base.Dispose(disposing);
        }
    }
}
