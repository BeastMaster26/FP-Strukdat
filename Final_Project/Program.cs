using System;
using System.Collections.Generic;

class Todo
{
    public string Description { get; set; }
    public bool IsDone { get; set; }
}

// Node class buat linked list
class Node
{
    public Todo Data;
    public Node Next;

    public Node(Todo data)
    {
        Data = data;
        Next = null;
    }
}

// linked list 
class ManualTodoList
{
    private Node head;
    private int count;

    public ManualTodoList()
    {
        head = null;
        count = 0;
    }

    public int Count { get { return count; } }

    public void Add(Todo item)
    {
        Node newNode = new Node(item);
        if (head == null)
        {
            head = newNode;
        }
        else
        {
            Node current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
        count++;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (index == 0)
        {
            head = head.Next;
        }
        else
        {
            Node current = head;
            for (int i = 0; i < index - 1; i++)
            {
                current = current.Next;
            }
            current.Next = current.Next.Next;
        }
        count--;
    }

    // Get Todo memakai index index
    public Todo GetTodoByIndex(int index)
    {
        if (index < 0 || index >= count)
        {
            throw new ArgumentOutOfRangeException();
        }
        Node current = head;
        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }
        return current.Data;
    }

}

// Stack untuk undo remove
class ManualTodoStack
{
    private Node top;
    private int count;

    public ManualTodoStack()
    {
        top = null;
        count = 0;
    }

    public int Count { get { return count; } }

    public void Push(Todo item)
    {
        Node newNode = new Node(item);
        newNode.Next = top;
        top = newNode;
        count++;
    }

    public Todo Pop()
    {
        if (top == null)
        {
            throw new InvalidOperationException("Stack is empty.");
        }
        Todo item = top.Data;
        top = top.Next;
        count--;
        return item;
    }
}

// Main class
class TodoListManager
{
    static ManualTodoList todos = new ManualTodoList();//inisiasi
    static ManualTodoStack undoStack = new ManualTodoStack();

    static void Main(string[] args)
    {
        LoadTodos();
        while (true)
        {
            Console.WriteLine("\nTodo List Manager");
            Console.WriteLine("1. View Todos");
            Console.WriteLine("2. Add Todo");
            Console.WriteLine("3. Remove Todo");
            Console.WriteLine("4. Mark Todo as Done");
            Console.WriteLine("5. Undo Last Remove");
            Console.WriteLine("6. Save and Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewTodos();
                    break;
                case "2":
                    AddTodo();
                    break;
                case "3":
                    RemoveTodo();
                    break;
                case "4":
                    MarkTodoAsDone();
                    break;
                case "5":
                    UndoRemove();
                    break;
                case "6":
                    SaveTodos();
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void ViewTodos()
    {
        Console.WriteLine("\nTodo List:");
        for (int i = 0; i < todos.Count; i++)
        {
            string status = todos.GetTodoByIndex(i).IsDone ? "[x]" : "[ ]";
            Console.WriteLine($"{i + 1}. {status} {todos.GetTodoByIndex(i).Description}");
        }
    }

    static void AddTodo()
    {
        Console.Write("Enter new todo: ");
        string description = Console.ReadLine();
        todos.Add(new Todo { Description = description });
    }

    static void RemoveTodo()
    {
        Console.Write("Enter the number of the todo to remove: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= todos.Count)
        {
            Todo removedTodo = todos.GetTodoByIndex(index - 1);
            todos.RemoveAt(index - 1);
            undoStack.Push(removedTodo);
            Console.WriteLine($"Removed todo: {removedTodo.Description}");
        }
        else
        {
            Console.WriteLine("Invalid number. Please try again.");
        }
    }

    static void MarkTodoAsDone()
    {
        Console.Write("Enter the number of the todo to mark as done: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= todos.Count)
        {
            todos.GetTodoByIndex(index - 1).IsDone = true;
            Console.WriteLine($"Marked todo as done: {todos.GetTodoByIndex(index - 1).Description}");
        }
        else
        {
            Console.WriteLine("Invalid number. Please try again.");
        }
    }

    static void UndoRemove()
    {
        if (undoStack.Count > 0)
        {
            Todo restoredTodo = undoStack.Pop();
            todos.Add(restoredTodo);
            Console.WriteLine($"Restored todo: {restoredTodo.Description}");
        }
        else
        {
            Console.WriteLine("No actions to undo.");
        }
    }

    static void SaveTodos()
    {
        using (StreamWriter writer = new StreamWriter("todos.txt"))
        {
            for (int i = 0; i < todos.Count; i++)
            {
                writer.WriteLine($"{todos.GetTodoByIndex(i).Description}|{todos.GetTodoByIndex(i).IsDone}");
            }
        }
        Console.WriteLine("Todos saved to todos.txt");
    }

    static void LoadTodos()
    {
        if (File.Exists("todos.txt"))
        {
            using (StreamReader reader = new StreamReader("todos.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('|');
                    todos.Add(new Todo { Description = parts[0], IsDone = bool.Parse(parts[1]) });
                }
            }
        }
    }
}