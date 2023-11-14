using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace keyboard_capture_v2
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		private static Random rnd = new Random();
		private static List<string> wordsEn = new List<string>();
		private static List<string> wordsRu = new List<string>();
		private void Form1_Load(object sender, EventArgs e)
		{
			//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			wordsEn = Properties.Resources.english.Split('\n').ToList();
			wordsRu = Properties.Resources.russian.Split('\n').ToList();

			wordLabel.ForeColor = Color.Black;
			wordLabel.Text = "qwerty";
			wordLabel.Location = new Point((this.Size.Width - wordLabel.Width) / 2 - 6, 12);
			for (int i = 0; i < KeyOriginVariants.variants().Count; i++)
				allkeys.Add(new TestKeys(KeyOriginVariants.origins()[i], KeyOriginVariants.variants()[i], KeyOriginVariants.keyValues()[KeyOriginVariants.variants()[i][0]]));
			
			ToolTip tip = new ToolTip();
			tip.AutoPopDelay = 5000;
			tip.InitialDelay = 250;
			tip.ReshowDelay = 200;
			tip.ShowAlways = true;
			tip.SetToolTip(startbutton, "Have a struggle? Press Enter to skip the word.");
			tip.SetToolTip(combinebutton, "Combine several results to a single file.");
		}
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			List<KeyPad> kpads = new List<KeyPad>(allkeys.Count);
			for (int i = 0; i < kpads.Capacity; i++)
				kpads.Add(allkeys[i].result());
			Keyboard keyboard = new Keyboard(kpads);

			if (keyboard.totalSamples > 0)
			{
				//string resultPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\\result - {keyboard.totalSamples} samples ";
				string resultPath = Environment.CurrentDirectory + $"\\result - {keyboard.totalSamples} samples ";
				while (File.Exists(resultPath[0..(resultPath.Length - 1)] + ".json"))
					resultPath += $"{wordsEn[rnd.Next(wordsEn.Count)]} ";
				resultPath = resultPath[0..(resultPath.Length - 1)] + ".json";

				File.WriteAllText(resultPath, keyboard.toJson(), Encoding.UTF8);
			}

			base.OnFormClosing(e);
		}
		private Keyboard combined(List<string> paths)
		{
			if (paths.Count > 0)
			{
				List<Keyboard> kbs = new List<Keyboard>(paths.Count);
				for (int i = 0; i < paths.Count; i++)
				{
					Keyboard toadd = new Keyboard(paths[i]);
					if (toadd.totalSamples > 0)
						kbs.Add(toadd);
				}
				return combined(kbs);
			}
			return new Keyboard();
		}
		private Keyboard combined(string path1, string path2)
		{
			Keyboard kb = new Keyboard(path1);
			Keyboard nb = new Keyboard(path2);
			if (kb.totalSamples > 0 && nb.totalSamples > 0)
				return combined(kb, nb);
			else if (kb.totalSamples > 0)
				return kb;
			else if (nb.totalSamples > 0)
				return nb;
			return new Keyboard();
		}
		private Keyboard combined(List<Keyboard> kbs)
		{
			if (kbs.Count > 0)
			{
				Keyboard ret = kbs[0];
				for (int i = 1; i < kbs.Count; i++)
					ret = combined(ret, kbs[i]);
				return ret;
			}
			return new Keyboard();
		}
		private Keyboard combined(Keyboard kb, Keyboard nb)
		{
			if (kb.totalSamples > 0 && nb.totalSamples > 0)
			{
				int last = 0;
				for (int i = 0; i < Math.Min(kb.keypads.Count, nb.keypads.Count); i++)
				{
					last = i;
					int kbs = kb.keypads[i].samples;
					int nbs = nb.keypads[i].samples;
					if (kbs > 0 || nbs > 0)
					{
						kb.keypads[i].precision = (kb.keypads[i].precision * kbs + nb.keypads[i].precision * nbs) / (kbs + nbs);
						kb.keypads[i].reaction = (kb.keypads[i].reaction * kbs + nb.keypads[i].reaction * nbs) / (kbs + nbs);
						kb.keypads[i].downtime = (kb.keypads[i].downtime * kbs + nb.keypads[i].downtime * nbs) / (kbs + nbs);

						for (int j = 0; j < kb.keypads[i].reactions.Count; j++)
						{
							int kbr = kb.keypads[i].reactions.ElementAt(j).Value;
							int nbr = nb.keypads[i].reactions.ElementAt(j).Value;
							char key = kb.keypads[i].reactions.ElementAt(j).Key;
							kb.keypads[i].reactions[key] = (kbr * kbs + nbr * nbs) / (kbs + nbs);
						}
						for (int j = 0; j < kb.keypads[i].mistakes.Count; j++)
						{
							double kbm = kb.keypads[i].mistakes.ElementAt(j).Value;
							double nbm = nb.keypads[i].mistakes.ElementAt(j).Value;
							char key = kb.keypads[i].mistakes.ElementAt(j).Key;
							kb.keypads[i].mistakes[key] = (kbm * kbs + nbm * nbs) / (kbs + nbs);
						}
						double missSumm = kb.keypads[i].mistakes.Select(zxc => zxc.Value).Sum();
						if (missSumm != 0 && missSumm != 1)
							kb.keypads[i].mistakes = kb.keypads[i].mistakes.ToDictionary(zxc => zxc.Key, zxc => zxc.Value / missSumm);

						kb.keypads[i].samples += nb.keypads[i].samples;
					}
				}
				if (nb.keypads.Count > kb.keypads.Count)
					for (int i = last; i < nb.keypads.Count; i++)
						kb.keypads.Add(nb.keypads[i]);
				kb.totalSamples += nb.totalSamples;
				return kb;
			}
			else if (kb.totalSamples > 0)
			{
				return kb;
			}
			else if (nb.totalSamples > 0)
			{
				return nb;
			}
			return new Keyboard();
		}

		private static int values = 0;
		private static char? lastInput = null;
		private static DateTime lastDate = DateTime.MinValue;
		private static List<TestKeys> allkeys = new List<TestKeys>();
		private static List<Tuple<TestKeys, DateTime>> pressedKeys = new List<Tuple<TestKeys, DateTime>>();
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
											values++;
											valuelabel.Text = $"Values: {values}";
										}
									}
									else
									{
										allkeys[index].addDown(ms, lastInput);
										allkeys[index].addSample();
										values++;
										valuelabel.Text = $"Values: {values}";
									}
								}
					}
				}
				lastInput = allkeys[index].origin;
			}
			lastDate = DateTime.Now;
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
			lastDate = DateTime.Now;
		}

		private void wordLabel_MouseDown(object sender, MouseEventArgs e)
		{
			if (rnd.NextDouble() < 0.9)
			{
				switch (currentLang)
				{
					case Language.English:
						wordLabel.Text = wordsEn[rnd.Next(wordsEn.Count)].ToLower();
						break;
					case Language.Russian:
						wordLabel.Text = wordsRu[rnd.Next(wordsRu.Count)].ToLower();
						break;
					case Language.All:
						if (rnd.NextDouble() < 0.5)
							wordLabel.Text = wordsRu[rnd.Next(wordsRu.Count)].ToLower();
						else
							wordLabel.Text = wordsEn[rnd.Next(wordsEn.Count)].ToLower();
						break;
				}
			}
			else
				wordLabel.Text = new string(Enumerable.Repeat(KeyOriginVariants.origins(), 5).Select(zxc => zxc[rnd.Next(zxc.Count)]).ToArray());
			wordLabel.Location = new Point((this.Size.Width - wordLabel.Width) / 2 - 6, 12);
			inputBox.Text = String.Empty;
			lastDate = DateTime.MinValue;
			lastInput = null;
		}
		private static int score = 0;
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
				score++;
				scoreLabel.Text = $"Score: {score}";
				wordLabel_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
			}
		}

		private enum Language
		{
			All,
			English,
			Russian,
		}
		private static Language currentLang = Language.English;
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
		}

		private void combinebutton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Title = "Select several files to combine...";
				dialog.Multiselect = true;
				dialog.ShowDialog();
				List<string> paths = dialog.FileNames.ToList();
				Keyboard result = combined(paths);
				if (paths.Count > 0 && result.totalSamples > 0)
				{
					for (int i = 0; i < paths.Count; i++)
					{
						try { File.Delete(paths[i]); }
						catch { }
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

		private void startbutton_Click(object sender, EventArgs e)
		{
			gamepanel.Visible = true;
			inputBox.Focus();
		}

		private void tosettingsbutton_Click(object sender, EventArgs e)
		{
			wordLabel_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
			gamepanel.Visible = false;
		}
	}
	public class TestKeys
	{
		public char origin { get; }
		public List<char> chars { get; }
		public int value { get; }
		public int samples { get; private set; }
		public Dictionary<char, int> mistakes { get; private set; }
		public List<Tuple<char?, int>> reaction { get; private set; }
		public List<int> downtime { get; private set; }

		public TestKeys(char orig, List<char> chars, int value)
		{
			this.origin = orig;
			this.chars = chars;
			this.value = value;
			this.samples = 0;
			this.mistakes = new Dictionary<char, int>();
			this.reaction = new List<Tuple<char?, int>>();
			this.downtime = new List<int>();
		}
		public void addDown(int delay, char? key = null)
		{
			this.reaction.Add(new(key, delay));
		}
		public void addUp(int delay)
		{
			this.downtime.Add(delay);
		}
		public void addMistake(char key)
		{
			if (this.mistakes.ContainsKey(key))
				this.mistakes[key]++;
			else
				this.mistakes.Add(key, 1);
		}
		public void addSample()
		{
			this.samples++;
		}
		public KeyPad result()
		{
			reaction = reaction.Where(zxc => zxc.Item2 > 1 && zxc.Item2 < 1000).ToList();
			downtime = downtime.Where(zxc => zxc > 1 && zxc < 1000).ToList();
			if (this.samples > 0)
				try
				{
					int reactAVG = reaction.Select(zxc => zxc.Item2).Sum() / reaction.Count;
					int down = downtime.Sum() / downtime.Count;

					int hit = reaction.Count;
					int miss = mistakes.Select(zxc => zxc.Value).Sum();
					double prec = (double)(hit) / (double)(hit + miss);

					Dictionary<char, double> mist = new Dictionary<char, double>();
					Dictionary<char, int> react = new Dictionary<char, int>();
					List<char?> reactChars = reaction.Where(zxc => zxc.Item1 != null).Select(zxc => zxc.Item1).ToList();
					for (int i = 0; i < KeyOriginVariants.origins().Count; i++)
					{
						char toadd = KeyOriginVariants.origins()[i];
						mist.Add(toadd, (mistakes.ContainsKey(toadd) ? (double)mistakes[toadd] / miss : 0));
						if (reactChars.Contains(toadd))
						{
							List<int> cut = reaction.Where(zxc => zxc.Item1 == toadd).Select(zxc => zxc.Item2).ToList();
							react.Add(toadd, cut.Sum() / cut.Count);
						}
						else
						{
							react.Add(toadd, reactAVG);
						}
					}
					return new KeyPad(this.origin, this.chars, this.value, this.samples, prec, reactAVG, down, react, mist);
				}
				catch { }

			Dictionary<char, double> mistEmpty = new Dictionary<char, double>();
			Dictionary<char, int> reactEmpty = new Dictionary<char, int>();
			for (int i = 0; i < KeyOriginVariants.origins().Count; i++)
			{
				char toadd = KeyOriginVariants.origins()[i];
				mistEmpty.Add(toadd, 0);
				reactEmpty.Add(toadd, 0);
			}
			return new KeyPad(this.origin, this.chars, this.value, 0, 0, 0, 0, reactEmpty, mistEmpty);
		}
	}
	public static class KeyOriginVariants
	{
		public static Dictionary<int, char> valueKeys()
		{
			return new Dictionary<int, char>()
			{
				{ 81, 'q' },
				{ 87, 'w' },
				{ 69, 'e' },
				{ 82, 'r' },
				{ 84, 't' },
				{ 89, 'y' },
				{ 85, 'u' },
				{ 73, 'i' },
				{ 79, 'o' },
				{ 80, 'p' },
				{ 219, '[' },
				{ 221, ']' },
				{ 65, 'a' },
				{ 83, 's' },
				{ 68, 'd' },
				{ 70, 'f' },
				{ 71, 'g' },
				{ 72, 'h' },
				{ 74, 'j' },
				{ 75, 'k' },
				{ 76, 'l' },
				{ 186, ';' },
				{ 222, '\'' },
				{ 220, '\\' },
				{ 90, 'z' },
				{ 88, 'x' },
				{ 67, 'c' },
				{ 86, 'v' },
				{ 66, 'b' },
				{ 78, 'n' },
				{ 77, 'm' },
				{ 188, ',' },
				{ 190, '.' },
				{ 191, '/' },
				{ 192, '`' },
				{ 49, '1' },
				{ 50, '2' },
				{ 51, '3' },
				{ 52, '4' },
				{ 53, '5' },
				{ 54, '6' },
				{ 55, '7' },
				{ 56, '8' },
				{ 57, '9' },
				{ 48, '0' },
				{ 189, '-' },
				{ 187, '=' },

			};
		}
		public static Dictionary<char, int> keyValues()
		{
			return valueKeys().ToDictionary(zxc => zxc.Value, zxc => zxc.Key);
		}
		public static List<char> origins()
		{
			return new List<char>()
			{
				'q',
				'w',
				'e',
				'r',
				't',
				'y',
				'u',
				'i',
				'o',
				'p',
				'[',
				']',
				'a',
				's',
				'd',
				'f',
				'g',
				'h',
				'j',
				'k',
				'l',
				';',
				'\'',
				'\\',
				'z',
				'x',
				'c',
				'v',
				'b',
				'n',
				'm',
				',',
				'.',
				'/',
				'`',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				'0',
				'-',
				'=',
			};
		}
		public static List<List<char>> variants()
		{
			return new List<List<char>>()
			{
				"qQйЙ".ToList(),
				"wWцЦ".ToList(),
				"eEуУ".ToList(),
				"rRкК".ToList(),
				"tTеЕ".ToList(),
				"yYнН".ToList(),
				"uUгГ".ToList(),
				"iIшШ".ToList(),
				"oOщЩ".ToList(),
				"pPзЗ".ToList(),
				"[{хХ".ToList(),
				"]}ъЪ".ToList(),
				"aAфФ".ToList(),
				"sSыЫ".ToList(),
				"dDвВ".ToList(),
				"fFаА".ToList(),
				"gGпП".ToList(),
				"hHрР".ToList(),
				"jJоО".ToList(),
				"kKлЛ".ToList(),
				"lLдД".ToList(),
				";:жЖ".ToList(),
				"'\"эЭ".ToList(),
				"\\|\\/".ToList(),
				"zZяЯ".ToList(),
				"xXчЧ".ToList(),
				"cCсС".ToList(),
				"vVмМ".ToList(),
				"bBиИ".ToList(),
				"nNтТ".ToList(),
				"mMьЬ".ToList(),
				",<бБ".ToList(),
				".>юЮ".ToList(),
				"/?.,".ToList(),
				"`~ёЁ".ToList(),
				"1!1!".ToList(),
				"2@2\"".ToList(),
				"3#3№".ToList(),
				"4$4;".ToList(),
				"5%5%".ToList(),
				"6^6:".ToList(),
				"7&7?".ToList(),
				"8*8*".ToList(),
				"9(9(".ToList(),
				"0)0)".ToList(),
				"-_-_".ToList(),
				"=+=+".ToList(),
			};
		}
	}
	public class Keyboard
	{
		public int totalSamples { get; set; }
		public List<KeyPad> keypads { get; set; }
		public Keyboard(List<KeyPad> keypads)
		{
			this.totalSamples = keypads.Select(zxc => zxc.samples).Sum();
			this.keypads = keypads;
		}
		public Keyboard(string json, bool isFile = true)
		{
			Keyboard? result;
			if (isFile)
				result = JsonSerializer.Deserialize<Keyboard>(File.ReadAllText(json, Encoding.UTF8), new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
			else
				result = JsonSerializer.Deserialize<Keyboard>(json, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
			if (result == null)
			{
				this.totalSamples = 0;
				this.keypads = new List<KeyPad>();
			}
			else
			{
				this.totalSamples = result.keypads.Select(zxc => zxc.samples).Sum();
				this.keypads = result.keypads;
			}
		}
		public Keyboard()
		{
			this.totalSamples = 0;
			this.keypads = new List<KeyPad>();
		}
		public string toJson()
		{
			return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
		}
	}
	public class KeyPad
	{
		public char origin { get; }
		public List<char> chars { get; }
		public int value { get; }
		public int samples { get; set; }
		public double precision { get; set; }
		public int reaction { get; set; }
		public int downtime { get; set; }
		public Dictionary<char, int> reactions { get; set; }
		public Dictionary<char, double> mistakes { get; set; }
		public KeyPad(char origin, List<char> chars, int value, int samples, double precision, int reaction, int downtime, Dictionary<char, int> reactions, Dictionary<char, double> mistakes)
		{
			this.origin = origin;
			this.chars = chars;
			this.value = value;
			this.samples = samples;
			this.precision = precision;
			this.reaction = reaction;
			this.downtime = downtime;
			this.reactions = reactions;
			this.mistakes = mistakes;
		}
	}
}