namespace Vlingo.Xoom.Streams
{
  public interface IControlledSubscription<T>
  {
    public void Cancel(SubscriptionController<T> subscription);
    public void Request(SubscriptionController<T> subscription, long maximum);
  }
}