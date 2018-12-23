# VoteKick

A plugin that allows players to vote to kick other players.

### A plugin for SCP: Secret Laboratory

# Installation

**[Smod2](https://github.com/Grover-c13/Smod2) must be installed for this to work.**

Place the "VoteKick.dll" file in your sm_plugins folder.

# Features
- Allows players to call vote kicks against other players
- Broadcasts status messages to the top of players' screens
- Customizable vote end conditions

**These commands are run through the player console, accessable by pressing `.**

| Command        |  Parameter          | Description  |
| :-------------: | :-----:|:-----|
| VOTEKICK | PLAYER NAME | Initiate a vote to kick the specified player.  |
| VOTEYES | | Vote yes on the current vote. |
| VOTENO | | Vote no on the current vote. |

| Config        |  Default          | Description  |
| :-------------: | :-----:|:-----|
| votekick_minimum_votes | 2 | The minimum number of votes for a vote to pass.  |
| votekick_cooldown | 120 | How many seconds each user must wait before initiating a second vote. |
| votekick_timeout | 30 | How many seconds until a vote will automatically end. |
