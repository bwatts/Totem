using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reflection;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;
using Totem.Runtime.Timeline;

namespace Totem.Runtime
{
	/// <summary>
	/// Reads the Start method of a request
	/// </summary>
	internal sealed class RuntimeReaderRequestStart : Notion
	{
		readonly RuntimeMap _map;
    readonly Type _declaredType;
    MethodInfo _method;

    internal RuntimeReaderRequestStart(RuntimeMap map, Type declaredType)
		{
      _map = map;
      _declaredType = declaredType;
		}

		internal RequestStartMethod Read()
		{
      var firstTwoMethods = ReadMethods().Take(2).ToList();

      var method = firstTwoMethods.FirstOrDefault();
      var duplicate = firstTwoMethods.Skip(1).FirstOrDefault();

      if(duplicate != null)
      {
        Log.Warning(
          "[runtime] Hierarchy of request {Type} must have exactly one Start method. Duplicate found in {DuplicateType}",
          _declaredType.FullName,
          duplicate.DeclaringType.FullName);
      }
      else if(method == null)
      {
        Log.Warning("[runtime] Request {Type} has no Start method", _declaredType.FullName);
      }
      else
      {
        _method = method;

        return ReadMethod();
      }

      return null;
    }

    IEnumerable<MethodInfo> ReadMethods() =>
      from sourceType in _declaredType.GetInheritanceChainTo<Request>(includeType: true, includeTargetType: false)
      let method = sourceType.GetMethod(
        "Start",
        BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Static
        | BindingFlags.Instance
        | BindingFlags.DeclaredOnly)
      where method != null
      select method;

    RequestStartMethod ReadMethod()
    {
      if(_method.IsStatic)
      {
        Log.Warning("[runtime] Request method {Type}.Start is static", _method.DeclaringType.FullName);
      }
      else if(!_method.IsPrivate)
      {
        Log.Warning("[runtime] Request method {Type}.Start is not private", _method.DeclaringType.FullName);
      }
      else if(!typeof(Event).IsAssignableFrom(_method.ReturnType))
      {
        Log.Warning("[runtime] Request method {Type}.Start does not return an event", _method.DeclaringType.FullName);
      }
      else
      {
        var eventTypes = ReadEventTypes().ToMany();

        if(eventTypes.Count == 0)
        {
          Log.Warning("[runtime] Request method {Type}.Start returns an abstract type but specifies no start events", _method.DeclaringType.FullName);
        }
        else
        {
          return new RequestStartMethod(_method, eventTypes, ReadDependencies());
        }
      }

      return null;
    }

    IEnumerable<EventType> ReadEventTypes()
    {
      foreach(var declaredType in ReadDeclaredEventTypes())
      {
        var eventType = _map.GetEvent(declaredType, strict: false);

        if(eventType == null)
        {
          Log.Warning(
            "[runtime] Request method {Type}.Start specifies start event {Event} that is not in the map",
            _method.DeclaringType.FullName,
            declaredType.FullName);
        }
        else
        {
          yield return eventType;
        }
      }
    }

    IEnumerable<Type> ReadDeclaredEventTypes()
    {
      if(!_method.ReturnType.IsAbstract)
      {
        yield return _method.ReturnType;
      }
      else
      {
        foreach(var attribute in _method.GetCustomAttributes<StartEventAttribute>(inherit: true))
        {
          yield return attribute.Type;
        }
      }
    }

    Many<Dependency> ReadDependencies()
    {
      return _method.GetParameters().ToMany(parameter =>
      {
        var attribute = parameter.GetCustomAttribute<DependencyAttribute>(inherit: true);

        return attribute?.GetDependency(parameter) ?? Dependency.Typed(parameter);
      });
    }
	}
}