using Smod2.API;
using System.Collections.Generic;
using scp4aiur;

namespace VoteKick
{
	partial class EventHandler
	{
		public void LoadConfigs()
		{
			VoteKick.cMinVotes = VoteKick.instance.GetConfigInt("vk_minimum_votes");
			VoteKick.cTimeout = VoteKick.instance.GetConfigFloat("vk_timeout");
			VoteKick.cPassPercent = VoteKick.instance.GetConfigInt("vk_pass_percent");
			VoteKick.cPassCooldown = VoteKick.instance.GetConfigInt("vk_pass_cooldown");
			VoteKick.cFailCooldown = VoteKick.instance.GetConfigInt("vk_fail_cooldown");
			VoteKick.cVoteRanks = new List<string>(VoteKick.instance.GetConfigList("vk_vote_ranks"));
			VoteKick.cImmuneRanks = new List<string>(VoteKick.instance.GetConfigList("vk_immune_ranks"));
		}

		public void StartCooldown(int cooldown)
		{
			onCooldown = true;
			vCooldown = cooldown;

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

			if (VoteKick.cVoteRanks.Count > 0 && !VoteKick.cVoteRanks.Contains(player.GetRankName()))
			{
				return "You are not allowed to initiate a vote kick.";
			}

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
								if (!VoteKick.cImmuneRanks.Contains(target.GetRankName().ToLower()))
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

		public string Vote(bool yes, Player player)
		{
			string rMessage = "";
			if (isVoting)
			{
				if (!vPlayers.Contains(player.SteamId))
				{
					if (player.SteamId != target.SteamId && player.SteamId != caller.SteamId)
					{
						if (yes) vYes++; else vNo++;
						rMessage = "Vote registered.";
						vPlayers.Add(player.SteamId);
						// Adding 2 to account for the initiater and target who can't vote
						if (vYes + vNo + 2 >= vPlayerCount) EndVote();
					}
					else
					{
						rMessage = $"{(player.SteamId == target.SteamId ? "You cannot vote for yourself." : "You cannot vote on your own vote.")}";
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
				else if (vYes + vNo + 2 < VoteKick.cMinVotes)
				{
					b.CallRpcAddElement($"There were not enough votes on the poll to kick <color=#FF0000>{target.Name}</color>.", 10, false);
				}
				else if (vYes / (vYes + vNo) * 100f >= VoteKick.cPassPercent)
				{
					b.CallRpcAddElement($"The vote to kick <color=#FF0000>{target.Name}</color> has passed. Kicking player...", 10, false);
					target.Disconnect($"You have been vote kicked by {caller.Name}.");
					StartCooldown(VoteKick.cPassCooldown);
				}
				else
				{
					b.CallRpcAddElement($"The vote to kick <color=#FF0000>{target.Name}</color> has failed.", 10, false);
					StartCooldown(VoteKick.cFailCooldown);
				}

				vYes = 0;
				vNo = 0;
				vPlayers.Clear();
			}
		}
	}
}
