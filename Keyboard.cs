using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace keyboard_capture_v2
{
	/// <summary>
	/// A result class, containing all gathered KeyPad results and their average values.
	/// </summary>
	/// <remarks>
	/// The actual output of this application. This will be further used in keyboard typer scenarios.
	/// </remarks>
	public partial class Keyboard
	{
		public int totalSamples { get; set; }
		public double precision { get; set; }
		public int reaction { get; set; }
		public int downtime { get; set; }
		public List<KeyPad> keypads { get; set; }
		public Keyboard(List<KeyPad> keypads)
		{
			this.totalSamples = keypads.Select(zxc => zxc.samples).Sum();
			if (this.totalSamples > 0)
			{
				this.precision = keypads.Select(zxc => zxc.precision * zxc.samples).Sum() / this.totalSamples;
				this.reaction = keypads.Select(zxc => zxc.reaction * zxc.samples).Sum() / this.totalSamples;
				this.downtime = keypads.Select(zxc => zxc.downtime * zxc.samples).Sum() / this.totalSamples;
			}
			else
			{
				this.precision = 0;
				this.reaction = 0;
				this.downtime = 0;
			}
			this.keypads = new List<KeyPad>(keypads);
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
				this.precision = 0;
				this.reaction = 0;
				this.downtime = 0;
				this.keypads = new List<KeyPad>();
			}
			else
			{
				this.totalSamples = result.keypads.Select(zxc => zxc.samples).Sum();
				if (this.totalSamples > 0)
				{
					this.precision = result.keypads.Select(zxc => zxc.precision * zxc.samples).Sum() / this.totalSamples;
					this.reaction = result.keypads.Select(zxc => zxc.reaction * zxc.samples).Sum() / this.totalSamples;
					this.downtime = result.keypads.Select(zxc => zxc.downtime * zxc.samples).Sum() / this.totalSamples;
				}
				else
				{
					this.precision = 0;
					this.reaction = 0;
					this.downtime = 0;
				}
				this.keypads = result.keypads;
			}
		}
		public Keyboard()
		{
			this.totalSamples = 0;
			this.precision = 0;
			this.reaction = 0;
			this.downtime = 0;
			this.keypads = new List<KeyPad>();
		}
		
		public string toJson()
		{
			return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
		}
		public Keyboard copy()
		{
			return new Keyboard(new List<KeyPad>(this.keypads));
		}
	}
}
