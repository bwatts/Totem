using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
  /// <summary>
  /// Computes hashes of .NET types based on normalized representations of their definitions
  /// </summary>
  public static class TypeHash
  {
    public static Sha1 Compute(RuntimeType type) => Compute(type.DeclaredType);
    public static Sha1 Compute(Type type) => new Computation(type).Execute();

    class Computation
    {
      private readonly StringBuilder _text = new StringBuilder();
      private TypeDefinition _type;
      private int _typeDepth;

      internal Computation(Type type)
      {
        _type = ModuleDefinition
          .ReadModule(type.Assembly.Location)
          .GetType(type.FullName);
      }

      internal Sha1 Execute()
      {
        WriteType();

        return ComputeHash();
      }

      void WriteType()
      {
        WriteFullName();

        WriteMethods();

        WriteFields();

        WriteNestedTypes();
      }

      void WriteFullName() => _text.AppendLine(_type.FullName);

      void WriteMethods()
      {
        var methods =
          from method in _type.Methods
          let text = method.ToString()
          orderby text
          select new
          {
            Text = text,
            Instructions = method.Body.Instructions
          };

        foreach(var method in methods)
        {
          WriteLine(method.Text);

          foreach(var instruction in method.Instructions)
          {
            // Offsets don't change the meaning of the code
            instruction.Offset = 0;

            WriteLine(instruction.ToString());
          }
        }
      }

      void WriteFields()
      {
        var fields =
          from field in _type.Fields
          let text = field.ToString()
          orderby text
          select text;

        foreach(var field in fields)
        {
          WriteLine(field);
        }
      }

      void WriteNestedTypes()
      {
        foreach(var nestedType in _type.NestedTypes)
        {
          var outerType = _type;

          _type = nestedType;
          _typeDepth++;

          WriteType();

          _type = outerType;
          _typeDepth--;
        }
      }

      void WriteLine(string value)
      {
        _text.Append(new string(' ', _typeDepth)).AppendLine(value);
      }

      Sha1 ComputeHash() => Sha1.Compute(_text.ToString());
    }
  }
}