using System;

namespace SignalrTypescriptGenerator.Models {

  public class ParameterInfo {

    public ParameterInfo(string name, ITypeInfo type) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (type == null)
        throw new ArgumentNullException(nameof(type));
      Name = name;
      Type = type;
    }

    public string Name { get; }

    public ITypeInfo Type { get; }

  }

}