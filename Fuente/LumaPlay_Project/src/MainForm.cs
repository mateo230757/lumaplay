using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LumaPlay
{
    public sealed class MainForm : Form
    {
        private readonly AppSettings _settings;
        private readonly Panel _content;
        private readonly Panel _sidebar;
        private readonly Label _pageTitle;
        private readonly Label _profileBadge;
        private readonly Button[] _navButtons;
        private GameCanvas _gameCanvas;
        private string _currentPage = "Inicio";

        public MainForm(AppSettings settings)
        {
            _settings = settings;
            _settings.Normalize();
            Theme.Apply(_settings.Accent);
            Text = "LumaPlay — Tu centro de videojuegos";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1024, 680);
            Size = new Size(1280, 820);
            BackColor = Theme.Background;
            ForeColor = Theme.Text;
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;

            _sidebar = new Panel { Dock = DockStyle.Left, Width = 218, BackColor = Theme.Sidebar };
            Panel right = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            Controls.Add(right);
            Controls.Add(_sidebar);
            _sidebar.BringToFront();
            BuildSidebar();

            Panel header = new Panel { Dock = DockStyle.Top, Height = 74, BackColor = Theme.Background };
            right.Controls.Add(header);
            _pageTitle = new Label
            {
                Text = "Inicio",
                Location = new Point(30, 23),
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold),
                ForeColor = Theme.Text
            };
            header.Controls.Add(_pageTitle);
            _profileBadge = new Label
            {
                Text = _settings.ProfileName,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(0, 20),
                Size = new Size(145, 34),
                BackColor = Theme.Card,
                ForeColor = Theme.Text,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold)
            };
            header.Controls.Add(_profileBadge);
            _profileBadge.Left = header.ClientSize.Width - _profileBadge.Width - 28;
            header.Resize += delegate { _profileBadge.Left = header.ClientSize.Width - _profileBadge.Width - 28; };

            _content = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            right.Controls.Add(_content);
            _content.BringToFront();
            header.BringToFront();

            _navButtons = new Button[6];
            ShowPage("Inicio");
            Shown += delegate { ShowPage(_currentPage); };
            FormClosed += delegate { _settings.Save(); };
        }

        private void BuildSidebar()
        {
            Panel brand = new Panel { Dock = DockStyle.Top, Height = 112, BackColor = Theme.Sidebar };
            _sidebar.Controls.Add(brand);
            Label mark = new Label
            {
                Text = "✦",
                Font = new Font("Segoe UI Symbol", 23F, FontStyle.Bold),
                ForeColor = Theme.Accent,
                Location = new Point(23, 27),
                Size = new Size(34, 38),
                TextAlign = ContentAlignment.MiddleCenter
            };
            brand.Controls.Add(mark);
            Label name = new Label
            {
                Text = "LUMAPLAY",
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(62, 34),
                AutoSize = true
            };
            brand.Controls.Add(name);
            Label sub = new Label
            {
                Text = "GAMING SPACE  ·  v1.1.1",
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Theme.Muted,
                Location = new Point(64, 57),
                AutoSize = true
            };
            brand.Controls.Add(sub);

            Panel line = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Theme.Border };
            _sidebar.Controls.Add(line);

            FlowLayoutPanel nav = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 340,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(12, 20, 12, 0),
                BackColor = Theme.Sidebar
            };
            _sidebar.Controls.Add(nav);
            string[] names = { "Inicio", "Descubrir", "Biblioteca", "Logros", "Ajustes", "Acerca de" };
            string[] symbols = { "⌂", "◈", "▣", "✧", "⚙", "ⓘ" };
            for (int i = 0; i < names.Length; i++)
            {
                Button b = new Button
                {
                    Text = "   " + symbols[i] + "     " + names[i],
                    Tag = names[i],
                    Size = new Size(190, 46),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Theme.Sidebar,
                    ForeColor = Theme.Muted,
                    Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(0, 0, 0, 5)
                };
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.MouseOverBackColor = Theme.CardHover;
                b.Click += Nav_Click;
                nav.Controls.Add(b);
                _navButtons[i] = b;
            }

            Panel footer = new Panel { Dock = DockStyle.Bottom, Height = 84, BackColor = Theme.Sidebar };
            _sidebar.Controls.Add(footer);
            Label local = new Label
            {
                Text = "●  MODO LOCAL",
                ForeColor = Color.FromArgb(112, 220, 174),
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                Location = new Point(25, 16),
                AutoSize = true
            };
            footer.Controls.Add(local);
            Label note = new Label
            {
                Text = "Tus datos permanecen\nen este dispositivo.",
                ForeColor = Theme.Muted,
                Font = new Font("Segoe UI", 8F),
                Location = new Point(25, 39),
                Size = new Size(170, 36)
            };
            footer.Controls.Add(note);
        }

        private void Nav_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null) ShowPage(Convert.ToString(button.Tag));
        }

        private void HighlightNavigation(string page)
        {
            foreach (Button b in _navButtons)
            {
                bool active = Convert.ToString(b.Tag) == page;
                b.BackColor = active ? Theme.Active : Theme.Sidebar;
                b.ForeColor = active ? Color.White : Theme.Muted;
                b.FlatAppearance.BorderColor = active ? Theme.Accent : Theme.Sidebar;
                b.FlatAppearance.BorderSize = active ? 1 : 0;
            }
        }

        private void ClearContent()
        {
            foreach (Control control in _content.Controls.Cast<Control>().ToArray())
            {
                _content.Controls.Remove(control);
                control.Dispose();
            }
            _gameCanvas = null;
        }

        private void ShowPage(string page)
        {
            _currentPage = page;
            ClearContent();
            _pageTitle.Text = page;
            HighlightNavigation(page);
            if (page == "Inicio") BuildHome();
            else if (page == "Descubrir") BuildDiscover();
            else if (page == "Biblioteca") BuildLibrary();
            else if (page == "Logros") BuildAchievements();
            else if (page == "Ajustes") BuildSettings();
            else if (page == "Acerca de") BuildAbout();
        }

        private FlowLayoutPanel NewStack()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(28, 22, 28, 30),
                BackColor = Theme.Background
            };
        }

        private int ContentWidth
        {
            get
            {
                int available = _content.ClientSize.Width;
                if (available < 100) available = ClientSize.Width - _sidebar.Width;
                return Math.Max(570, available - 82);
            }
        }

        private Label Heading(string text, int size)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = Theme.Text,
                Font = new Font("Segoe UI Semibold", size, FontStyle.Bold),
                Margin = new Padding(2, 9, 0, 12)
            };
        }

        private Label MutedText(string text, int size)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = Theme.Muted,
                Font = new Font("Segoe UI", size),
                Margin = new Padding(2, 0, 0, 12)
            };
        }

        private Panel Card(int width, int height)
        {
            return new Panel
            {
                Width = width,
                Height = height,
                BackColor = Theme.Card,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(16)
            };
        }

        private Button ActionButton(string text, bool primary, EventHandler click)
        {
            Button b = new Button
            {
                Text = text,
                Size = new Size(150, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = primary ? Theme.Accent : Theme.CardHover,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = primary ? Theme.AccentHover : Theme.Border;
            if (click != null) b.Click += click;
            return b;
        }

        private void BuildHome()
        {
            FlowLayoutPanel stack = NewStack();
            _content.Controls.Add(stack);

            GradientPanel hero = new GradientPanel
            {
                Width = ContentWidth,
                Height = 226,
                Margin = new Padding(0, 0, 0, 18),
                Padding = new Padding(27),
                StartColor = Color.FromArgb(51, 42, 105),
                EndColor = Color.FromArgb(22, 70, 100)
            };
            Label eyebrow = new Label
            {
                Text = "TU UNIVERSO, TUS REGLAS",
                Location = new Point(28, 24),
                AutoSize = true,
                ForeColor = Color.FromArgb(204, 195, 255),
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold)
            };
            hero.Controls.Add(eyebrow);
            Label welcome = new Label
            {
                Text = "¡Bienvenido, " + _settings.ProfileName + "!",
                Location = new Point(25, 54),
                Size = new Size(Math.Max(320, hero.Width - 55), 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 25F, FontStyle.Bold)
            };
            hero.Controls.Add(welcome);
            Label sub = new Label
            {
                Text = "Tu biblioteca está lista. Entra al arcade o añade los juegos que ya tienes en tu PC.",
                Location = new Point(29, 112),
                Size = new Size(Math.Max(360, hero.Width - 60), 42),
                ForeColor = Color.FromArgb(218, 220, 245),
                Font = new Font("Segoe UI", 9.5F)
            };
            hero.Controls.Add(sub);
            Button play = ActionButton("Jugar Neon Drift  →", true, delegate { LaunchBuiltIn(); });
            play.Location = new Point(28, 166);
            play.Size = new Size(186, 39);
            hero.Controls.Add(play);
            stack.Controls.Add(hero);

            stack.Controls.Add(Heading("Tu actividad", 15));
            FlowLayoutPanel metrics = new FlowLayoutPanel
            {
                Width = ContentWidth,
                Height = 104,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 15),
                BackColor = Theme.Background
            };
            metrics.Controls.Add(MetricCard("JUEGOS", _settings.Games.Count.ToString(), "En tu biblioteca"));
            metrics.Controls.Add(MetricCard("PARTIDAS", _settings.TotalSessions.ToString(), "Sesiones de arcade"));
            metrics.Controls.Add(MetricCard("RÉCORD", _settings.BestScore.ToString("N0"), "Neon Drift"));
            stack.Controls.Add(metrics);

            stack.Controls.Add(Heading("Continúa jugando", 15));
            stack.Controls.Add(GameCard(_settings.Games.First(g => g.Id == "neon-drift"), ContentWidth));
            stack.Controls.Add(Heading("Accesos rápidos", 15));
            Panel quick = Card(ContentWidth, 82);
            Label quickTitle = new Label { Text = "Añade un juego de tu PC", Location = new Point(17, 16), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold) };
            quick.Controls.Add(quickTitle);
            Label quickDesc = new Label { Text = "Selecciona el .exe de un juego instalado y aparecerá en tu biblioteca.", Location = new Point(17, 42), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8.5F) };
            quick.Controls.Add(quickDesc);
            Button add = ActionButton("Añadir juego", false, delegate { AddExternalGame(); });
            add.Location = new Point(quick.Width - 177, 21);
            quick.Controls.Add(add);
            stack.Controls.Add(quick);
        }

        private Panel MetricCard(string title, string value, string description)
        {
            Panel card = Card(Math.Max(165, (ContentWidth - 20) / 3), 88);
            Label small = new Label { Text = title, Location = new Point(14, 10), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI Semibold", 7.5F, FontStyle.Bold) };
            Label number = new Label { Text = value, Location = new Point(13, 29), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold) };
            Label hint = new Label { Text = description, Location = new Point(95, 43), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8F) };
            card.Controls.Add(small);
            card.Controls.Add(number);
            card.Controls.Add(hint);
            return card;
        }

        private void BuildDiscover()
        {
            FlowLayoutPanel stack = NewStack();
            _content.Controls.Add(stack);
            stack.Controls.Add(Heading("Descubre tu próximo juego", 19));
            stack.Controls.Add(MutedText("Experiencias disponibles dentro de esta versión de LumaPlay.", 9));
            stack.Controls.Add(CatalogCard("Neon Drift", "ARCADE  ·  REFLEJOS", "Conduce entre carriles, esquiva obstáculos y bate tu récord. Incluido y jugable sin conexión.", "DISPONIBLE", true, ContentWidth, delegate { LaunchBuiltIn(); }));
            stack.Controls.Add(CatalogCard("Pixel Garden", "CREATIVO  ·  RELAJANTE", "Un pequeño mundo de construcción y exploración. Este título todavía no forma parte del paquete.", "PRÓXIMAMENTE", false, ContentWidth, null));
            stack.Controls.Add(CatalogCard("Orbit Tactics", "ESTRATEGIA  ·  ESPACIAL", "Planea rutas y administra recursos en desafíos por turnos. Este título está en concepto.", "EN DESARROLLO", false, ContentWidth, null));
            stack.Controls.Add(MutedText("El catálogo de esta edición es local. No hay tienda en línea ni descargas de terceros integradas.", 8));
        }

        private Panel CatalogCard(string name, string category, string description, string status, bool available, int width, EventHandler launch)
        {
            Panel card = Card(width, 135);
            Panel icon = new Panel { Location = new Point(17, 19), Size = new Size(82, 82), BackColor = available ? Color.FromArgb(52, 52, 104) : Color.FromArgb(38, 43, 62) };
            Label glyph = new Label { Text = available ? "ϟ" : "✧", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = available ? Color.FromArgb(171, 159, 255) : Theme.Muted, Font = new Font("Segoe UI Symbol", 34F, FontStyle.Bold) };
            icon.Controls.Add(glyph);
            card.Controls.Add(icon);
            Label title = new Label { Text = name, Location = new Point(119, 15), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold) };
            card.Controls.Add(title);
            Label cat = new Label { Text = category, Location = new Point(120, 42), AutoSize = true, ForeColor = Theme.Accent, Font = new Font("Segoe UI Semibold", 7.5F, FontStyle.Bold) };
            card.Controls.Add(cat);
            Label desc = new Label { Text = description, Location = new Point(120, 66), Size = new Size(Math.Max(250, width - 345), 45), ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8.5F) };
            card.Controls.Add(desc);
            Label badge = new Label { Text = status, Location = new Point(width - 160, 19), Size = new Size(135, 22), TextAlign = ContentAlignment.MiddleCenter, BackColor = available ? Color.FromArgb(34, 83, 78) : Color.FromArgb(49, 53, 73), ForeColor = available ? Color.FromArgb(142, 238, 205) : Theme.Muted, Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold) };
            card.Controls.Add(badge);
            if (available)
            {
                Button open = ActionButton("Jugar", true, launch);
                open.Size = new Size(105, 33);
                open.Location = new Point(width - 145, 77);
                card.Controls.Add(open);
            }
            return card;
        }

        private void BuildLibrary()
        {
            FlowLayoutPanel stack = NewStack();
            _content.Controls.Add(stack);
            Panel top = new Panel { Width = ContentWidth, Height = 86, BackColor = Theme.Background, Margin = new Padding(0, 0, 0, 10) };
            Label intro = new Label { Text = "Tus juegos, reunidos en un solo lugar.", Location = new Point(0, 1), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 9F) };
            top.Controls.Add(intro);
            TextBox search = new TextBox
            {
                Text = "",
                Location = new Point(0, 32),
                Size = new Size(260, 32),
                BackColor = Theme.Input,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F)
            };
            top.Controls.Add(search);
            Label searchHint = new Label { Text = "Buscar en biblioteca...", Location = new Point(8, 39), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8F) };
            searchHint.Cursor = Cursors.IBeam;
            searchHint.BackColor = Theme.Input;
            searchHint.Click += delegate { search.Focus(); };
            top.Controls.Add(searchHint);
            search.TextChanged += delegate { searchHint.Visible = search.Text.Length == 0; };
            Button add = ActionButton("+ Añadir juego", true, delegate { AddExternalGame(); });
            add.Size = new Size(150, 36);
            add.Location = new Point(ContentWidth - 155, 30);
            top.Controls.Add(add);
            stack.Controls.Add(top);

            FlowLayoutPanel gameList = new FlowLayoutPanel
            {
                Width = ContentWidth,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Theme.Background,
                Margin = new Padding(0)
            };
            Action refresh = delegate
            {
                foreach (Control c in gameList.Controls.Cast<Control>().ToArray()) { gameList.Controls.Remove(c); c.Dispose(); }
                string query = search.Text.Trim();
                GameEntry[] matching = _settings.Games.Where(g => query.Length == 0 || g.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                foreach (GameEntry game in matching) gameList.Controls.Add(GameCard(game, ContentWidth));
                if (matching.Length == 0)
                {
                    Panel empty = Card(ContentWidth, 75);
                    Label emptyLabel = new Label { Text = "No hay juegos que coincidan con esa búsqueda.", Location = new Point(18, 25), AutoSize = true, ForeColor = Theme.Muted };
                    empty.Controls.Add(emptyLabel);
                    gameList.Controls.Add(empty);
                }
            };
            search.TextChanged += delegate { refresh(); };
            refresh();
            stack.Controls.Add(gameList);
            stack.Controls.Add(MutedText("LumaPlay guarda accesos a los ejecutables que selecciones; no copia, modifica ni descarga esos juegos.", 8));
        }

        private Panel GameCard(GameEntry game, int width)
        {
            Panel card = Card(width, 105);
            Panel icon = new Panel { Location = new Point(16, 16), Size = new Size(70, 70), BackColor = Color.FromArgb(50, 48, 96) };
            Label glyph = new Label { Text = game.IsBuiltIn ? "ϟ" : "▶", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Theme.Accent, Font = new Font("Segoe UI Symbol", 27F, FontStyle.Bold) };
            icon.Controls.Add(glyph);
            card.Controls.Add(icon);
            Label name = new Label { Text = game.Name, Location = new Point(104, 18), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold) };
            card.Controls.Add(name);
            Label desc = new Label { Text = game.IsBuiltIn ? "ARCADE INCLUIDO  ·  " + game.Description : "JUEGO LOCAL  ·  " + game.Description, Location = new Point(105, 47), Size = new Size(Math.Max(180, width - 330), 37), ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8F) };
            card.Controls.Add(desc);
            Button launch = ActionButton(game.IsBuiltIn ? "Jugar" : "Abrir", true, delegate { LaunchEntry(game); });
            launch.Location = new Point(width - 178, 31);
            launch.Size = new Size(88, 36);
            card.Controls.Add(launch);
            if (!game.IsBuiltIn)
            {
                Button remove = ActionButton("Quitar", false, delegate { RemoveGame(game); });
                remove.Location = new Point(width - 83, 31);
                remove.Size = new Size(70, 36);
                card.Controls.Add(remove);
            }
            return card;
        }

        private void BuildAchievements()
        {
            FlowLayoutPanel stack = NewStack();
            _content.Controls.Add(stack);
            stack.Controls.Add(Heading("Logros", 19));
            stack.Controls.Add(MutedText("Pequeños hitos de tu actividad en LumaPlay.", 9));
            AddAchievement(stack, "Primer despegue", "Juega tu primera partida de Neon Drift.", _settings.TotalSessions >= 1);
            AddAchievement(stack, "A toda velocidad", "Consigue 100 puntos en una partida.", _settings.BestScore >= 100);
            AddAchievement(stack, "Piloto experto", "Consigue 250 puntos en una partida.", _settings.BestScore >= 250);
            AddAchievement(stack, "De vuelta al circuito", "Completa 10 partidas.", _settings.TotalSessions >= 10);
            AddAchievement(stack, "Coleccionista", "Añade un juego externo a tu biblioteca.", _settings.Games.Any(g => !g.IsBuiltIn));
        }

        private void AddAchievement(FlowLayoutPanel stack, string title, string detail, bool unlocked)
        {
            Panel card = Card(ContentWidth, 82);
            Label icon = new Label
            {
                Text = unlocked ? "✦" : "◇",
                Location = new Point(18, 16),
                Size = new Size(45, 45),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = unlocked ? Color.FromArgb(60, 57, 103) : Color.FromArgb(39, 43, 59),
                ForeColor = unlocked ? Theme.Accent : Theme.Muted,
                Font = new Font("Segoe UI Symbol", 22F, FontStyle.Bold)
            };
            card.Controls.Add(icon);
            Label name = new Label { Text = title, Location = new Point(80, 15), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold) };
            card.Controls.Add(name);
            Label desc = new Label { Text = detail, Location = new Point(81, 42), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8.5F) };
            card.Controls.Add(desc);
            Label state = new Label { Text = unlocked ? "DESBLOQUEADO" : "BLOQUEADO", Location = new Point(card.Width - 150, 28), Size = new Size(126, 22), TextAlign = ContentAlignment.MiddleCenter, BackColor = unlocked ? Color.FromArgb(33, 79, 69) : Color.FromArgb(43, 48, 67), ForeColor = unlocked ? Color.FromArgb(142, 238, 205) : Theme.Muted, Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold) };
            card.Controls.Add(state);
            stack.Controls.Add(card);
        }

        private void BuildSettings()
        {
            FlowLayoutPanel stack = NewStack();
            _content.Controls.Add(stack);
            stack.Controls.Add(Heading("Personaliza tu espacio", 19));
            stack.Controls.Add(MutedText("Los cambios se guardan en este dispositivo.", 9));

            Panel profile = Card(ContentWidth, 190);
            Label title = new Label { Text = "Perfil", Location = new Point(18, 16), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold) };
            profile.Controls.Add(title);
            Label nameLabel = new Label { Text = "Nombre visible", Location = new Point(19, 58), AutoSize = true, ForeColor = Theme.Muted };
            profile.Controls.Add(nameLabel);
            TextBox nameBox = new TextBox { Text = _settings.ProfileName, Location = new Point(19, 82), Size = new Size(300, 31), BackColor = Theme.Input, ForeColor = Theme.Text, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 9F) };
            profile.Controls.Add(nameBox);
            Label accentLabel = new Label { Text = "Color de acento", Location = new Point(350, 58), AutoSize = true, ForeColor = Theme.Muted };
            profile.Controls.Add(accentLabel);
            ComboBox accent = new ComboBox { Location = new Point(350, 82), Size = new Size(180, 31), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Theme.Input, ForeColor = Theme.Text, FlatStyle = FlatStyle.Flat };
            accent.Items.AddRange(new object[] { "Violeta", "Cian", "Verde" });
            accent.SelectedItem = _settings.Accent;
            if (accent.SelectedIndex < 0) accent.SelectedIndex = 0;
            profile.Controls.Add(accent);
            Button save = ActionButton("Guardar cambios", true, delegate
            {
                if (nameBox.Text.Trim().Length < 2)
                {
                    MessageBox.Show("El nombre debe tener al menos 2 caracteres.", "Ajustes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _settings.ProfileName = nameBox.Text.Trim();
                _settings.Accent = Convert.ToString(accent.SelectedItem);
                _settings.Save();
                _profileBadge.Text = _settings.ProfileName;
                Theme.Apply(_settings.Accent);
                MessageBox.Show("Tus ajustes se guardaron.", "LumaPlay", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowPage("Ajustes");
            });
            save.Location = new Point(19, 135);
            profile.Controls.Add(save);
            stack.Controls.Add(profile);

            Panel data = Card(ContentWidth, 125);
            Label dataTitle = new Label { Text = "Datos locales", Location = new Point(18, 15), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold) };
            data.Controls.Add(dataTitle);
            Label path = new Label { Text = AppSettings.DataDirectory, Location = new Point(19, 48), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8F) };
            data.Controls.Add(path);
            Button openFolder = ActionButton("Abrir carpeta de datos", false, delegate
            {
                try { Process.Start("explorer.exe", AppSettings.DataDirectory); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "No se pudo abrir la carpeta", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            });
            openFolder.Location = new Point(19, 76);
            openFolder.Size = new Size(190, 34);
            data.Controls.Add(openFolder);
            stack.Controls.Add(data);

            Panel reset = Card(ContentWidth, 104);
            Label resetTitle = new Label { Text = "Reiniciar estadísticas", Location = new Point(18, 14), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold) };
            reset.Controls.Add(resetTitle);
            Label resetDesc = new Label { Text = "Pone a cero el récord y el contador de partidas. No elimina tus juegos añadidos.", Location = new Point(19, 41), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8F) };
            reset.Controls.Add(resetDesc);
            Button resetButton = ActionButton("Restablecer", false, delegate
            {
                DialogResult answer = MessageBox.Show("¿Seguro que quieres reiniciar tus estadísticas?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer == DialogResult.Yes)
                {
                    _settings.BestScore = 0;
                    _settings.TotalSessions = 0;
                    _settings.Save();
                    ShowPage("Ajustes");
                }
            });
            resetButton.Location = new Point(ContentWidth - 176, 34);
            resetButton.Size = new Size(145, 36);
            reset.Controls.Add(resetButton);
            stack.Controls.Add(reset);
        }

        private void BuildAbout()
        {
            FlowLayoutPanel stack = NewStack();
            _content.Controls.Add(stack);
            GradientPanel hero = new GradientPanel
            {
                Width = ContentWidth,
                Height = 180,
                StartColor = Color.FromArgb(51, 42, 105),
                EndColor = Color.FromArgb(17, 66, 87),
                Padding = new Padding(25),
                Margin = new Padding(0, 0, 0, 18)
            };
            Label brand = new Label { Text = "✦  LUMAPLAY", Location = new Point(25, 28), AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold) };
            hero.Controls.Add(brand);
            Label slogan = new Label { Text = "Un lugar para reunir, descubrir y jugar.", Location = new Point(29, 83), AutoSize = true, ForeColor = Color.FromArgb(223, 225, 249), Font = new Font("Segoe UI", 11F) };
            hero.Controls.Add(slogan);
            Label version = new Label { Text = "VERSIÓN 1.1.1  ·  WINDOWS DESKTOP  ·  AUTÓNOMA", Location = new Point(29, 125), AutoSize = true, ForeColor = Color.FromArgb(201, 192, 255), Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold) };
            hero.Controls.Add(version);
            stack.Controls.Add(hero);
            stack.Controls.Add(Heading("Qué incluye", 15));
            stack.Controls.Add(InfoCard("Biblioteca local", "Organiza tus accesos a juegos instalados y abre sus ejecutables desde un solo lugar."));
            stack.Controls.Add(InfoCard("Arcade integrado", "Neon Drift se ejecuta dentro de LumaPlay y guarda tu mejor puntuación."));
            stack.Controls.Add(InfoCard("Perfiles y progreso", "Guarda un nombre de jugador, estadísticas y logros en tu equipo."));
            stack.Controls.Add(InfoCard("Sin cuenta obligatoria", "Esta versión funciona localmente y no necesita iniciar sesión."));
            stack.Controls.Add(MutedText("LumaPlay es un proyecto independiente. No está afiliado a Steam, Xbox, PlayStation ni Roblox.", 8));
        }

        private Panel InfoCard(string title, string detail)
        {
            Panel card = Card(ContentWidth, 76);
            Label h = new Label { Text = title, Location = new Point(17, 13), AutoSize = true, ForeColor = Theme.Text, Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold) };
            Label d = new Label { Text = detail, Location = new Point(18, 39), AutoSize = true, ForeColor = Theme.Muted, Font = new Font("Segoe UI", 8.5F) };
            card.Controls.Add(h);
            card.Controls.Add(d);
            return card;
        }

        private void LaunchBuiltIn()
        {
            GameEntry game = _settings.Games.First(g => g.Id == "neon-drift");
            LaunchEntry(game);
        }

        private void LaunchEntry(GameEntry game)
        {
            if (game.IsBuiltIn)
            {
                OpenArcade();
                return;
            }
            if (String.IsNullOrWhiteSpace(game.ExecutablePath) || !File.Exists(game.ExecutablePath))
            {
                MessageBox.Show("No se encuentra el ejecutable de este juego. Puedes quitarlo y añadirlo de nuevo desde la biblioteca.", "Ruta no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Process.Start(new ProcessStartInfo { FileName = game.ExecutablePath, UseShellExecute = true });
                game.LaunchCount++;
                game.LastPlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                _settings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el juego.\n\n" + ex.Message, "Error al iniciar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenArcade()
        {
            ClearContent();
            _pageTitle.Text = "Neon Drift  ·  Arcade";
            HighlightNavigation("Biblioteca");
            Panel shell = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            Panel bar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Theme.Background };
            Label label = new Label { Text = "NEON DRIFT  /  ARCADE", Location = new Point(12, 14), AutoSize = true, ForeColor = Theme.Accent, Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold) };
            bar.Controls.Add(label);
            Button exit = ActionButton("← Volver a biblioteca", false, delegate { ShowPage("Biblioteca"); });
            exit.Location = new Point(Math.Max(400, _content.ClientSize.Width - 190), 6);
            exit.Size = new Size(170, 34);
            bar.Controls.Add(exit);
            bar.Resize += delegate { exit.Left = bar.ClientSize.Width - exit.Width - 10; };
            shell.Controls.Add(bar);
            _gameCanvas = new GameCanvas(_settings.BestScore) { Dock = DockStyle.Fill };
            _gameCanvas.RunCompleted += delegate(int score)
            {
                _settings.TotalSessions++;
                if (score > _settings.BestScore) _settings.BestScore = score;
                GameEntry entry = _settings.Games.First(g => g.Id == "neon-drift");
                entry.LaunchCount++;
                entry.LastPlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                _settings.Save();
            };
            _gameCanvas.ExitRequested += delegate { ShowPage("Biblioteca"); };
            shell.Controls.Add(_gameCanvas);
            _gameCanvas.BringToFront();
            bar.BringToFront();
            _content.Controls.Add(shell);
            _gameCanvas.Focus();
        }

        private void AddExternalGame()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Selecciona el ejecutable de tu juego";
                dialog.Filter = "Aplicaciones de Windows (*.exe)|*.exe|Todos los archivos (*.*)|*.*";
                dialog.CheckFileExists = true;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                if (_settings.Games.Any(g => !g.IsBuiltIn && String.Equals(g.ExecutablePath, dialog.FileName, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Ese juego ya está en tu biblioteca.", "LumaPlay", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GameEntry entry = new GameEntry
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = Path.GetFileNameWithoutExtension(dialog.FileName),
                    Description = "Añadido desde tu PC",
                    ExecutablePath = dialog.FileName,
                    IsBuiltIn = false,
                    LaunchCount = 0,
                    LastPlayed = "Nunca"
                };
                _settings.Games.Add(entry);
                _settings.Save();
                ShowPage("Biblioteca");
            }
        }

        private void RemoveGame(GameEntry game)
        {
            DialogResult answer = MessageBox.Show("¿Quitar " + game.Name + " de tu biblioteca?\nEl archivo original no se eliminará.", "Quitar juego", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;
            _settings.Games.Remove(game);
            _settings.Save();
            ShowPage("Biblioteca");
        }
    }
}
