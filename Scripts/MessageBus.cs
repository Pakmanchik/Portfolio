public class MessageBus
{
    // Фрагмент кода, который реазлизует паттерн Observer 
    // Использовался вместо реактивнных фреймворков 
    public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : struct, IMessage
    {
        if (_publishingCount == 0)
            Subscribe(typeof(TMessage), handler);
        else
            _updates.Enqueue(new SubscriptionUpdate(UpdateType.Subscribe, typeof(TMessage), handler));
    }

    public void Unsubscribe<TMessage>(Action<TMessage> handler) where TMessage : struct, IMessage
    {
        if (_publishingCount == 0)
            Unsubscribe(typeof(TMessage), handler);
        else
            _updates.Enqueue(new SubscriptionUpdate(UpdateType.Unsubscribe, typeof(TMessage), handler));
    }
    
    public void Publish<TMessage>(TMessage message) where TMessage : struct, IMessage
    {
        if (_handlers.TryGetValue(typeof(TMessage), out var handlers))
        {
            _publishingCount++;
            foreach (var handler in handlers)
                ((Action<TMessage>)handler)(message);

            _publishingCount--;
            ActualizeSubscriptions();
        }
    }
    
    private void ActualizeSubscriptions()
    {
        if (_publishingCount == 0)
            while (_updates.TryDequeue(out var update))
                _updaters[update.Type](update);
    }
}