# VoteKick

A plugin that allows players to vote to kick other players.

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

| Config        | Type | Default          | Description  |
| :-------------: |:----:|:-----:|:-----|
| vk_minimum_votes | Integer | 2 | The minimum number of votes for a vote to pass.  |
| vk_timeout | Float | 30 | How many seconds until a vote will automatically end. |
| vk_pass_percent | Integer | 60 | What percent of the votes must be in favor of the kick for it to go through. |
| vk_pass_cooldown | Integer | 300 | The number of seconds before another vote can be initiated if the previous vote passed. |
| vk_fail_cooldown | Integer | 300 | The number of seconds before another vote can be initiated if the previous vote failed. |
| vk_vote_ranks | List | | A list of ranks that can initiate a vote kick. If left blank it will allow all ranks. |
| vk_immune_ranks | List | moderator, admin, owner | A list of ranks that cannot be kicked. |
| vk_vote_level | Integer | 1 | [PlayerXP](https://github.com/Cyanox62/PlayerXP) integration, sets a minimum level that players must be in order to start a vote kick. |
