namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum DifficultyPreset : int {
        Rookie = 0, // osu!: Easy
        Casual = 1, // osu!: Normal
        Pro = 2,    // osu!: Hard
        Expert = 3, // osu!: Expert
        Master = 4  // osu!: Expert+
        //Immortal = 5
    }

    #region Difficuly Levels in other Games

    /* osu difficulties: Easy, Normal, Hard, Expert, Expert+ 
     * https://osu.ppy.sh/help/wiki/Difficulties
     */
    
    /* Guitar Hero difficulties: Beginner, Easy/Casual, Medium, Hard, Expert, Expert+ 
     * https://en.wikipedia.org/wiki/Guitar_Hero_III:_Legends_of_Rock#Gameplay
     * https://guitarhero.fandom.com/wiki/Category:Difficulties
     */
    
    /* StepMania difficulties: Novice, Easy, Medium, Hard, Expert
     * https://www.reddit.com/r/Stepmania/comments/63jc39/remind_me_how_do_you_set_the_default_difficulty/
     */

    #endregion

}
