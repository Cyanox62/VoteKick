extern alias xyz;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Threading;

namespace VoteKick
{
	class EventHandler : IEventHandlerCallCommand, IEventHandlerRoundStart, IEventHandlerRoundEnd
	{
		private Plugin plugin;

		Broadcast b = null;
		Player caller = null;
		Player target = null;

		public bool isVoting = false;
		public bool isRoundStarted = false;

		public Dictionary<string, int> pCooldown = new Dictionary<string, int>();

		public int vYes = 0;
		public int vNo = 0;

		public EventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			isRoundStarted = true;
			b = xyz.UnityEngine.Object.FindObjectOfType<Broadcast>();
			Thread CooldownHandler = new Thread(new ThreadStart(() => new CooldownHandler(this)));
			CooldownHandler.Start();
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			isRoundStarted = false;
			vYes = 0;
			vNo = 0;
		}

		public void EndVote(bool timedout = false)
		{
			if (timedout)
			{
				b.CallRpcAddElement("The vote to kick '" + target.Name + "' has timed out.", 10, false);
			}
			if (vYes == vNo)
			{
				b.CallRpcAddElement("The vote to kick '" + target.Name + "' is a tie, player will not be kicked.", 10, false);
			}
			else if (vYes > vNo && vYes + vNo > plugin.GetConfigInt("votekick_minimum_votes") - 1)
			{
				plugin.Info(target.Name);
				b.CallRpcAddElement("The vote to kick '" + target.Name + "' has passed. Kicking player...", 10, false);
				target.Disconnect("You have been vote kicked by " + caller.Name + ".");
			}
			else
			{
				b.CallRpcAddElement("The vote to kick '" + target.Name + "' has failed.", 10, false);
			}

			isVoting = false;
			pCooldown.Add(caller.SteamId, plugin.GetConfigInt("votekick_cooldown"));
			vYes = 0;
			vNo = 0;
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			if (ev.Command.ToLower().StartsWith("votekick"))
			{
				if (isRoundStarted)
				{
					if (!isVoting)
					{
						string[] args = VoteKick.StringToStringArray(ev.Command.Replace("votekick ", ""));

						if (args.Length > 0)
						{
							if (args[0].Length > 0)
							{
								target = VoteKick.GetPlayer(args[0], out target);
								if (target != null)
								{
									caller = ev.Player;
									if (!pCooldown.ContainsKey(caller.SteamId))
									{
										b.CallRpcAddElement(caller.Name + " has initiated a vote kick against " + target.Name + ". To vote, press ` and type '.voteyes' or .'voteno'.", 10, false);
										ev.ReturnMessage = "Vote has been started.";
										isVoting = true;
										Thread TimeoutHandler = new Thread(new ThreadStart(() => new TimeoutHandler(plugin, this)));
										TimeoutHandler.Start();
									}
									else
									{
										ev.ReturnMessage = "Error: you cannot start a vote for another " + pCooldown[caller.SteamId] + " seconds.";
									}
								}
								else
								{
									ev.ReturnMessage = "Error: invalid player.";
								}
							}
						}
						else
						{
							ev.ReturnMessage = "VOTEKICK [PLAYER NAME]";
						}
					}
					else
					{
						ev.ReturnMessage = "Error: vote in progress.";
					}
				}
				else
				{
					ev.ReturnMessage = "Error: round must be started to vote kick.";
				}
			}

			if (ev.Command.ToLower() == "voteyes")
			{
				if (isVoting)
				{
					vYes++;
					ev.ReturnMessage = "Vote registered.";
				}
				else
				{
					ev.ReturnMessage = "Error: there is not an active vote.";
				}
			}

			if (ev.Command.ToLower() == "voteno")
			{
				if (isVoting)
				{
					vNo++;
					ev.ReturnMessage = "Vote registered.";
				}
				else
				{
					ev.ReturnMessage = "Error: there is not an active vote.";
				}
			}

			if (vYes + vNo == plugin.Server.GetPlayers().Count)
			{
				EndVote();
			}
		}
	}
}
