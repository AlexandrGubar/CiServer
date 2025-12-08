using CiServer.Core.Entities;
namespace CiServer.Core.States;

public class FailedState : BuildState
{
    public override void StartBuild() => Console.WriteLine("Збірка завершена (провал). Створіть нову.");
    public override void FinishBuild(bool isSuccess) => Console.WriteLine("Вже завершено.");
    public override void CancelBuild() => Console.WriteLine("Вже скасовано/провалено.");
}