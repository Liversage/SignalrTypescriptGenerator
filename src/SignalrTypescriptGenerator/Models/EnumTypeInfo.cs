using System;
using System.Collections.Generic;

namespace SignalrTypescriptGenerator.Models {

  public class EnumTypeInfo : IEnumTypeInfo {

    public EnumTypeInfo(string name, string @namespace, IEnumerable<IMemberInfo> members) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (@namespace == null)
        throw new ArgumentNullException(nameof(@namespace));
      if (members == null)
        throw new ArgumentNullException(nameof(members));
      Name = name;
      Namespace = @namespace;
      Members = members;
    }

    public ContractType Type => ContractType.Enum;

    public string Name { get; }

    public string Namespace { get; }

    public IEnumerable<IMemberInfo> Members { get; }

    public string FullName => Namespace.Length > 0 ? $"{Namespace}.{Name}" : Name;

    public string GetDeclaration(string surroundingNamespace) {
      return this.ToQualifiedName(surroundingNamespace);
    }

  }

}