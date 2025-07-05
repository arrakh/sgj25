/*
 *   Copyright (c) 2025 
 *   All rights reserved.
 */
namespace Prototype
{
    public class LibraryEntry
    {
        public int Amount => amount;
            
        public readonly CardInstance instance;
        private int amount;

        public LibraryEntry(LibraryData data, GameDatabase gameDb)
        {
            var cardData = gameDb.GetCard(data.id);
            amount = data.amount;
            instance = new CardInstance(cardData);
        }

        public LibraryEntry(CardInstance instance, int amount)
        {
            this.instance = instance;
            this.amount = amount;
        }

        public void SetAmount(int newAmount)
        {
            amount = newAmount;
        }
    }
}