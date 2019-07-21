using ListProjects;
using System;
using System.Collections;
using System.Collections.Generic;
using Trees;
using UnityEngine;
using UnityEngine.UI;

public class CommandManager
{
    SinglyLinkedList<int> ll;
    RedBlackTree t;
    CircularlyDoublyLinkedList<int> cdll;

    byte selection = 0;

    Dropdown dropdown;
    InputField valueField;
    InputField indexField;

    Action<IEnumerable<int>, List<List<int>>, string> action;
    List<Node> nodes;

    GameObject textPrefab;
    Canvas canvas;

    public CommandManager(Action<IEnumerable<int>, List<List<int>>, string> action, Dropdown dropdown, InputField inputField, InputField inputField2, List<Node> nodes, GameObject textPrefab, Canvas canvas)
    {
        this.nodes = nodes;
        this.action = action;
        this.dropdown = dropdown;
        this.valueField = inputField;
        this.indexField = inputField2;

        this.textPrefab = textPrefab;
        this.canvas = canvas;
    }

    List<Dropdown.OptionData> llOptions = new List<Dropdown.OptionData>() { new Dropdown.OptionData("AddToFront"), new Dropdown.OptionData("AddToBack"), new Dropdown.OptionData("RemoveFromFront"), new Dropdown.OptionData("RemoveFromEnd"), new Dropdown.OptionData("RemoveAt") };
    public void llCreate()
    {
        ll = new SinglyLinkedList<int>();
        ll.OnUpdate += (positions, connections, message) =>
        {
            action(positions, connections, message);
        };
        selection = 0;

        dropdown.ClearOptions();
        dropdown.AddOptions(llOptions);
        DropdownSelect();
    }

    List<Dropdown.OptionData> tOptions = new List<Dropdown.OptionData>() { new Dropdown.OptionData("Insert"), new Dropdown.OptionData("Delete") };
    public void tCreate()
    {
        t = new RedBlackTree();
        t.OnUpdate += (positions, connections, message) =>
        {
            action(positions, connections, message);
        };
        selection = 1;

        dropdown.ClearOptions();
        dropdown.AddOptions(tOptions);
        DropdownSelect();
    }

    List<Dropdown.OptionData> cdllOptions = new List<Dropdown.OptionData>() { new Dropdown.OptionData("AddToFront"), new Dropdown.OptionData("AddToEnd"), new Dropdown.OptionData("AddAt"), new Dropdown.OptionData("RemoveFromFront"), new Dropdown.OptionData("RemoveFromEnd"), new Dropdown.OptionData("RemoveAt") };
    public void cdllCreate()
    {
        cdll = new CircularlyDoublyLinkedList<int>();
        cdll.OnUpdate += (positions, connections, message) =>
        {
            action(positions, connections, message);
        };
        selection = 2;

        dropdown.ClearOptions();
        dropdown.AddOptions(cdllOptions);
        DropdownSelect();
    }

    public void DropdownSelect()
    {
        if(dropdown.options[dropdown.value].text == "RemoveAt")
        {
            valueField.interactable = false;
            indexField.interactable = true;
        }
        else if(dropdown.options[dropdown.value].text == "AddAt")
        {
            valueField.interactable = true;
            indexField.interactable = true;
        }else if(dropdown.options[dropdown.value].text == "RemoveFromFront" || dropdown.options[dropdown.value].text == "RemoveFromEnd")
        {
            valueField.interactable = false;
            indexField.interactable = false;
        }
        else
        {
            valueField.interactable = true;
            indexField.interactable = false;
        }
    }

    public void pulse()
    {
        try
        {
            switch (selection)
            {
                case 0:
                    llRun(dropdown.value);
                    break;
                case 1:
                    tRun(dropdown.value);
                    break;
                case 2:
                    cdllRun(dropdown.value);
                    break;
            }
        } catch(Exception e)
        {
            Text text = NodeManager.CreateScrollingText(textPrefab, canvas.gameObject, "Error: " + e.GetType().ToString());
            text.color = Color.red;
        }

        valueField.text = "";
        indexField.text = "";

        if (valueField.enabled)
        {
            valueField.Select();
            valueField.ActivateInputField();
        }
        else
        {
            indexField.Select();
            indexField.ActivateInputField();
        }
    }
    
    bool checkDuplicate(int value)
    {
        foreach(Node node in nodes)
        {
            if(value == node.item)
            {
                Text text = NodeManager.CreateScrollingText(textPrefab, canvas.gameObject, "Error: Nodes cannot contain the same value");
                text.color = Color.red;
                return true;
            }
        }
        return false;
    }

    public void llRun(int command)
    {
        switch (command)
        {
            case 0:
                if (checkDuplicate(int.Parse(valueField.text))) break;
                ll.AddToFront(int.Parse(valueField.text));
                break;
            case 1:
                if (checkDuplicate(int.Parse(valueField.text))) break;
                ll.AddToEnd(int.Parse(valueField.text));
                break;
            case 2:
                ll.RemoveFromFront();
                break;
            case 3:
                ll.RemoveFromEnd();
                break;
            case 4:
                ll.RemoveAt(int.Parse(indexField.text));
                break;
        }
    }

    public void tRun(int command)
    {
        switch (command)
        {
            case 0:
                if (checkDuplicate(int.Parse(valueField.text))) break;
                t.Insert(new RBNode(int.Parse(valueField.text)));
                break;
            case 1:
                t.Delete(t.IterativeFind(int.Parse(valueField.text)));
                break;
        }
    }

    public void cdllRun(int command)
    {
        switch (command)
        {
            case 0:
                if (checkDuplicate(int.Parse(valueField.text))) break;
                cdll.AddToFront(int.Parse(valueField.text));
                break;
            case 1:
                if (checkDuplicate(int.Parse(valueField.text))) break;
                cdll.AddToEnd(int.Parse(valueField.text));
                break;
            case 2:
                if (checkDuplicate(int.Parse(valueField.text))) break;
                cdll.AddAt(int.Parse(valueField.text), int.Parse(indexField.text));
                break;
            case 3:
                cdll.RemoveFromFront();
                break;
            case 4:
                cdll.RemoveFromEnd();
                break;
            case 5:
                cdll.RemoveAt(int.Parse(indexField.text));
                break;
        }
    }
}
