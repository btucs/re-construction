using System;
using System.Threading.Tasks;
using RSG;

public static class PromiseExtensions
{
  public static async Task<PromiseT> Await<PromiseT>(this IPromise<PromiseT> promiseToAwait)
  {
    var awaitablePromise = await new PromiseAwaitable<PromiseT>(promiseToAwait).Run();
    return awaitablePromise;
  }
}