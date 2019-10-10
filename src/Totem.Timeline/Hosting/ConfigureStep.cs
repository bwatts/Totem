using System;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// A step in the configuration of a Totem app
  /// </summary>
  /// <typeparam name="TArg">The single argument accepted by the step</typeparam>
  public class ConfigureStep<TArg>
  {
    Action<TArg> _before;
    Action<TArg> _after;
    Action<TArg> _replace;

    public TConfigureApp Before<TConfigureApp>(TConfigureApp app, Action<TArg> configure)
    {
      _before = configure;

      return app;
    }

    public TConfigureApp After<TConfigureApp>(TConfigureApp app, Action<TArg> configure)
    {
      _after = configure;

      return app;
    }

    public TConfigureApp Replace<TConfigureApp>(TConfigureApp app, Action<TArg> configure)
    {
      _replace = configure;

      return app;
    }

    public void Apply(TArg arg, Action action = null)
    {
      _before?.Invoke(arg);

      if(_replace != null)
      {
        _replace(arg);
      }
      else
      {
        action?.Invoke();
      }

      _after?.Invoke(arg);
    }
  }

  /// <summary>
  /// A step in the configuration of a Totem app
  /// </summary>
  /// <typeparam name="TArg0">The first of two arguments accepted by the step</typeparam>
  /// <typeparam name="TArg1">The second of two arguments accepted by the step</typeparam>
  public class ConfigureStep<TArg0, TArg1>
  {
    Action<TArg0, TArg1> _before;
    Action<TArg0, TArg1> _after;
    Action<TArg0, TArg1> _replace;

    public TConfigureApp Before<TConfigureApp>(TConfigureApp app, Action<TArg0, TArg1> configure)
    {
      _before = configure;

      return app;
    }

    public TConfigureApp Before<TConfigureApp>(TConfigureApp app, Action<TArg1> configure)
    {
      _before = (arg0, arg1) => configure(arg1);

      return app;
    }

    public TConfigureApp After<TConfigureApp>(TConfigureApp app, Action<TArg0, TArg1> configure)
    {
      _after = configure;

      return app;
    }

    public TConfigureApp After<TConfigureApp>(TConfigureApp app, Action<TArg1> configure)
    {
      _after = (arg0, arg1) => configure(arg1);

      return app;
    }

    public TConfigureApp Replace<TConfigureApp>(TConfigureApp app, Action<TArg0, TArg1> configure)
    {
      _replace = configure;

      return app;
    }

    public TConfigureApp Replace<TConfigureApp>(TConfigureApp app, Action<TArg1> configure)
    {
      _replace = (arg0, arg1) => configure(arg1);

      return app;
    }

    public void Apply(TArg0 arg0, TArg1 arg1, Action action = null)
    {
      _before?.Invoke(arg0, arg1);

      if(_replace != null)
      {
        _replace(arg0, arg1);
      }
      else
      {
        action?.Invoke();
      }

      _after?.Invoke(arg0, arg1);
    }
  }
}