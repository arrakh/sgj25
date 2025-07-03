using System.Collections.Generic;

namespace Prototype
{
    public class GameResult
    {
        public readonly bool win;
        public readonly IReadOnlyList<CardInstance> enemyDefeated;
        public readonly IReadOnlyList<CardInstance> itemsLeft;

        public GameResult(bool win, List<CardInstance> enemyDefeated, List<CardInstance> itemsLeft)
        {
            this.win = win;
            this.enemyDefeated = enemyDefeated;
            this.itemsLeft = itemsLeft;
        }
    }
}