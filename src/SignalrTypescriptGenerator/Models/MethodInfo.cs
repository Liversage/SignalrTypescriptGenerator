using System;
using System.Collections.Generic;

namespace SignalrTypescriptGenerator.Models {

  public class MethodInfo : IMemberInfo {

    public MethodInfo(string name, ITypeInfo type, IEnumerable<ParameterInfo> parameters) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (type == null)
        throw new ArgumentNullException(nameof(type));
      if (parameters == null)
        throw new ArgumentNullException(nameof(parameters));
      Name = name;
      Type = type;
      Parameters = parameters;
    }

    public string Name { get; }

    public IEnumerable<ParameterInfo> Parameters { get; }

    public ITypeInfo Type { get; }

    public string GetDeclaration(string surroundingNamespace) => $"{Name}{Parameters.ToParameterList(surroundingNamespace)}: {Type.GetDeclaration(surroundingNamespace)}";

  }

}