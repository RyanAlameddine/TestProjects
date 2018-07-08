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
        public Vector2 startPoint = Vector2.Zero;
        public Vector2 endPoint = Vector2.Zero;
        Random rand = new Random(1);

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

        List<(Vector2, float, Vector2)> getConnections(Vector2 point)
        {
            List<(Vector2, float, Vector2)> availableConnections = new List<(Vector2, float, Vector2)>();

            int x = (int)point.X - 1;
            int y = (int)point.Y;
            if (checkConnection(x, y)) { availableConnections.Add((point, 0, new Vector2(x, y))); }
            x = (int)point.X + 1;
            y = (int)point.Y;
            if (checkConnection(x, y)) { availableConnections.Add((point, 0, new Vector2(x, y))); }
            x = (int)point.X;
            y = (int)point.Y + 1;
            if (checkConnection(x, y)) { availableConnections.Add((point, 0, new Vector2(x, y))); }
            x = (int)point.X;
            y = (int)point.Y - 1;
            if (checkConnection(x, y)) { availableConnections.Add((point, 0, new Vector2(x, y))); }

            return availableConnections;
        }

        bool checkConnection(int x, int y)
        {
            if (x < 0 || x >= width) { return false; }
            if (y < 0 || y >= height) { return false; }
            if(spots[x, y] == 1) { return false; }
            return true;
        }

        public void AStar()
        {
            Dictionary<Vector2, (float, Vector2)> costs = new Dictionary<Vector2, (float, Vector2)>();

            costs.Add(startPoint, (ManhattanDistance(startPoint, endPoint), new Vector2(-1, -1)));

            List<(Vector2, float, Vector2)> currentConnections = new List<(Vector2, float, Vector2)>();

            currentConnections.AddRange(getConnections(startPoint));

            while (currentConnections.Count > 0)
            {
                (Vector2, float, Vector2) lowestEdge = (new Vector2(-1, -1), float.MaxValue, new Vector2(-1, -1));
                float lowestCost = float.MaxValue;

                for (int c = 0; c < currentConnections.Count; c++)
                {
                    (Vector2, float, Vector2) connection = currentConnections[c];
                    float cost = connection.Item2 + costs[connection.Item1].Item1 - ManhattanDistance(connection.Item1, endPoint) + ManhattanDistance(connection.Item3, endPoint) + 1;
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
                if(lowestEdge.Item3 == endPoint)
                {
                    break;
                }
                currentConnections.AddRange(getConnections(lowestEdge.Item3));
            }
            Vector2 currentSpot = costs[endPoint].Item2;
            while(currentSpot != startPoint)
            {
                overLaySpots.Add(currentSpot);
                currentSpot = costs[currentSpot].Item2;
            }
        }

        public float ManhattanDistance(Vector2 start, Vector2 target)
        {
            float distance = Math.Abs(target.X - start.X) + Math.Abs(target.Y - start.Y);
            return distance;
        }

        public void Prim()
        {
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    spots[x, y] = 1;
                }
            }
            spots[(int)startPoint.X, (int)startPoint.Y] = 2;

            List<Vector2> walls = new List<Vector2>();
            walls.AddRange(getWalls(startPoint, walls));

            while(walls.Count > 0)
            {
                int index = rand.Next(0, walls.Count - 1);
                List<Vector2> connections = new List<Vector2>();
                Vector2 wall = walls[index];
                if (getCell(wall.X - 1, wall.Y) != 1) { connections.Add(new Vector2(wall.X - 1, wall.Y)); }
                if (getCell(wall.X + 1, wall.Y) != 1) { connections.Add(new Vector2(wall.X + 1, wall.Y)); }
                if (getCell(wall.X, wall.Y - 1) != 1) { connections.Add(new Vector2(wall.X, wall.Y - 1)); }
                if (getCell(wall.X, wall.Y + 1) != 1) { connections.Add(new Vector2(wall.X, wall.Y + 1)); }

                if(connections.Count == 1)
                {
                    int xChange = (int)(wall.X - connections[0].X);
                    int yChange = (int)(wall.Y - connections[0].Y);
                    Vector2 open = wall + new Vector2(xChange, yChange);
                    if(open.X < 0 || open.X >= width || open.Y < 0 || open.Y >= height) { walls.Remove(wall); continue; }
                    spots[(int)wall.X, (int)wall.Y] = 0;
                    spots[(int)open.X, (int)open.Y] = 0;
                    walls.AddRange(getWalls(open, walls));
                }
                walls.Remove(wall);
            }
            spots[(int)endPoint.X, (int)endPoint.Y] = 3;
        }

        List<Vector2> getWalls(Vector2 position, List<Vector2> existingwalls)
        {
            List<Vector2> walls = new List<Vector2>();
            for (int x = (int)position.X - 1; x <= position.X + 1; x++)
            {
                if (x < 0 || x >= width) { continue; }
                for (int y = (int)position.Y - 1; y <= position.Y + 1; y++)
                {
                    if(x != position.X && y != position.Y) { continue; }
                    if (y < 0 || y >= height) { continue; }
                    if (spots[x, y] != 1) { continue; }
                    if (x == position.X && y == position.Y) { continue; }

                    if (!existingwalls.Contains(new Vector2(x, y)))
                    {
                        walls.Add(new Vector2(x, y));
                    }
                }
            }
            return walls;
        }

        byte getCell(float x, float y)
        {
            if (x < 0 || x >= width) { return 1; }
            if (y < 0 || y >= height) { return 1; }
            return spots[(int)x, (int)y];
        }

        public void UnionFindGen()
        {
            float xOff = startPoint.X % 2;
            float yOff = startPoint.Y % 2;
            int length = 0;
            int[,] indicies = new int[width, height];
            List<int> unconnected = new List<int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    spots[x, y] = 1;
                    if (x % 2 == xOff && y % 2 == yOff)
                    {
                        spots[x, y] = 0;
                        indicies[x, y] = length;
                        unconnected.Add(length);
                        length++;
                    }
                }
            }
            spots[(int)startPoint.X, (int)startPoint.Y] = 2;

            UnionFind unionFind = new UnionFind(length);
            bool finished = false;
            while (!finished)
            {
                for (int i = 0; i < length*2; i++)
                {
                    int randIndex = unconnected[rand.Next(0, unconnected.Count)];
                    (int, int) pos = getPos(indicies, randIndex);
                    (int, int) targetPos;
                    do
                    {
                        int randDir = rand.Next(0, 4);
                        if (randDir == 0)
                        {
                            targetPos = (pos.Item1 + 1, pos.Item2);
                        }
                        else if (randDir == 1)
                        {
                            targetPos = (pos.Item1 - 1, pos.Item2);
                        }
                        else if (randDir == 2)
                        {
                            targetPos = (pos.Item1, pos.Item2 - 1);
                        }
                        else
                        {
                            targetPos = (pos.Item1, pos.Item2 + 1);
                        }
                    } while (targetPos.Item1 < 0 || targetPos.Item1 >= width || targetPos.Item2 < 0 || targetPos.Item2 >= height);
                    (int, int) endPoint = (pos.Item1 + 2 * (targetPos.Item1 - pos.Item1), pos.Item2 + 2 * (targetPos.Item2 - pos.Item2));
                    int endIndex = indicies[endPoint.Item1, endPoint.Item2];

                    if (unionFind.Find(randIndex, endIndex) != -1)
                    {
                        continue;
                    }
                    spots[targetPos.Item1, targetPos.Item2] = 0;
                    unionFind.Union(randIndex, endIndex);
                }

                finished = true;
                //check if done
                for(int i = 0; i < length/2 + 1; i++)
                {
                    if(unionFind.Find(i, length-i-1) == -1)
                    {
                        finished = false;
                        break;
                    }
                }
                
            }
        }

        (int, int) getPos(int[,] indicies, int index)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(indicies[x, y] == index)
                    {
                        return (x, y);
                    }
                }
            }
            return (-1, -1);
        }
    }
}
