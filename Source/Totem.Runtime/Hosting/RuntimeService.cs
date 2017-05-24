using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using Totem.Reflection;
using Totem.Runtime.Map;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// The Topshelf service control hosting the Totem runtime
  /// </summary>
  internal sealed class RuntimeService : ServiceControl, IBindable, IRuntimeService
  {
    readonly Assembly _programAssembly;
    HostControl _hostControl;
    CompositionContainer _container;
    CancellationTokenSource _cancellationTokenSource;
    IDisposable _instance;

    internal RuntimeService(string instanceName, Assembly programAssembly)
    {
      InstanceName = instanceName;
      _programAssembly = programAssembly;

      SetCurrentDirectoryToProgram();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    Fields Fields { get; } = new Fields();
    IClock Clock => Notion.Traits.Clock.Get(this);
    ILog Log => Notion.Traits.Log.Get(this);
    RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

    public string InstanceName { get; }

    void SetCurrentDirectoryToProgram()
    {
      // Run the service where installed, instead of a system folder

      Directory.SetCurrentDirectory(_programAssembly.GetDirectoryName());
    }

    //
    // Start
    //

    internal static IRuntimeService Instance;

    public bool Start(HostControl hostControl)
    {
      _hostControl = hostControl;

      Instance = this;

      Log.Info("[runtime] Starting service");

      OpenScope();

      StartMonitor();

      LoadInstance();

      Log.Info("[runtime] Service started");

      return true;
    }

    void OpenScope()
    {
      _container = new CompositionContainer(Runtime.Catalog, CompositionOptions.DisableSilentRejection);

      _cancellationTokenSource = new CancellationTokenSource();
    }

    void StartMonitor()
    {
      try
      {
        Runtime.Monitor.Start(InstanceName);
      }
      catch(Exception error)
      {
        Log.Error(error, "[runtime] Failed to start performance monitoring");
      }
    }

    void LoadInstance()
    {
      try
      {
        _instance = _container
          .GetExportedValue<CompositionRoot>()
          .Connect(_cancellationTokenSource.Token);
      }
      catch(Exception error)
      {
        throw new Exception("Failed to compose the runtime service", GetRootException(error));
      }
    }

    Exception GetRootException(Exception error)
    {
      // http://haacked.com/archive/2014/12/09/unwrap-mef-exception/

      var compositionError = error as CompositionException;

      if(compositionError == null)
      {
        return error;
      }

      var unwrapped = compositionError;

      while(unwrapped != null)
      {
        var firstError = unwrapped.Errors.FirstOrDefault();

        if(firstError == null)
        {
          break;
        }

        var currentError = firstError.Exception;

        if(currentError == null)
        {
          break;
        }

        var composablePartError = currentError as ComposablePartException;

        if(composablePartError != null && composablePartError.InnerException != null)
        {
          var innerCompositionError = composablePartError.InnerException as CompositionException;

          if(innerCompositionError == null)
          {
            return currentError.InnerException ?? error;
          }

          currentError = innerCompositionError;
        }

        unwrapped = currentError as CompositionException;
      }

      return error;
    }

    //
    // Stop
    //

    public bool Stop(HostControl hostControl)
    {
      UnloadInstance();

      CloseScope();

      return true;
    }

    void UnloadInstance()
    {
      if(_instance != null)
      {
        _cancellationTokenSource.Cancel();

        _instance.Dispose();
      }
    }

    void CloseScope()
    {
      if(_container != null)
      {
        _container.Dispose();
      }

      _hostControl = null;
      _container = null;
      _cancellationTokenSource = null;
      _instance = null;
    }

    //
    // Restart
    //

    public void Restart(string reason)
    {
      Restarted = true;

      Log.Info("[runtime] Restarting in 5s: {Reason:l}", reason);

      Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => _hostControl.Stop());
    }

    internal bool Restarted { get; private set; }
  }
}