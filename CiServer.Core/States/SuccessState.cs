using CiServer.Core.Entities;
namespace CiServer.Core.States;

public class SuccessState : BuildState
{
    public override void StartBuild() => Console.WriteLine("Збірка вже завершена (успіх). Не можна запустити знову.");
    public override void FinishBuild(bool isSuccess) => Console.WriteLine("Вже завершено.");
    public override void CancelBuild() => Console.WriteLine("Не можна скасувати завершену збірку.");
}