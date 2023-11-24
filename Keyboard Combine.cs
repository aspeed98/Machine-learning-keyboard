namespace keyboard_capture_v2
{
	/// <summary>
	/// A result class, containing all gathered KeyPad results and their average values.
	/// </summary>
	/// <remarks>
	/// Includes Methods to combine several Keyboard results into one object.
	/// </remarks>
	public partial class Keyboard
	{
		public static Keyboard combined(List<string> paths)
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
		public static Keyboard combined(List<Keyboard> kbs)
		{
			if (kbs.Count > 0)
			{
				Keyboard ret = new Keyboard();
				for (int i = 0; i < kbs.Count; i++)
					ret = combined(ret, kbs[i]);
				return ret;
			}
			return new Keyboard();
		}
		public static Keyboard combined(Keyboard kb, Keyboard nb)
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
				kb.precision = (kb.precision * kb.totalSamples + nb.precision * nb.totalSamples) / (kb.totalSamples + nb.totalSamples);
				kb.reaction = (kb.reaction * kb.totalSamples + nb.reaction * nb.totalSamples) / (kb.totalSamples + nb.totalSamples);
				kb.downtime = (kb.downtime * kb.totalSamples + nb.downtime * nb.totalSamples) / (kb.totalSamples + nb.totalSamples);
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
	}
}
