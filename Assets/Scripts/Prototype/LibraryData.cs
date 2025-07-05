using System;

namespace Prototype
{
    [Serializable]
    public struct LibraryData
    {
        public string id;
        public int amount;

        public LibraryData(string id, int amount)
        {
            this.id = id;
            this.amount = amount;
        }
    }
}