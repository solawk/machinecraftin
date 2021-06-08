# Friday Night MachineCraftin'
Friday Night Funkin' recreated as a MachineCraft script
Original game by ninja_muffin99
MachineCraft by G2CREW

**Parser** as an online page: https://solawk.github.io/machinecraftin/

# How to install
1. Find the **UserData** folder in your copy of Machinecraft.
2. Download the repository as a ZIP archive.
3. Extract the **UserData** folder and merge it with your own.
4. Access *_UserData/_scripts/FNF.cs* and change the name to your in-game name on line 165.
You can now launch the machine but there's no songs for it to play.

# How to add songs
1. Find the .json **chart** file in *assets/data/<song_name>* in your copy of Friday Night Funkin' or its mods.
2. Copy its contents and paste them into the first textbox of the **parser** page.
3. Submit, the parsed chart will appear in the second textbox. Copy and paste it into the *_UserData/_scripts/FNF.cs* script. I suggest storing the parsed charts so you don't have to parse them every time.
4. Open **Audacity** or any other software/service capable of converting .ogg audio files into .wav.
5. Find the "Inst" and "Voices" audio files of the song in your copy of Friday Night Funkin'. In the vanilla game these are in *assets/music*.
6. Convert those into .wav and put into the *UserData/_sounds/FNF* folder, name them "<song_name>\_Inst" and "<song_name>\_Voices". Song name must match the one in the beginning of the chart.

# How to play
Launch the FNF machine in the practice mode.
Default layout is **A**, **S**, **Up Arrow**, **Right Arrow**.
You can change the layout via script, these are the KeyCodes in the **Keys** array on line 72.
All in all, it's just normal gameplay of the vanilla Friday Night Funkin' (meaning there are no, for example, fire notes in the "vs Tricky" songs).
Don't ever open the Tab/Esc menu when the music is playing, since the game stops and restarts it afterwards.

# FNF machine
You can use the script on any machine, the default one is just a testbed.
Actions triggered by the game are called **LeftL**, **LeftD**, **LeftU**, **LeftR**, **RightL**, **RightD**, **RightU**, **RightR**, **BeatL** and **BeatR**.

# Troubleshooting
## (0, 0) : error : Array index out of range.
Simply re-enter the practice mode. I don't know where this crash has come from and it's getting even worse the more code the script has. It usually works on the 3rd try for me.
## There's a huge lag when the song loads and it messes up the arrows!
The game will always lag when the song is loaded in for the first time since game launch. While the arrows that get caught up in the lag are affected, the script then syncs properly, so it's nothing critical, unless the player has arrows to press right from the song's start. I don't remember a single song that has any.
## "Not enough 'something'!!"
Increase the appropriate 'something'Max integer. Don't increase too much, since it negatively affects the booting up performance.
## The machine is too shaky
It is. You can remove the Basic1 and its rotors if it's critical.
## The song restarts after it's over
Yes. Play the track and leave the game session, it's not restartable yet.
## The track restarts if I miss too much arrows
If you're playing on a low screen resolution, this is the problem. Switch to a higher one.
## I get a white error and the script crashes
Please screenshot it and send to me via Discord (Solawk#9890).
## I think Heavy is dead
**Heavy is dead?**
