namespace Adventure
{
    public abstract class Enemy : Character
    {
        public override void Interact(Player player)
        {
            player.Attack(this);

            if (stats.Health > 0) { Attack(player); }
        }
    }

    public class Skeleton : Enemy
    {
        public Skeleton()
        {
            stats = new Stats("Skeleton", "}", 50, 5, 80);
        }
    }
    public class Zombie : Enemy
    {
        public Zombie()
        {
            stats = new Stats("Zombie", "Z", 75, 20, 25);
        }
    }
    public class Ghost : Enemy
    {
        public Ghost()
        {
            stats = new Stats("Ghost", "O", 25, 80, 3);
        }
    }
}
