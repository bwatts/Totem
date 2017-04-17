using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem
{
  /// <summary>
  /// The observable value of a field in a bindable object
  /// </summary>
  public struct FieldValue : ITextable, IObservable<FieldChange>
  {
    readonly IBindable _binding;
    readonly Field _field;
    object _content;
    bool _contentSet;
    ConcurrentDictionary<Subscription, bool> _subscriptions;
    int _subscriptionsLocked;

    internal FieldValue(IBindable binding, Field field) : this()
    {
      _binding = binding;
      _field = field;
    }

    internal FieldValue(IBindable binding, Field field, object content) : this()
    {
      _binding = binding;
      _field = field;
      _content = content;
      _contentSet = true;
    }

    public IBindable Binding
    {
      get
      {
        if(_binding == null)
        {
          throw new InvalidOperationException("This type is a struct for performance reasons. Do not use the default constructor.");
        }

        return _binding;
      }
    }

    public Field Field
    {
      get
      {
        if(_field == null)
        {
          throw new InvalidOperationException("This type is a struct for performance reasons. Do not use the default constructor.");
        }

        return _field;
      }
    }

    public object Content
    {
      get
      {
        if(!_contentSet)
        {
          _content = Field.ResolveDefault();
        }

        return _content;
      }
      private set
      {
        var oldContent = Interlocked.Exchange(ref _content, value);

        if(_subscriptions != null)
        {
          var subscriptions = _subscriptions.Keys.ToList();

          if(subscriptions.Count > 0 && !EqualityComparer<object>.Default.Equals(oldContent, value))
          {
            foreach(var subscription in subscriptions)
            {
              subscription.OnNext(oldContent, value);
            }
          }
        }
      }
    }

    public bool IsUnset => !_contentSet;
    public bool IsSet => _contentSet;

    public sealed override string ToString() => ToText();
    public Text ToText() => Text.Of(Content);

    public void Set(object content)
    {
      _contentSet = true;

      Content = content;
    }

    //
    // Subscriptions
    //

    public IDisposable Subscribe(IObserver<FieldChange> observer)
    {
      if(_subscriptions == null && LockSubscriptions())
      {
        _subscriptions = new ConcurrentDictionary<Subscription, bool>();

        UnlockSubscriptions();
      }

      var subscription = new Subscription(this, observer);

      _subscriptions.TryAdd(subscription, true);

      return subscription;
    }

    bool LockSubscriptions()
    {
      var original = Interlocked.CompareExchange(ref _subscriptionsLocked, 1, 0);

      return original == 0;
    }

    void UnlockSubscriptions()
    {
      Interlocked.Exchange(ref _subscriptionsLocked, 0);
    }

    void Unsubscribe(Subscription subscription)
    {
      bool ignored;

      _subscriptions.TryRemove(subscription, out ignored);
    }

    class Subscription : IDisposable
    {
      readonly FieldValue _value;
      readonly IObserver<FieldChange> _observer;

      internal Subscription(FieldValue value, IObserver<FieldChange> observer)
      {
        _value = value;
        _observer = observer;
      }

      internal void OnNext(object oldContent, object newContent)
      {
        _observer.OnNext(new FieldChange(_value.Binding, _value.Field, oldContent, newContent));
      }

      internal void OnError(Exception error)
      {
        _observer.OnError(error);
      }

      public void Dispose()
      {
        try
        {
          _observer.OnCompleted();
        }
        finally
        {
          _value.Unsubscribe(this);
        }
      }
    }
  }
}