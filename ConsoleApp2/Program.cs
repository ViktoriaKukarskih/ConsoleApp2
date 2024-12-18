
using System.Collections.Generic;
using System;
using System.Linq;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }

    public Task(int id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
        IsCompleted = false;
    }

    public override string ToString()
    {
        return $"[ID: {Id}] {Title} - {Description} (Завершена: {IsCompleted})";
    }
}

// Класс TaskRepository - отвечает за управление коллекцией задач (SRP)
public class TaskRepository
{
    private readonly List<Task> _tasks;
    private int _nextId;

    public TaskRepository()
    {
        _tasks = new List<Task>();
        _nextId = 1;
    }

    public Task AddTask(string title, string description)
    {
        var newTask = new Task(_nextId++, title, description);
        _tasks.Add(newTask);
        return newTask;
    }

    public List<Task> GetAllTasks()
    {
        return _tasks;
    }

    public Task GetTaskById(int id)
    {
        return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public bool DeleteTask(int id)
    {
        var task = GetTaskById(id);
        if (task != null)
        {
            _tasks.Remove(task);
            return true;
        }
        return false;
    }
}

// VIEW

public interface ITaskView
{
    void DisplayTasks(List<Task> tasks);
    void DisplayMessage(string message);
}

public class ConsoleTaskView : ITaskView
{
    public void DisplayTasks(List<Task> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("Список задач пуст.");
        }
        else
        {
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }
}

// CONTROLLER

public class TaskController
{
    private readonly TaskRepository _repository;
    private readonly ITaskView _view;

    public TaskController(TaskRepository repository, ITaskView view)
    {
        _repository = repository;
        _view = view;
    }

    public void AddTask(string title, string description)
    {
        var task = _repository.AddTask(title, description);
        _view.DisplayMessage($"Задача '{task.Title}' успешно добавлена.");
    }

    public void DisplayTasks()
    {
        var tasks = _repository.GetAllTasks();
        _view.DisplayTasks(tasks);
    }

    public void EditTask(int id, string newTitle, string newDescription)
    {
        var task = _repository.GetTaskById(id);
        if (task != null)
        {
            task.Title = newTitle;
            task.Description = newDescription;
            _view.DisplayMessage("Задача успешно обновлена.");
        }
        else
        {
            _view.DisplayMessage("Задача не найдена.");
        }
    }

    public void DeleteTask(int id)
    {
        if (_repository.DeleteTask(id))
        {
            _view.DisplayMessage("Задача успешно удалена.");
        }
        else
        {
            _view.DisplayMessage("Задача не найдена.");
        }
    }

    public void MarkTaskAsCompleted(int id)
    {
        var task = _repository.GetTaskById(id);
        if (task != null)
        {
            task.IsCompleted = true;
            _view.DisplayMessage("Задача отмечена как завершённая.");
        }
        else
        {
            _view.DisplayMessage("Задача не найдена.");
        }
    }
}

// MAIN PROGRAM

class Program
{
    public static void Main()
    {
        var repository = new TaskRepository();
        var view = new ConsoleTaskView();
        var controller = new TaskController(repository, view);

        while (true)
        {
            Console.WriteLine("\nМенеджер задач:");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Показать задачи");
            Console.WriteLine("3. Редактировать задачу");
            Console.WriteLine("4. Удалить задачу");
            Console.WriteLine("5. Отметить задачу как завершённую");
            Console.WriteLine("6. Выйти");
            Console.Write("Выберите действие: ");

            if (int.TryParse(Console.ReadLine(), out int option))
            {
                switch (option)
                {
                    case 1:
                        Console.Write("Введите название задачи: ");
                        string title = Console.ReadLine();
                        Console.Write("Введите описание задачи: ");
                        string description = Console.ReadLine();
                        controller.AddTask(title, description);
                        break;
                    case 2:
                        controller.DisplayTasks();
                        break;
                    case 3:
                        Console.Write("Введите ID задачи для редактирования: ");
                        if (int.TryParse(Console.ReadLine(), out int editId))
                        {
                            Console.Write("Введите новое название: ");
                            string newTitle = Console.ReadLine();
                            Console.Write("Введите новое описание: ");
                            string newDescription = Console.ReadLine();
                            controller.EditTask(editId, newTitle, newDescription);
                        }
                        else
                        {
                            view.DisplayMessage("Неверный ID.");
                        }
                        break;
                    case 4:
                        Console.Write("Введите ID задачи для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            controller.DeleteTask(deleteId);
                        }
                        else
                        {
                            view.DisplayMessage("Неверный ID.");
                        }
                        break;
                    case 5:
                        Console.Write("Введите ID задачи для завершения: ");
                        if (int.TryParse(Console.ReadLine(), out int completeId))
                        {
                            controller.MarkTaskAsCompleted(completeId);
                        }
                        else
                        {
                            view.DisplayMessage("Неверный ID.");
                        }
                        break;
                    case 6:
                        return;
                    default:
                        view.DisplayMessage("Неверный выбор. Попробуйте ещё раз.");
                        break;
                }
            }
            else
            {
                view.DisplayMessage("Введите корректное число.");
            }
        }
    }
}
