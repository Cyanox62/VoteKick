using System.Linq;
using System.Threading;

namespace VoteKick
{
	class CooldownHandler
	{
		public CooldownHandler(EventHandler ev)
		{
			while (ev.isRoundStarted)
			{
				if (ev.pCooldown.Count > 0)
				{
					foreach (var key in ev.pCooldown.Keys.ToList())
					{
						ev.pCooldown[key]--;
						if (ev.pCooldown[key] <= 0)
							ev.pCooldown.Remove(key);
					}
				}
				Thread.Sleep(1000);
			}
		}
	}
}
