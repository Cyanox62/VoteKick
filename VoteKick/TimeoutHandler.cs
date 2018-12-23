using System.Threading;
using Smod2;

namespace VoteKick
{
	class TimeoutHandler
	{
		public TimeoutHandler(Plugin plugin, EventHandler ev)
		{
			Thread.Sleep(plugin.GetConfigInt("votekick_timeout") * 1000); // 30 seconds
			ev.EndVote(true);
		}
	}
}
