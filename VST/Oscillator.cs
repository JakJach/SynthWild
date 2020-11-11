using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VST
{
    public class Oscillator : GroupBox
    {
        public Oscillator()
        {
            this.Controls.Add(new Button()
            {
                Name = "Sine",
                Location = new Point(10, 20),
                Text = "Sine",
                BackColor = Color.Gold
            });

            this.Controls.Add(new Button()
            {
                Name = "Square",
                Location = new Point(95, 20),
                Text = "Square",
            });

            this.Controls.Add(new Button()
            {
                Name = "Saw",
                Location = new Point(180, 20),
                Text = "Saw",
            });

            this.Controls.Add(new Button()
            {
                Name = "Triangle",
                Location = new Point(265, 20),
                Text = "Triangle",
            });

            this.Controls.Add(new Button()
            {
                Name = "Noise",
                Location = new Point(350, 20),
                Text = "Noise",
            });

            foreach(Control control in Controls)
            {
                control.Size = new Size(75, 20);
                control.Font = new Font("Calibri", 8);
                control.Click += WaveButton_Click;
            }

            Controls.Add(new CheckBox()
            {
                Name = "On/Off",
                Location = new Point(450,20),
                Size = new Size(20,20),
                Text ="On/Off",
                Checked = true
            });
        }
        #region Properties
        public EWaveFormType WaveForm { get; private set; }
        public bool Status => ((CheckBox)Controls["On/Off"]).Checked;
        #endregion

        #region Methods
        public void WaveButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            WaveForm = (EWaveFormType)Enum.Parse(typeof(EWaveFormType), button.Text);
            foreach (Button b in Controls.OfType<Button>())
                b.UseVisualStyleBackColor = true;
            button.BackColor = Color.Gold;
        }
        #endregion
    }
}
