using System;

namespace SignalrTypescriptGenerator.Models {

  public class FieldInfo : IMemberInfo {

    public FieldInfo(string name, string value) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (value == null)
        throw new ArgumentNullException(nameof(value));
      Name = name;
      Value = value;
    }

    public string Name { get; }

    public string Value { get; set; }

    public ITypeInfo Type => null;

    public string GetDeclaration(string surroundingNamespace) {
      return $"{Name} = {Value}";
    }

  }

}