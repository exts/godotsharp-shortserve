# Changelog
All notable changes to this project will be documented in this file.

## [1.2.0] - 2019 - 12 - 18
### Changed
- Added a HTML5 check to force window maximize
- Removed splash from start up because issue with 3.2 
- Refactored game over code so I can force gameover when quitting
- Made code work with 3.2, namespaces such as Godot.Dictionary & Godot.Array have changed, etc... 

## [1.1.0] - 2018 - 08 - 14
### Changed
- Made plates deselect when the game ends
- Made order items spawn items based on the plate pool of items 50% more often
- Made item spawn 60% of the time be based off items from one of your available orders (that feeling of bingo)

### FIXED
- Fixed highscore not properly updating its value even though it saves the latest highscore making highscores more accurate

