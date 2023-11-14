using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;

namespace keyboard_capture_v2
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			inputBox = new TextBox();
			wordLabel = new Label();
			switchLanguage = new Button();
			scoreLabel = new Label();
			valuelabel = new Label();
			gamepanel = new Panel();
			tosettingsbutton = new Button();
			settingspanel = new Panel();
			combinebutton = new Button();
			infolabel = new Label();
			startbutton = new Button();
			gamepanel.SuspendLayout();
			settingspanel.SuspendLayout();
			SuspendLayout();
			// 
			// inputBox
			// 
			inputBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			inputBox.Location = new Point(9, 54);
			inputBox.Name = "inputBox";
			inputBox.Size = new Size(560, 29);
			inputBox.TabIndex = 0;
			inputBox.TextAlign = HorizontalAlignment.Center;
			inputBox.TextChanged += inputBox_TextChanged;
			inputBox.KeyDown += inputBox_KeyDown;
			inputBox.KeyUp += inputBox_KeyUp;
			// 
			// wordLabel
			// 
			wordLabel.AutoSize = true;
			wordLabel.BackColor = SystemColors.Control;
			wordLabel.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
			wordLabel.ForeColor = SystemColors.ControlText;
			wordLabel.Location = new Point(230, 9);
			wordLabel.Name = "wordLabel";
			wordLabel.Size = new Size(108, 30);
			wordLabel.TabIndex = 1;
			wordLabel.Text = "wordLabel";
			wordLabel.TextAlign = ContentAlignment.MiddleLeft;
			wordLabel.MouseDown += wordLabel_MouseDown;
			// 
			// switchLanguage
			// 
			switchLanguage.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
			switchLanguage.Location = new Point(529, 9);
			switchLanguage.Name = "switchLanguage";
			switchLanguage.Size = new Size(40, 30);
			switchLanguage.TabIndex = 2;
			switchLanguage.Text = "En";
			switchLanguage.UseVisualStyleBackColor = true;
			switchLanguage.MouseDown += switchLanguage_MouseDown;
			// 
			// scoreLabel
			// 
			scoreLabel.AutoSize = true;
			scoreLabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
			scoreLabel.Location = new Point(9, 9);
			scoreLabel.Name = "scoreLabel";
			scoreLabel.Size = new Size(55, 17);
			scoreLabel.TabIndex = 3;
			scoreLabel.Text = "Score: 0";
			// 
			// valuelabel
			// 
			valuelabel.AutoSize = true;
			valuelabel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
			valuelabel.Location = new Point(9, 26);
			valuelabel.Name = "valuelabel";
			valuelabel.Size = new Size(60, 17);
			valuelabel.TabIndex = 4;
			valuelabel.Text = "Values: 0";
			// 
			// gamepanel
			// 
			gamepanel.Controls.Add(tosettingsbutton);
			gamepanel.Controls.Add(scoreLabel);
			gamepanel.Controls.Add(valuelabel);
			gamepanel.Controls.Add(wordLabel);
			gamepanel.Controls.Add(switchLanguage);
			gamepanel.Controls.Add(inputBox);
			gamepanel.Location = new Point(0, 0);
			gamepanel.Name = "gamepanel";
			gamepanel.Size = new Size(578, 92);
			gamepanel.TabIndex = 5;
			gamepanel.Visible = false;
			// 
			// tosettingsbutton
			// 
			tosettingsbutton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
			tosettingsbutton.Location = new Point(459, 9);
			tosettingsbutton.Name = "tosettingsbutton";
			tosettingsbutton.Size = new Size(64, 30);
			tosettingsbutton.TabIndex = 5;
			tosettingsbutton.Text = "Settings";
			tosettingsbutton.UseVisualStyleBackColor = true;
			tosettingsbutton.Click += tosettingsbutton_Click;
			// 
			// settingspanel
			// 
			settingspanel.Controls.Add(gamepanel);
			settingspanel.Controls.Add(combinebutton);
			settingspanel.Controls.Add(infolabel);
			settingspanel.Controls.Add(startbutton);
			settingspanel.Location = new Point(3, 3);
			settingspanel.Name = "settingspanel";
			settingspanel.Size = new Size(578, 92);
			settingspanel.TabIndex = 6;
			// 
			// combinebutton
			// 
			combinebutton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			combinebutton.Location = new Point(178, 55);
			combinebutton.Name = "combinebutton";
			combinebutton.Size = new Size(133, 30);
			combinebutton.TabIndex = 2;
			combinebutton.Text = "Combine results";
			combinebutton.UseVisualStyleBackColor = true;
			combinebutton.Click += combinebutton_Click;
			// 
			// infolabel
			// 
			infolabel.AutoSize = true;
			infolabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
			infolabel.Location = new Point(29, 7);
			infolabel.Name = "infolabel";
			infolabel.Size = new Size(523, 40);
			infolabel.TabIndex = 1;
			infolabel.Text = "This application will capture your keystrokes, including mistakes, and will\r\npreserve result information in \"result.json\" file placed in the executable folder.";
			infolabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// startbutton
			// 
			startbutton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			startbutton.Location = new Point(317, 55);
			startbutton.Name = "startbutton";
			startbutton.Size = new Size(62, 30);
			startbutton.TabIndex = 0;
			startbutton.Text = "Start";
			startbutton.UseVisualStyleBackColor = true;
			startbutton.Click += startbutton_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(584, 98);
			Controls.Add(settingspanel);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Name = "Form1";
			Text = "Keyboard Capture";
			Load += Form1_Load;
			gamepanel.ResumeLayout(false);
			gamepanel.PerformLayout();
			settingspanel.ResumeLayout(false);
			settingspanel.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private TextBox inputBox;
		private Label wordLabel;
		private Label scoreLabel;
		private Button switchLanguage;
		private Label valuelabel;
		private Panel gamepanel;
		private Panel settingspanel;
		private Button startbutton;
		private Label infolabel;
		private Button combinebutton;
		private Button tosettingsbutton;
	}
}