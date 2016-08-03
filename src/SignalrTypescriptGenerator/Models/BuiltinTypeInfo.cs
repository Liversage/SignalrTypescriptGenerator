using System;

namespace SignalrTypescriptGenerator.Models {

  public class BuiltinTypeInfo : ITypeInfo {

    public BuiltinTypeInfo(string name) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      Name = name;
    }

    public bool IsTopLevel => false;

    public ContractType Type => ContractType.Other;

    public string Name { get; }

    public string Namespace => string.Empty;

    public string FullName => Name;

    public string GetDeclaration(string surroundingNamespace) {
      return Name;
    }

    public static readonly BuiltinTypeInfo Any = new BuiltinTypeInfo("any");

  }

}