﻿using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.IO;

namespace VoteKick
{
	partial class EventHandler : IEventHandlerCallCommand, IEventHandlerWaitingForPlayers, IEventHandlerRoundStart, IEventHandlerRoundEnd
	{
		Broadcast b = null;
		Player caller = null;
		Player target = null;

		public bool isVoting = false;
		public bool isRoundStarted = false;
		public bool onCooldown = false;

		public List<string> vPlayers = new List<string>();

		public float vYes = 0;
		public float vNo = 0;

		public int vPlayerCount;
		public int vCooldown;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			LoadConfigs();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			isRoundStarted = true;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			isRoundStarted = false;
			if (isVoting) CancelVote();
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			b = UnityEngine.Object.FindObjectOfType<Broadcast>();
			string cmd = ev.Command.ToLower();

			if (cmd.StartsWith("votekick"))
			{
				if (VoteKick.isPlayerBanned(ev.Player))
				{
					ev.ReturnMessage = "You have been banned from using VoteKick.";
					return;
				}
				vPlayerCount = PluginManager.Manager.Server.GetPlayers().Count;
				ev.ReturnMessage = StartVote(ev.Command, ev.Player);
			}
			else if (cmd == "voteyes")
			{
				if (VoteKick.isPlayerBanned(ev.Player))
				{
					ev.ReturnMessage = "You have been banned from using VoteKick.";
					return;
				}
				ev.ReturnMessage = Vote(true, ev.Player);
			}
			else if (cmd == "voteno")
			{
				if (VoteKick.isPlayerBanned(ev.Player))
				{
					ev.ReturnMessage = "You have been banned from using VoteKick.";
					return;
				}
				ev.ReturnMessage = Vote(false, ev.Player);
			}
		}
	}
}
