using System;

namespace Adventure
{
    public abstract class Character : GameObject
    {
        //Base class for each creature in the game, including the player.

        protected int[,] Move(int moveX, int moveY)
        {
            //This method takes in our movement and returns our new position.
            //When we move, we need to ensure we are not moving into a wall. We check with the board.
            if (gameManager.gameBoard.isWall(coordinates.x + moveX, coordinates.y + moveY) != true)
            {
                coordinates.x += moveX;
                coordinates.y += moveY;
            }
            return new int[coordinates.y, coordinates.x];
        }

        public void Attack(Character opponent)
        {
            //This is always called by an opponent.
            if (random.Next(1, 100) <= stats.Accuracy)
            {
                opponent.stats.Subtract(new Stats(stats.Strength, 0, 0));

                gameManager.infoBox.Write($"{stats.Name} hit {opponent.stats.Name} for {stats.Strength} damage!");
            }
            if (opponent.stats.Health <= 0)
            {
                opponent.Death();
            }
        }

        public virtual void Death()
        {
            //Check if we are in the list, remove all memories of us.
            if (gameManager.SceneObjects.Contains(this))
            {
                gameManager.SceneObjects.Remove(this);
                UndrawObject();
                gameManager.infoBox.Write($"{stats.Name} died!");
            }
        }
    }

}
