using System.Threading.Tasks;

namespace Totem
{
  /// <summary>
  /// A <see cref="TaskCompletionSource{TResult}"/> with an <see cref="object"/>
  /// type, a null result, and continuations that run asynchonrously by default
  /// </summary>
  public class TaskSource : TaskCompletionSource<object>
  {
    public TaskSource()
      : base(TaskCreationOptions.RunContinuationsAsynchronously)
    {}

    public TaskSource(TaskCreationOptions creationOptions)
      : base(creationOptions)
    {}

    public TaskSource(object state)
      : base(state, TaskCreationOptions.RunContinuationsAsynchronously)
    {}

    public TaskSource(object state, TaskCreationOptions creationOptions)
      : base(state, creationOptions)
    {}

    public void SetResult() =>
      SetResult(null);

    public bool TrySetResult() =>
      TrySetResult(null);
  }

  /// <summary>
  /// A <see cref="TaskCompletionSource{TResult}"/> with continuations that
  /// run asynchonrously by default
  /// </summary>
  /// <typeparam name="TResult">The type of associated result</typeparam>
  public class TaskSource<TResult> : TaskCompletionSource<TResult>
  {
    public TaskSource()
      : base(TaskCreationOptions.RunContinuationsAsynchronously)
    {}

    public TaskSource(TaskCreationOptions creationOptions)
      : base(creationOptions)
    {}

    public TaskSource(object state)
      : base(state, TaskCreationOptions.RunContinuationsAsynchronously)
    {}

    public TaskSource(object state, TaskCreationOptions creationOptions)
      : base(state, creationOptions)
    {}
  }
}