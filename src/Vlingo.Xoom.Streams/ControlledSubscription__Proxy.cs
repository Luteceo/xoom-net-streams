using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams
{
  public class ControlledSubscription__Proxy<T> : ControlledSubscription<T>
  {
    private readonly Actor _actor;
    private readonly IMailbox _mailbox;

    public ControlledSubscription__Proxy(Actor actor, IMailbox mailbox)
    {
      _actor = actor;
      _mailbox = mailbox;
    }

    public void Cancel(SubscriptionController<T> subscription)
    {
      //throw new NotImplementedException();
    }

    public void Request(SubscriptionController<T> subscription, long maximum)
    {
      //throw new NotImplementedException();
    }
  }
}