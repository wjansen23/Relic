using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetStatAdditiveModifiers(StatType reqStat);
        IEnumerable<float> GetStatPercentageModifiers(StatType reqStat);

    }
}
