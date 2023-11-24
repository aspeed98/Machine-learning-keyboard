namespace keyboard_capture_v2
{
	/// <summary>
	/// KeyPad class. This is the result product of previously recorded keystrokes.
	/// </summary>
	/// <remarks>
	/// Can only be obtained from TestKeys class.
	/// </remarks>
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
