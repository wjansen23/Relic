
namespace RPG.Stats
{
    /// <summary>
    ///  Stat types that are in the game
    /// </summary>

    public enum StatType
    {
        Health,             //How much physical damage can something take
        Energy,             //How many feats can a something do
        Magic,              //How many spells can something case
        LevelXp,            //Max XP for a level
        XpReward,           //How much XP something rewards on death
        PhysicalDamage,     //How much base physical damage something does
        MagicalDamage,      //How much base magical damage something does
        MagicRegen,         //how much magic (In percent) does something regenerate
        HealthRegen         //How much health (In percent) does something regenerate
    }
}
