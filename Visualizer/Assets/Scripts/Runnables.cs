using System;
using System.Collections;
using System.Collections.Generic;
using Trees;
using UnityEngine;

public class Runnables
{
    Action<IEnumerable<int>, List<List<int>>, string> action;

    public Runnables(Action<IEnumerable<int>, List<List<int>>, string> action)
    {
        this.action = action;
    }

    public IEnumerator Tree()
    {
        RedBlackTree tree = new RedBlackTree();
        tree.OnUpdate += (positions, connections, message) =>
        {
            action(positions, connections, message);
        };

        int i = 100000000;
        while (true)
        {
            tree.Insert(new RBNode(i * 8));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Insert(new RBNode(i * 103));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Insert(new RBNode(i * 5006));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Insert(new RBNode(i * 12));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Delete(tree.Search(i * 103));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Insert(new RBNode(i * 3094));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Insert(new RBNode(i * 2092));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Delete(tree.Search(i * 3094));
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            tree.Delete(tree.Search(i * 2092));
            i += 1;
        }
    }

    public IEnumerator DoublyLinked()
    {
        ListProjects.CircularlyDoublyLinkedList<int> list = new ListProjects.CircularlyDoublyLinkedList<int>();
        System.Random random = new System.Random();
        list.OnUpdate += (positions, connections, message) =>
        {
            action(positions, connections, message);
        };

        while (true)
        {
            list.AddToFront(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToEnd(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToEnd(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddAt(3, random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToFront(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveAt(4);
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveAt(2);
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToEnd(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveFromFront();
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveFromEnd();
        }
    }

    public IEnumerator SinglyLinked()
    {

        ListProjects.SinglyLinkedList<int> list = new ListProjects.SinglyLinkedList<int>();
        System.Random random = new System.Random();
        list.OnUpdate += (positions, connections, message) =>
        {
            action(positions, connections, message);
        };

        while (true)
        {
            list.AddToFront(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToEnd(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToEnd(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToFront(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToFront(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveAt(4);
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveAt(2);
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.AddToEnd(random.Next());
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveFromFront();
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
            list.RemoveFromEnd();
            yield return new WaitUntil(() => NodeManager.pulsed);
            NodeManager.pulsed = false;
        }

    }
}
