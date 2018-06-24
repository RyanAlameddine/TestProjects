using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeTraversal
{
    public class UnionFind
    {
        int[] spots;

        public UnionFind(int length)
        {
            spots = new int[length];
            for(int i = 0; i < length; i++)
            {
                spots[i] = -1;
            }
        }

        int getRoot(int index)
        {
            int currentIndex = index;
            while (spots[currentIndex] >= 0)
            {
                currentIndex = spots[currentIndex];
            }
            return currentIndex;
        }

        bool isRoot(int index)
        {
            return spots[index] < 0;
        }

        public void Union(int index1, int index2)
        {
            int root1 = getRoot(index1);
            int height1 = spots[root1] * -1;

            int root2 = getRoot(index2);
            int height2 = spots[root2] * -1;

            if (height1 <= height2)
            {
                if (isRoot(index1))
                {
                    spots[index1] = index2;
                }
                else
                {
                    spots[root1] = index2;
                }
                spots[root2] -= height1;
            }
            else
            {
                if (isRoot(index2))
                {
                    spots[index2] = index1;
                }
                else
                {
                    spots[root2] = index1;
                }
                spots[root1] -= height2;
            }
            Find(index1, index2);
        }

        public int Find(int index1, int index2)
        {
            int root = getRoot(index1);
            int currentIndex1 = index1;
            while(spots[currentIndex1] >= 0)
            {
                int currentIndex = currentIndex1;
                currentIndex1 = spots[currentIndex1];
                if(currentIndex1 != root)
                {
                    spots[currentIndex] = root;
                }
            }

            root = getRoot(index2);
            int currentIndex2 = index2;
            while (spots[currentIndex2] >= 0)
            {
                int currentIndex = currentIndex2;
                currentIndex2 = spots[currentIndex2];
                if (currentIndex2 != root)
                {
                    spots[currentIndex] = root;
                }
            }

            if (currentIndex1 == currentIndex2)
            {
                return spots[currentIndex1] * -1;
            }
            else
            {
                return -1;
            }
        }

        public void Print()
        {
            String line1 = "";
            String line2 = "";
            for(int i = 0; i < spots.Length; i++)
            {
                if (spots[i] < 0) line1 += " ";
                line1 += i + " ";
                
                line2 += spots[i] + " ";
            }
            Console.WriteLine(line1);
            Console.WriteLine(line2);
        }
    }
}
