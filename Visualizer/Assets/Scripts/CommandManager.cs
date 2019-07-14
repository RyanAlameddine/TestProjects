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

    [SerializeField]
    GameObject textPrefab; FixedJoint THESE TWO VARIABLES
    [SerializeField]
    Canvas canvas;

    public CommandManager(Action<IEnumerable<int>, List<List<int>>, string> action, Dropdown dropdown, InputField inputField, InputField inputField2, List<Node> nodes)
    {
        this.nodes = nodes;
        this.action = action;
        this.dropdown = dropdown;
        this.valueField = inputField;
        this.indexField = inputField2;
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
        }
        else
        {
            valueField.interactable = true;
            indexField.interactable = false;
        }
    }

    public void pulse()
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

        valueField.text = "";
        indexField.text = "";
        valueField.Select();
        valueField.ActivateInputField();
    }
    
    bool checkDuplicate(int value)
    {
        foreach(Node node in nodes)
        {
            if(value == node.item)
            {
                NodeManager.CreateScrollingText(textPrefab, canvas.gameObject, "ERROR: DUPLICATE ITEM FOUND");
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
