using System;
using Vlingo.Xoom.Actors;

namespace Reactive.Streams
{
  public class Publisher__Proxy<T> : IPublisher<T>
  {
    private static readonly string _subscriptionRepresentation1 = "Subscribe(ISubscriber<T> subscriber)";
    private readonly Actor _actor;
    private readonly IMailbox _mailbox;

    public Publisher__Proxy(Actor actor, IMailbox mailbox)
    {
      _actor = actor;
      _mailbox = mailbox;
    }

    public void Subscribe(ISubscriber<T> subscriber)
    {
      if (subscriber == null)
        throw new ArgumentNullException("Subscriber must not be null");
      if (_actor.IsStopped)
        _actor.DeadLetters.FailedDelivery(new DeadLetter(_actor, _subscriptionRepresentation1));
      else
      {
        Action<IPublisher<T>> consumer = (actor) => actor.Subscribe(subscriber);
        if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, _subscriptionRepresentation1);
        else _mailbox.Send(new LocalMessage<IPublisher<T>>(_actor, consumer, _subscriptionRepresentation1));
      }
    }
  }
}