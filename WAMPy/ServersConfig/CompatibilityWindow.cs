using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WDS
{
    /// <summary>
    /// Shows which installed versions can work together, how the program joins them,
    /// and the rules behind it.
    /// </summary>
    public class CompatibilityWindow : Form
    {
        private static readonly Color Red = Color.FromArgb(255, 222, 222);
        private static readonly Color Yellow = Color.FromArgb(255, 245, 205);
        private static readonly Color Green = Color.FromArgb(225, 245, 225);

        private DataGridView grid;

        public CompatibilityWindow()
        {
            this.Text = "Compatibility";
            this.Size = new Size(900, 560);
            this.MinimumSize = new Size(600, 360);
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.Icon = WDS.Properties.Resources.ServerGreen;

            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;

            TabPage installed = new TabPage("Installed versions");
            Label help = new Label();
            help.Dock = DockStyle.Top;
            help.Height = 36;
            help.Padding = new Padding(4);
            help.Text = "Every installed combination. Green: the program sets it up automatically. Yellow: it works with limits. " +
                        "Red: it cannot work. Bold rows use the versions selected in the main window.";
            this.grid = NewGrid(new String[] { "Component", "With", "Result", "How it is set up / why not" });
            installed.Controls.Add(this.grid);
            installed.Controls.Add(help);

            TabPage rules = new TabPage("Rules");
            DataGridView rulesGrid = NewGrid(new String[] { "Combination", "Result" });
            foreach (String[] rule in Compatibility.RulesTable())
            {
                int i = rulesGrid.Rows.Add(rule[0], rule[1]);
                rulesGrid.Rows[i].DefaultCellStyle.BackColor = rule[1].StartsWith("Incompatible") ? Red : Color.White;
            }
            rulesGrid.Columns[0].FillWeight = 45;
            rules.Controls.Add(rulesGrid);

            tabs.TabPages.Add(installed);
            tabs.TabPages.Add(rules);

            Button close = new Button();
            close.Text = "Close";
            close.DialogResult = DialogResult.OK;
            Panel bottom = new Panel();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 36;
            close.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            close.Location = new Point(bottom.Width - 90, 6);
            bottom.Controls.Add(close);
            bottom.Resize += (s, e) => close.Left = bottom.Width - close.Width - 10;
            this.AcceptButton = close;
            this.CancelButton = close;

            this.Controls.Add(tabs);
            this.Controls.Add(bottom);

            this.Load += (s, e) => this.Fill();
        }

        private static DataGridView NewGrid(String[] columns)
        {
            DataGridView g = new DataGridView();
            g.Dock = DockStyle.Fill;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.RowHeadersVisible = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.BackgroundColor = SystemColors.Window;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            g.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            g.DefaultCellStyle.SelectionForeColor = Color.Black;
            foreach (String c in columns)
            {
                g.Columns.Add(c.Replace(" ", ""), c);
            }
            if (columns.Length == 4)
            {
                g.Columns[0].FillWeight = 22;
                g.Columns[1].FillWeight = 22;
                g.Columns[2].FillWeight = 14;
                g.Columns[3].FillWeight = 60;
            }
            return g;
        }

        private void Fill()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                List<SoftwareComponent> webs = Compatibility.Installed(ComponentKind.WebServer);
                List<SoftwareComponent> phps = Compatibility.Installed(ComponentKind.PHP);
                List<SoftwareComponent> dbs = Compatibility.Installed(ComponentKind.DataBase);

                if (webs.Count + phps.Count + dbs.Count == 0)
                {
                    this.grid.Rows.Add("Nothing installed yet.", "", "", "Install servers from the Web Server, PHP and Database tabs.");
                    return;
                }

                // Each component on its own (Windows requirements...).
                List<SoftwareComponent> all = new List<SoftwareComponent>(webs);
                all.AddRange(phps);
                all.AddRange(dbs);
                foreach (SoftwareComponent c in all)
                {
                    List<Issue> issues = Compatibility.CheckAlone(c);
                    Severity worst = Severity.Info;
                    List<String> texts = new List<String>();
                    foreach (Issue i in issues)
                    {
                        if (i.Level > worst) worst = i.Level;
                        texts.Add(i.Text);
                    }
                    String result = (worst == Severity.Error) ? "Cannot run" : (worst == Severity.Warning) ? "Warning" : "OK";
                    AddRow(Title(c), "(on its own)", result, texts.Count > 0 ? String.Join(" ", texts.ToArray()) : "Runs on this computer.",
                        worst == Severity.Error ? Red : worst == Severity.Warning ? Yellow : Green, IsActive(c));
                }

                foreach (SoftwareComponent w in webs)
                {
                    foreach (SoftwareComponent p in phps)
                    {
                        AddPair(Compatibility.WebPhp(w, p), IsActive(w) && IsActive(p));
                    }
                }
                foreach (SoftwareComponent p in phps)
                {
                    foreach (SoftwareComponent d in dbs)
                    {
                        AddPair(Compatibility.PhpDb(p, d), IsActive(p) && IsActive(d));
                    }
                }
                if (webs.Count > 0 && dbs.Count > 0)
                {
                    AddRow("Any web server", "Any database", "Compatible", "They do not talk to each other directly.", Green, false);
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void AddPair(PairResult r, bool active)
        {
            String result;
            Color color;
            if (!r.Compatible)
            {
                result = "Incompatible";
                color = Red;
            }
            else if (r.Notes.Count > 0 || r.Link == PhpLink.Cgi)
            {
                result = "Compatible, with limits";
                color = Yellow;
            }
            else
            {
                result = "Compatible";
                color = Green;
            }
            String how = (r.Link != PhpLink.None && r.Compatible ? r.Method + ". " : "") + r.Summary;
            AddRow(Title(r.A), Title(r.B), result, how, color, active);
        }

        private void AddRow(String a, String b, String result, String how, Color color, bool bold)
        {
            int i = this.grid.Rows.Add(a, b, result, how);
            DataGridViewRow row = this.grid.Rows[i];
            row.DefaultCellStyle.BackColor = color;
            row.DefaultCellStyle.SelectionBackColor = ControlPaint.Dark(color, 0.05f);
            if (bold)
            {
                row.DefaultCellStyle.Font = new Font(this.grid.Font, FontStyle.Bold);
            }
        }

        private static String Title(SoftwareComponent c)
        {
            return c.Details + "\r\n" + c.Name;
        }

        private static bool IsActive(SoftwareComponent c)
        {
            switch (c.Kind)
            {
                case ComponentKind.PHP: return c.Name == Globals.ActivePHPVersion;
                case ComponentKind.DataBase: return c.Name == Globals.ActiveMySQLVersion;
            }
            return c.Name == Globals.ActiveWebServerVersion;
        }
    }
}
