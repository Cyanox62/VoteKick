using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Threading;
using scp4aiur;
using System;

namespace VoteKick
{
	partial class EventHandler
	{
		public void LoadConfigs()
		{
			VoteKick.cMinVotes = VoteKick.instance.GetConfigInt("vk_minimum_votes");
			VoteKick.cCooldown = VoteKick.instance.GetConfigInt("vk_cooldown");
			VoteKick.cTimeout = VoteKick.instance.GetConfigFloat("vk_timeout");
			VoteKick.cPassPercent = VoteKick.instance.GetConfigInt("vk_pass_percent");
			VoteKick.cCooldownOnPass = VoteKick.instance.GetConfigBool("vk_cooldown_on_pass");
			VoteKick.cCooldownOnFail = VoteKick.instance.GetConfigBool("vk_cooldown_on_fail");
			VoteKick.cRankWhitelist = new List<string>(VoteKick.instance.GetConfigList("vk_rank_whitelist"));
		}

		public void StartCooldown()
		{
			onCooldown = true;
			vCooldown = VoteKick.cCooldown;

			Timing.In(x => RefreshCooldown(), 1);
		}

		public void RefreshCooldown()
		{
			vCooldown--;
			if (vCooldown <= 0)
				onCooldown = false;
			else
				Timing.In(x => RefreshCooldown(), 1);
		}

		public string StartVote(string command, Player player)
		{
			string rMessage = "";

			if (isRoundStarted)
			{
				if (!isVoting)
				{
					string[] args = VoteKick.StringToStringArray(command.Replace("votekick ", ""));

					if (args.Length > 0)
					{
						target = VoteKick.GetPlayer(args[0], out target);
						if (target != null)
						{
							if (player.SteamId != target.SteamId)
							{
								if (!VoteKick.cRankWhitelist.Contains(target.GetRankName().ToLower()))
								{
									caller = player;
									if (!onCooldown)
									{
										b.CallRpcAddElement($"<color=#10EE00>{caller.Name}</color> has initiated a vote kick against <color=#FF0000>{target.Name}</color>. To vote, press [`] or [~] and type <color=#10EE00>.voteyes</color> or <color=#FF0000>.voteno</color>.", 10, false);
										rMessage = "Vote has been started.";
										isVoting = true;

										Timing.In(x =>
										{
											EndVote();
										}, VoteKick.cTimeout);
									}
									else
									{
										rMessage = $"You cannot start a vote kick for another {vCooldown} second{(vCooldown == 1 ? "" : "s")}.";
									}
								}
								else
								{
									rMessage = "You cannot vote kick this player.";
								}
							}
							else
							{
								rMessage = "You cannot initiate a vote on yourself.";
							}
						}
						else
						{
							rMessage = "Invalid player.";
						}
					}
				}
				else
				{
					rMessage = "There is already a vote in progress.";
				}
			}
			else
			{
				rMessage = "Error: round must be started to initiate vote kick.";
			}
			return rMessage;
		}

		public void CancelVote()
		{
			EndVote(true);
		}

		public string VoteYes(Player player)
		{
			string rMessage = "";
			if (isVoting)
			{
				if (!vPlayers.Contains(player.SteamId))
				{
					if (player.SteamId != target.SteamId)
					{
						vYes++;
						rMessage = "Vote registered.";
						vPlayers.Add(player.SteamId);
						if (vYes + vNo >= vPlayerCount) EndVote();
					}
					else
					{
						rMessage = $"Error: {(player.SteamId == target.SteamId ? "you cannot vote for yourself." : "you cannot vote on your own vote.")}";
					}
				}
				else
				{
					rMessage = "You have already voted on this poll.";
				}
			}
			else
			{
				rMessage = "There is not an active vote.";
			}
			return rMessage;
		}

		public string VoteNo(Player player)
		{
			string rMessage = "";
			if (isVoting)
			{
				if (!vPlayers.Contains(player.SteamId))
				{
					vNo++;
					rMessage = "Vote registered.";
					vPlayers.Add(player.SteamId);
					if (vYes + vNo >= vPlayerCount) EndVote();
				}
				else
				{
					rMessage = "You have already voted on this poll.";
				}
			}
			else
			{
				rMessage = "There is not an active vote.";
			}
			return rMessage;
		}

		public void EndVote(bool cancelled = false)
		{
			if (isVoting)
			{
				isVoting = false;

				VoteKick.instance.Info(vYes.ToString());
				VoteKick.instance.Info(vNo.ToString());
				VoteKick.instance.Info((vYes / (vYes + vNo) * 100f).ToString());

				if (cancelled)
				{
					b.CallRpcAddElement($"The vote kick against <color=#00f9ff>{target.Name}</color> has been cancelled.", 10, false);
				}
				else if (vYes + vNo < VoteKick.cMinVotes)
				{
					b.CallRpcAddElement($"There were not enough votes on the poll to kick <color=#FF0000>{target.Name}</color>.", 10, false);
				}
				else if (vYes / (vYes + vNo) * 100f >= VoteKick.cPassPercent)
				{
					b.CallRpcAddElement($"The vote to kick <color=#FF0000>{target.Name}</color> has passed. Kicking player...", 10, false);
					target.Disconnect($"You have been vote kicked by {caller.Name}.");
					if (VoteKick.cCooldownOnPass) StartCooldown();
				}
				else
				{
					b.CallRpcAddElement($"The vote to kick <color=#FF0000>{target.Name}</color> has failed.", 10, false);
					if (VoteKick.cCooldownOnFail) StartCooldown();
				}

				vYes = 0;
				vNo = 0;
				vPlayers.Clear();
			}
		}
	}
}
