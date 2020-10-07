using System;
using System.Collections.Generic;

namespace Adventure
{
    public class GameManager
    {
        //This class handles all logic in the game.

        public Board gameBoard { get; private set; }
        public Menu infoBox;
        public Menu statusBox;
        List<GameObject> sceneObjects;
        Player player;

        public List<GameObject> SceneObjects {
            get { return sceneObjects; }
            set { sceneObjects = value; }
        }

        public GameManager()
        {
            //This constructor IS the game, and will eventually end up in a loop that makes it all come together.
            //First, we declare our variables. We need two separate Menus for the two boxes on the right, and one main board to play on.
            //We also instantiate our player.
            Console.CursorVisible = false;
            gameBoard = new Board(80, 30);
            infoBox = new Menu(30, 20, 80, 0, "HISTORY", true);
            statusBox = new Menu(30, 10, 80, infoBox.sizeY, "STATUS");
            player = new Player(this);

            //Next, we spawn all enemies and items.
            SceneObjects = new List<GameObject>
            {
                new HealthPotion(),
                new LargeHealthPotion(),
                new SmallHealthPotion(),
                new AccuracyPotion(),
                new StrengthPotion(),
                new Ghost(),
                new Ghost(),
                new Skeleton(),
                new Skeleton(),
                new Skeleton(),
                new Skeleton(),
                new Zombie(),
                new Zombie(),
                new Zombie(),
                new Helmet(),
                new Sword_Iron(),
                new Sword_Steel(),
                new Leggings(),
                new Chestplate(),
                player
            };

            //Loop through the list, assign this game manager to each, randomize their position and draw them onto the screen.
            foreach (var GameObject in SceneObjects)
            {
                GameObject.gameManager = this;
                GameObject.RandomizePosition();
                GameObject.DrawObject();
            }

            //This here loops indefinitely. We read player input, then redraw all characters in the scene.
            while (true)
            {
                player.ReadInput();
                foreach (var GameObject in SceneObjects)
                {
                    GameObject.DrawObject();
                }
            }
        }
    }
}
