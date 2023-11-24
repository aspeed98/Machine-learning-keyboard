using System.Text;

namespace keyboard_capture_v2
{
	public partial class CaptureForm : Form
	{
		private Random rnd = new Random();

		private List<string> wordsEn = new List<string>();
		private List<string> wordsRu = new List<string>();
		private Language currentLang = Language.English;
		
		private List<TestKeys> allkeys = new List<TestKeys>();
		private List<Tuple<TestKeys, DateTime>> pressedKeys = new List<Tuple<TestKeys, DateTime>>();

		private char? lastInput = null;
		private DateTime lastDate = DateTime.MinValue;
		private int values = 0;
		private int score = 0;

		public CaptureForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			wordsEn = Properties.Resources.english.Split('\n').ToList();
			wordsRu = Properties.Resources.russian.Split('\n').ToList();

			SetNextWord("qwerty");
			SetToolTips();

			for (int i = 0; i < KeyOriginVariants.variants().Count; i++)
				allkeys.Add(new TestKeys(KeyOriginVariants.origins()[i], KeyOriginVariants.variants()[i], KeyOriginVariants.keyValues()[KeyOriginVariants.variants()[i][0]]));
		}
		private void SetNextWord(string text)
		{
			wordLabel.ForeColor = Color.Black;
			wordLabel.Text = text;
			wordLabel.Location = new Point((this.Size.Width - wordLabel.Width) / 2 - 6, 12);
			inputBox.Text = string.Empty;
		}
		private void SetToolTips()
		{
			ToolTip tip = new ToolTip();
			tip.AutoPopDelay = 5000;
			tip.InitialDelay = 250;
			tip.ReshowDelay = 200;
			tip.ShowAlways = true;
			tip.SetToolTip(startbutton, "Have a struggle? Press Enter to skip the word.");
			tip.SetToolTip(combinebutton, "Combine several results to a single file.");
		}
		private void startbutton_Click(object sender, EventArgs e)
		{
			gamepanel.Visible = true;
			inputBox.Focus();
		}
		private void tosettingsbutton_Click(object sender, EventArgs e)
		{
			wordLabel_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
			gamepanel.Visible = false;
			startbutton.Focus();
		}
		private void combinebutton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Title = "Select several files to combine...";
				dialog.Filter = "JSON file|*.json";
				dialog.InitialDirectory = Environment.CurrentDirectory;
				dialog.Multiselect = true;
				dialog.ShowDialog();
				List<string> paths = dialog.FileNames.ToList();
				Keyboard result = Keyboard.combined(paths);
				if (paths.Count > 0 && result.totalSamples > 0)
				{
					for (int i = 0; i < paths.Count; i++)
					{
						try { File.Delete(paths[i]); }
						catch
						{
							DialogResult again = MessageBox.Show($"File:\n{paths[i].Substring(paths[i].LastIndexOf('\\') + 1)}\nis being used.\n Try again?", "Could not delete file", MessageBoxButtons.YesNo);
							if (again == DialogResult.Yes)
								i--;
						}
					}

					string name = paths[0][0..(paths[0].LastIndexOf('\\'))] + $"\\result - {result.totalSamples} samples ";
					while (File.Exists(name[0..(name.Length - 1)] + ".json"))
						name += $"{wordsEn[rnd.Next(wordsEn.Count)]} ";
					name = name[0..(name.Length - 1)] + ".json";

					File.WriteAllText(name, result.toJson(), Encoding.UTF8);
					MessageBox.Show($"Combined {paths.Count} files.\nResult has {result.totalSamples} samples.", "Combine result");
				}
				else
					MessageBox.Show("Nothing combined.", "Combine result");
				dialog.Dispose();
			}
		}
		private void switchLanguage_MouseDown(object sender, MouseEventArgs e)
		{
			switch (currentLang)
			{
				case Language.All:
					currentLang = Language.English;
					switchLanguage.Text = "En";
					break;
				case Language.English:
					currentLang = Language.Russian;
					switchLanguage.Text = "Ru";
					break;
				case Language.Russian:
					currentLang = Language.All;
					switchLanguage.Text = "All";
					break;
			}
			wordLabel_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
		}
		private void wordLabel_MouseDown(object sender, MouseEventArgs e)
		{
			if (rnd.NextDouble() < 0.9)
			{
				switch (currentLang)
				{
					case Language.English:
						SetNextWord(wordsEn[rnd.Next(wordsEn.Count)].ToLower());
						break;
					case Language.Russian:
						SetNextWord(wordsRu[rnd.Next(wordsRu.Count)].ToLower());
						break;
					case Language.All:
						if (rnd.NextDouble() < 0.5)
							SetNextWord(wordsRu[rnd.Next(wordsRu.Count)].ToLower());
						else
							SetNextWord(wordsEn[rnd.Next(wordsEn.Count)].ToLower());
						break;
					default:
						SetNextWord("qwerty");
						break;
				}
			}
			else
				SetNextWord(new string(Enumerable.Repeat(KeyOriginVariants.origins(), 5).Select(zxc => zxc[rnd.Next(zxc.Count)]).ToArray()));

			lastDate = DateTime.MinValue;
			lastInput = null;
		}
		private void inputBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				wordLabel_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
				e.Handled = true;
				e.SuppressKeyPress = true;
				return;
			}
			DateTime now = DateTime.Now;
			if (KeyOriginVariants.valueKeys().ContainsKey(e.KeyValue))
			{
				int index = -1;
				for (int i = 0; i < allkeys.Count; i++)
					if (allkeys[i].value == e.KeyValue)
					{ index = i; break; }

				if (index != -1)
				{
					if (!pressedKeys.Any(zxc => zxc.Item1 == allkeys[index]))
						pressedKeys.Add(new(allkeys[index], now));

					string lower = inputBox.Text.ToLower();
					if (lastDate != DateTime.MinValue)
					{
						int ms = (int)(now - lastDate).TotalMilliseconds;
						if (ms > 1 && ms < 1000)
							if (lower.Length < wordLabel.Text.Length)
								if (lower == wordLabel.Text[0..lower.Length])
								{
									if (!allkeys[index].chars.Contains(wordLabel.Text[lower.Length])) // mistake
									{
										int wordOrigin = -1;
										for (int i = 0; i < allkeys.Count; i++)
											if (allkeys[i].chars.Contains(wordLabel.Text[lower.Length]))
											{ wordOrigin = i; break; }

										if (wordOrigin != -1)
										{
											allkeys[wordOrigin].addMistake(allkeys[index].origin);
											allkeys[wordOrigin].addSample();
											AddValues();
										}
									}
									else
									{
										allkeys[index].addDown(ms, lastInput);
										allkeys[index].addSample();
										AddValues();
									}
								}
					}
				}
				lastInput = allkeys[index].origin;
			}
			lastDate = now;
		}
		private void AddValues()
		{
			values++;
			valuelabel.Text = $"Values: {values}";
		}
		private void inputBox_KeyUp(object sender, KeyEventArgs e)
		{
			DateTime now = DateTime.Now;
			for (int i = 0; i < pressedKeys.Count; i++)
			{
				if (pressedKeys[i].Item1.value == e.KeyValue)
				{
					int index = -1;
					for (int j = 0; j < allkeys.Count; j++)
						if (allkeys[j].value == e.KeyValue)
						{ index = j; break; }

					allkeys[index].addUp((int)(now - pressedKeys[i].Item2).TotalMilliseconds);
					pressedKeys.RemoveAt(i); i--;
				}
			}
		}
		private void inputBox_TextChanged(object sender, EventArgs e)
		{
			if (inputBox.Text.Length > 0)
				if (inputBox.Text.Length <= wordLabel.Text.Length)
				{
					if (wordLabel.Text[0..(inputBox.Text.Length)] == inputBox.Text)
						wordLabel.ForeColor = Color.Black;
					else
						wordLabel.ForeColor = Color.Crimson;
				}
				else
				{
					wordLabel.ForeColor = Color.Crimson;
				}
			else
				wordLabel.ForeColor = Color.Black;

			if (inputBox.Text.ToLower() == wordLabel.Text)
			{
				wordLabel.ForeColor = Color.Black;
				AddScore();
				wordLabel_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
			}
		}
		private void AddScore()
		{
			score++;
			scoreLabel.Text = $"Score: {score}";
		}
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			List<KeyPad> kpads = new List<KeyPad>(allkeys.Count);
			for (int i = 0; i < kpads.Capacity; i++)
				kpads.Add(allkeys[i].result());
			Keyboard keyboard = new Keyboard(kpads);

			if (keyboard.totalSamples > 0)
			{
				string resultPath = Environment.CurrentDirectory + $"\\result - {keyboard.totalSamples} samples ";
				while (File.Exists(resultPath[0..(resultPath.Length - 1)] + ".json"))
					resultPath += $"{wordsEn[rnd.Next(wordsEn.Count)]} ";
				resultPath = resultPath[0..(resultPath.Length - 1)] + ".json";

				File.WriteAllText(resultPath, keyboard.toJson(), Encoding.UTF8);
				MessageBox.Show($"Created result file with {keyboard.totalSamples} samples.", "Result status");
			}
			else
				MessageBox.Show("No file was created.", "Result status");

			base.OnFormClosing(e);
		}
	}
}