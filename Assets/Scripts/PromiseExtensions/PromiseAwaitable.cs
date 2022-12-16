using System;
using System.Threading.Tasks;
using RSG;

internal class PromiseAwaitable
{
  private TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

  public PromiseAwaitable(IPromise promise)
  {
    promise.Done(OnCompleted, OnRejected);
  }

  private void OnCompleted()
  {
    _taskCompletionSource.TrySetResult(true);
  }

  private void OnRejected(Exception ex)
  {
    _taskCompletionSource.TrySetException(ex);
  }

  public Task<bool> Run()
  {
    return _taskCompletionSource.Task;
  }
}

internal class PromiseAwaitable<T>
{
  private TaskCompletionSource<T> _taskCompletionSource = new TaskCompletionSource<T>();

  public PromiseAwaitable(IPromise<T> promise)
  {
    promise.Done(OnFinished, OnRejected);
  }

  private void OnFinished(T returnValue)
  {
    _taskCompletionSource.TrySetResult(returnValue);
  }

  private void OnRejected(Exception ex)
  {
    _taskCompletionSource.TrySetException(ex);
  }

  public Task<T> Run()
  {
    return _taskCompletionSource.Task;
  }
}