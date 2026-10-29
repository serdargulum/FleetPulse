namespace StreamWorker.Application.Interfaces;

public interface IMessageConsumer
{
  Task ConsumeAsync(Func<string, Task> handler, CancellationToken cancellationToken = default);
}