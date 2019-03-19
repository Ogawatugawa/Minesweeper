using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper
{
    public class Grid : MonoBehaviour
    {
        public GameObject tilePrefab;
        public int width = 10, height = 10;
        public float spacing = .155f;
        [Range(0, 100)]
        public int minesPercentage;


        private Tile[,] tiles;

        public int GetAdjacentMineCount(Tile tile)
        {
            // Set our mine count to 0 to start
            int count = 0;

            // Loop through all adjacent tiles on the X
            for(int x = -1; x <= 1; x++)
            {
                // Loop through all adjacent tiles on the Y
                for (int y = -1; y <= 1; y++)
                {
                    // Calculate which adjacent tile to look at
                    int desiredX = tile.x + x;
                    int desiredY = tile.y + y;

                    // Check if the desired x & y is outside bounds
                    if(desiredX < 0 || desiredX >= width ||
                       desiredY < 0 || desiredY >= height)
                    {
                        // Continue to next element in loop
                        continue;
                    }

                    // Select current tile
                    Tile currentTile = tiles[desiredX, desiredY];
                    
                    // Check if that tile is a mine
                    if (currentTile.isMine)
                    {
                        //increment count by 1
                        count++;
                    }
                }
            }
            // Make sure to return the count to complete the function and retrieve an integer
            return count;
        }

        Tile SpawnTile(Vector3 pos)
        {
            // Clone the tile prefab by instantiating it
            GameObject clone = Instantiate(tilePrefab);

            // Edit its properites by making its transform position the Vector 3 'pos'
            clone.transform.position = pos;
            Tile currentTile = clone.GetComponent<Tile>();

            /* Assigns the isMine variable to the spawned tile to determine if it's a mine or not depending on the percentage we set
            in the dynamic variable 'minesPercentage'that we declared up the top */
            currentTile.isMine = Random.value < minesPercentage*0.01f;
            
            // Return it
            return currentTile;
        }

        private void Start()
        {
            // Makes the script spawn the grid at the beginning of the game
            GenerateTiles();
        }

        private void Update()
        {   
            // Make it so when the mouse left button is clicked then the SelectATile() function runs
            if (Input.GetMouseButtonDown(0))
            {
                SelectATile();
            }
        }

        void GenerateTiles()
        {
            // Create a new 2D array of size width by height
            tiles = new Tile[width, height];
            // Loop through the entire tile list
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create the halfSize variable to use above
                    Vector2 halfSize = new Vector2(width * 0.5f, height * 0.5f);
                    //Pivot tiles around grid
                    Vector2 pos = new Vector2(x - halfSize.x, y - halfSize.y);

                    // The grid spawns .5 off center, so we apply the following offset to center
                    Vector2 offset = new Vector2(.5f, .5f);
                    pos += offset;
                    
                    // Apply spacing
                    pos *= spacing;
                    
                    // Spawn the tile using spawn function that we made earlier
                    Tile tile = SpawnTile(pos);
                    
                    // Attach newly spawned tile to the transform value of self
                    tile.transform.SetParent(transform);
                    
                    //Store its array coordinates within itself for future reference
                    tile.x = x;
                    tile.y = y;

                    // Store tile in array at those prior coordinates
                    tiles[x, y] = tile;
                }
            }
        }
        void SelectATile()
        {
            // Generate a ray from the camera using the mouse position
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Perform a 2D Raycast using the above ray
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);

            // If the mouse hits something (literally: 'if the hit collider isn't detecting nothing')
            if (hit.collider != null)
            {
                // Try getting a Tile component from the object we hit
                Tile hitTile = hit.collider.GetComponent<Tile>();

                // Check if the object we hit is a Tile (literally: 'if hitTile doesn't return nothing')
                if (hitTile != null)
                {
                    //Get a count of mines around the hit tile
                    int adjacentMines = GetAdjacentMineCount(hitTile);

                    // Reveal what that hit tile is
                    hitTile.Reveal(adjacentMines);
                }
            }
        }
    }
}
