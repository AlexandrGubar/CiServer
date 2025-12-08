using CiServer.Core.Entities;

namespace CiServer.Core.States;

public class RunningState : BuildState
{
    public override void StartBuild()
    {
        Console.WriteLine("Помилка: Збірка вже виконується.");
    }

    public override void FinishBuild(bool isSuccess)
    {
        if (isSuccess)
        {
            Console.WriteLine("Збірка успішно завершена!");
            _context.TransitionTo(new SuccessState());
        }
        else
        {
            Console.WriteLine("Збірка провалилася під час виконання.");
            _context.TransitionTo(new FailedState());
        }
    }

    public override void CancelBuild()
    {
        Console.WriteLine("Зупинка активної збірки...");
        _context.TransitionTo(new FailedState());
    }
}