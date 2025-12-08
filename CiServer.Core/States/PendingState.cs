using CiServer.Core.Entities;

namespace CiServer.Core.States;

public class PendingState : BuildState
{
    public override void StartBuild()
    {
        Console.WriteLine("Перехід зі стану PENDING у RUNNING...");
        _context.TransitionTo(new RunningState());
    }

    public override void FinishBuild(bool isSuccess)
    {
        Console.WriteLine("Помилка: Не можна завершити збірку, яка ще не почалася.");
    }

    public override void CancelBuild()
    {
        Console.WriteLine("Збірку скасовано до початку.");
        _context.TransitionTo(new FailedState());
    }
}