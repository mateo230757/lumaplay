using System;
using System.Drawing;
using System.Windows.Forms;

namespace LumaPlay
{
    public sealed class SetupForm : Form
    {
        private readonly AppSettings _settings;
        private TextBox _profileBox;
        private ComboBox _accentBox;

        public SetupForm(AppSettings settings)
        {
            _settings = settings;
            Text = "Configura LumaPlay";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(560, 470);
            BackColor = Color.FromArgb(13, 15, 27);
            ForeColor = Color.FromArgb(239, 241, 255);
            Font = new Font("Segoe UI", 9F);

            Panel stripe = new Panel { Dock = DockStyle.Top, Height = 5, BackColor = Color.FromArgb(146, 110, 255) };
            Controls.Add(stripe);

            Label brand = new Label
            {
                Text = "LUMAPLAY",
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(178, 158, 255),
                Location = new Point(36, 28),
                AutoSize = true
            };
            Controls.Add(brand);

            Label title = new Label
            {
                Text = "Tu espacio de juego,\na tu manera.",
                Font = new Font("Segoe UI Semibold", 25F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(34, 72),
                Size = new Size(470, 78)
            };
            Controls.Add(title);

            Label subtitle = new Label
            {
                Text = "Antes de entrar, vamos a preparar tu perfil local.\nTus datos se guardan en este equipo.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(164, 171, 195),
                Location = new Point(38, 160),
                Size = new Size(465, 43)
            };
            Controls.Add(subtitle);

            Label profileLabel = MakeLabel("¿Cómo quieres que te llamemos?", 39, 225);
            Controls.Add(profileLabel);
            _profileBox = new TextBox
            {
                Location = new Point(38, 252),
                Size = new Size(470, 34),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(24, 27, 44),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Text = _settings.ProfileName
            };
            Controls.Add(_profileBox);

            Label accentLabel = MakeLabel("Color de acento", 39, 306);
            Controls.Add(accentLabel);
            _accentBox = new ComboBox
            {
                Location = new Point(38, 333),
                Size = new Size(220, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(24, 27, 44),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _accentBox.Items.AddRange(new object[] { "Violeta", "Cian", "Verde" });
            _accentBox.SelectedItem = _settings.Accent;
            if (_accentBox.SelectedIndex < 0) _accentBox.SelectedIndex = 0;
            Controls.Add(_accentBox);

            Button start = new Button
            {
                Text = "Crear perfil y entrar  →",
                Location = new Point(315, 398),
                Size = new Size(193, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(125, 91, 240),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            start.FlatAppearance.BorderSize = 0;
            start.Click += Start_Click;
            Controls.Add(start);
            AcceptButton = start;
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(211, 215, 234),
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold)
            };
        }

        private void Start_Click(object sender, EventArgs e)
        {
            string name = _profileBox.Text.Trim();
            if (name.Length < 2)
            {
                MessageBox.Show("Escribe un nombre de al menos 2 caracteres.", "Perfil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _profileBox.Focus();
                return;
            }
            _settings.ProfileName = name;
            _settings.Accent = Convert.ToString(_accentBox.SelectedItem);
            _settings.SetupComplete = true;
            _settings.Save();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
