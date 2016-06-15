using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using SignalrTypescriptGenerator.Models;

namespace SignalrTypescriptGenerator {

  class TypeHelper {

    readonly HashSet<Type> _doneTypes;

    public readonly Stack<Type> EnumTypes;

    public readonly Stack<Type> InterfaceTypes;

    public TypeHelper() {
      _doneTypes = new HashSet<Type>();
      InterfaceTypes = new Stack<Type>();
      EnumTypes = new Stack<Type>();
    }

    public List<FunctionDetails> GetClientFunctions(Type hubType) {
      var list = new List<FunctionDetails>();

      var clientType = ClientType(hubType);
      if (clientType == null)
        return list;
      foreach (var method in clientType.GetMethods()) {
        var functionDetails = new FunctionDetails();
        var ps = method.GetParameters().Select(x => x.Name + ": " + GetTypeContractName(x.ParameterType));
        var functionName = method.Name.ToCamelCase();
        var functionArgs = "(" + string.Join(", ", ps) + ")";

        functionDetails.Name = functionName;
        functionDetails.Arguments = functionArgs;

        list.Add(functionDetails);
      }

      return list;
    }

    public Type ClientType(Type hubType) {
      while (hubType != null && hubType != typeof(Hub)) {
        if (hubType.IsGenericType && hubType.GetGenericTypeDefinition() == typeof(Hub<>))
          return hubType.GetGenericArguments().Single();
        hubType = hubType.BaseType;
      }
      return null;
    }

    public string GetTypeContractName(Type type) {
      if (type == typeof(Task))
        return "void";

      if (type.IsArray)
        return GetTypeContractName(type.GetElementType()) + "[]";

      if (type.IsGenericType) {
        if (typeof(Task<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
          return GetTypeContractName(type.GetGenericArguments()[0]);

        if (typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
          return GetTypeContractName(type.GetGenericArguments()[0]);

        if (typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
          return GetTypeContractName(type.GetGenericArguments()[0]) + "[]";
      }

      switch (type.Name.ToLowerInvariant()) {
        case "datetime":
          return "Date";
        case "int16":
        case "int32":
        case "int64":
        case "single":
        case "double":
          return "number";
        case "boolean":
          return "boolean";
        case "void":
        case "string":
          return type.Name.ToLowerInvariant();
      }

      if (_doneTypes.Contains(type))
        return GenericSpecificName(type, true);
      _doneTypes.Add(type);
      if (type.IsEnum)
        EnumTypes.Push(type);
      else
        InterfaceTypes.Push(type);
      return GenericSpecificName(type, true);
    }

    public string GenericSpecificName(Type type, bool referencing) {
      var name = (referencing ? type.FullName : type.Name).Split('`').First();
      if (type.IsGenericType)
        name += "_" + string.Join("_", type.GenericTypeArguments.Select(a => GenericSpecificName(a, false))) + "_";
      return name;
    }

  }

}