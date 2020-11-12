# MAG Test

This is the code test for the interview process at **MAG**.

# Content
- Main menu, level selection (map) menu, match-3 linker with 5 tile colours
- Objective based levels
-- At a bare minimum, score X points (first star) 
-- Pop X tiles of a given colour
-- Make a chain of a certain amount of tiles, X times
- A penalty Grey tile that removes score

# Level Editor

- Very limited and version specific UI editor
> Open the "Level_Editor" scene, then run
- Allows the creation of new levels
- Allows the modification of existing levels
> Either input the level index in the box, or tap the next button (top right) until end level
- Input for the star scores, the tiles in the level, the board size, the maximum move amount, and up to two objectives
- Some basic checks are made (minimum board size, minimum moves) but it is easy to create a broken level
- Possible to test a level before committing to save it, it will remember the changes made until another level is loaded

# Known issues
- The board is supposed to check if there are any possible match, and reshuffle if there is not. It happens that sometimes it is not properly detected, and the player has to exit the level manually.
- Very occasionally, some tiles don't move down

# What's missing?
- Well, A LOT, but here's what I planned to have and didn't add in the end
- Popups for the start of a level (showing the objectives, and player highscore if previously completed)
- Proper level complete popup with ceremony
- Toast messages when reshuffling the board, or making long chains
- Show a possible chain if the player is inactive for a while
- Feedback on the map nodes
- Transition between scenes
- Bonus round when level is completed and the player has extra moves
- ...and more