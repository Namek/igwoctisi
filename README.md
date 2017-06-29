# IGWOCTISI

**IGWOCTISI** is a pure multiplayer turn-based strategy game developed as a team project on the last term of my bachelor grade university education. Its rules are more or less similar to those of the popular browser game [Warlight](http://warlight.net/) and the classical board game [Risk](http://en.wikipedia.org/wiki/Risk_(game)). The setting is different however as all the action takes place in space. The aim of the description is to be a brief retrospection of the project.

**See also:**
* [IGWOCTISI Server](https://github.com/modrzew/igwoctisi-server)
* [IGWOCTISI Web Interface](https://github.com/modrzew/igwoctisi-web)
* [Lambda Cipher Genesis, codename IGWOCTISI 2: The revenge of IGWOCTISI](https://github.com/Namek/lets-code-game)

## Challenges

The major challenges which the team encountered during the 3 month development phase consisted of:

*   **Implementing multiplayer functionality.** The network communication was to implemented in a multithreaded way, which forced the team to carefully design the specifics so as to avoid many troublesome and difficult to detect errors such as race conditions and deadlocks. Also, there were many various so-called "rainy day" scenarios which needed to be handled in order to make the game stable enough for the player to actually be able to finish a single round.
*   **Devising an effective workflow, communication and documentation methods.** There where many opportunities for disagreements and misunderstandings starting with the game client architecture, through server message specification and ending with such down-to-earth issues as code organization and coding conventions.
*   **Integration.** The client, server and the website were developed using different technologies and essentially running on different machines.

## Technologies and tools

The client was developed using C# along with XNA and the Nuclex Framework (specifically the GUI and particle modules). The server is a Python script using SQL Alchemy package. The website consisting the ladder and game history was using the Kohana 3.3.0 framework. Most of the code was written using Visual Studio. Two version control systems were used: Mercurial and Team Foundation Server.

## The good

*   The team consisted of programmers who started programming more than 6 years ago and apart from general programming experience all had worked with agile software development methodologies.
*   The team members already had experience using most of required technologies.
*   All in all, the project hand very clear vision and goals and went according to schedule, with almost no feature creep (apart from a few visual enhancements).

## The bad

*   Obviously, not all middleware was perfect. Some of it required serious hacking in order to make it fit out needs.
*   Some features were not really needed and were therefore ignored, but remained until the end of the project, haunting the backlog.
*   The project management software was cumbersome and required some additional effort in order to learn the workflow in the beginning, but in the end, it worked and didn't make it (much) more difficult to accomplish the tasks.

## Screenshots

| ![Planetary systems](/_screenshots/igwoctisi-7-planetary-systems.png?raw=true "Planetary systems") |
|:---:|
| Planetary systems |

| ![Logging screen](/_screenshots/igwoctisi-1-logowanie.png?raw=true "Logging screen") | ![Game lobby](/_screenshots/igwoctisi-2-game-lobby-chat.png?raw=true "Game lobby") | ![In-game chat](/_screenshots/igwoctisi-3-in-game-chat-player-list.png?raw=true "In-game chat") |
|:---:|:---:|:---:|
| Logging screen | Game lobby | In-game chat |

| ![Deploying fleets](/_screenshots/igwoctisi-4-deploy-animation.png?raw=true "Deploying fleets") | ![Moving fleets](/_screenshots/igwoctisi-5-move-fleets-between-planets.png?raw=true "Moving fleets") | ![Commands](/_screenshots/igwoctisi-6-commanding-moves-and-attacks.png?raw=true "Commands") |
|:---:|:---:|:---:|
| Deploying fleets | Moving fleets | Commands |


| ![Game statistics](/_screenshots/igwoctisi-8-game-statistics.png?raw=true "Game statistics") | ![Player ranking](/_screenshots/igwoctisi-9-player-ranking.png?raw=true "Player ranking") | ![Player profile](/_screenshots/igwoctisi-10-player-profile.png?raw=true "Player profile") |
|:---:|:---:|:---:|
| Game statistics | Player ranking | Player profile |

