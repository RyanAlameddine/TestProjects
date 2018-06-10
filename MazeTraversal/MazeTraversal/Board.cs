using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeTraversal
{
    public class Board
    {
        public byte[,] spots;
        public List<Vector2> overLaySpots = new List<Vector2>();
        public int width;
        public int height;
        public int size;

        public Board(int width, int height, int size)
        {
            this.width = width;
            this.height = height;
            this.size = size;
            spots = new byte[width, height];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    spots[i, j] = 0;
                }
            }
        }

        public Vector2 findStart()
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(spots[x, y] == 2)
                    {
                        return new Vector2(x, y);
                    } 
                }
            }
            return new Vector2(-1, -1);
        }

        public Vector2 findEnd()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (spots[x, y] == 3)
                    {
                        return new Vector2(x, y);
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        List<(Vector2, float, Vector2)> getConnections(Vector2 point)
        {
            List<(Vector2, float, Vector2)> availableConnections = new List<(Vector2, float, Vector2)>();

            for(int x = (int)point.X - 1; x <= point.X + 1; x++)
            {
                if (x < 0 || x >= width) { continue; }
                for (int y = (int)point.Y - 1; y <= point.Y + 1; y++)
                {
                    if (y < 0 || y >= height) { continue; }
                    if (x == point.X && y == point.Y) { continue; }

                    availableConnections.Add((point, 0, new Vector2(x, y)));
                }
            }

            return availableConnections;
        }

        public void AStar(Vector2 start, Vector2 end)
        {
            Dictionary<Vector2, (float, Vector2)> costs = new Dictionary<Vector2, (float, Vector2)>();

            costs.Add(start, (0, new Vector2(-1, -1)));

            List<(Vector2, float, Vector2)> currentConnections = new List<(Vector2, float, Vector2)>();

            currentConnections.AddRange(getConnections(start));

            while (currentConnections.Count > 0)
            {
                (Vector2, float, Vector2) lowestEdge = (new Vector2(-1, -1), float.MaxValue, new Vector2(-1, -1));
                float lowestCost = float.MaxValue;

                for (int c = 0; c < currentConnections.Count; c++)
                {
                    (Vector2, float, Vector2) connection = currentConnections[c];
                    float cost = connection.Item2 + costs[connection.Item1].Item1 + ManhattanDistance(connection.Item3, end);
                    if (cost < lowestCost)
                    {
                        if (costs.ContainsKey(connection.Item3))
                        {
                            currentConnections.Remove(connection);
                            c--;
                            continue;

                        }
                        lowestEdge = connection;
                        lowestCost = cost;
                    }
                }

                currentConnections.Remove(lowestEdge);
                costs.Add(lowestEdge.Item3, (lowestCost, lowestEdge.Item1));
                currentConnections.AddRange(getConnections(lowestEdge.Item3));
            }
            Vector2 currentSpot = costs[end].Item2;
            while(currentSpot != start)
            {
                overLaySpots.Add(currentSpot);
                currentSpot = costs[currentSpot].Item2;
            }
        }

        public float ManhattanDistance(Vector2 start, Vector2 target)
        {
            float distance = Math.Abs(target.X - start.X) + Math.Abs(target.Y - target.Y);
            return distance;
        }
    }
}
