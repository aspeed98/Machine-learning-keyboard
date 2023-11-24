namespace keyboard_capture_v2
{
	/// <summary>
	/// TestKeys Class. Used for collecting initial input information on this form.
	/// </summary>
	/// <remarks>
	/// Can create a KeyPad class as a result of recorded keystrokes.
	/// </remarks>
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
}
