namespace Adventure
{
    public struct Stats
    {
        public string Name;
        public string Symbol;
        public int Health;
        public int Strength;
        public int Accuracy;

        public Stats(string name, string symbol, int health, int strength, int accuracy)
        {
            Name = name;
            Symbol = symbol;
            Health = health;
            Strength = strength;
            Accuracy = accuracy;
        }
        public Stats(int health, int strength, int accuracy)
        {
            Name = "Unnamed";
            Symbol = "?";
            Health = health;
            Strength = strength;
            Accuracy = accuracy;
        }

        public Stats Add(Stats opponent)
        {
            Health += opponent.Health;
            Strength += opponent.Strength;
            Accuracy += opponent.Accuracy;

            return this;
        }
        public Stats Subtract(Stats opponent)
        {
            Health -= opponent.Health;
            Strength -= opponent.Strength;
            Accuracy -= opponent.Accuracy;

            return this;
        }
    }
}
