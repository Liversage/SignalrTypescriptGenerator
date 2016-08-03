using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalrTypescriptGenerator.Models {

  public class GenericTypeDefinitionInfo : IInterfaceTypeInfo {

    public GenericTypeDefinitionInfo(string name, string @namespace, IEnumerable<IMemberInfo> members, IEnumerable<ITypeInfo> genericParameters, IInterfaceTypeInfo baseInterface) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (@namespace == null)
        throw new ArgumentNullException(nameof(@namespace));
      if (members == null)
        throw new ArgumentNullException(nameof(members));
      if (genericParameters == null)
        throw new ArgumentNullException(nameof(genericParameters));
      Name = name;
      Namespace = @namespace;
      Members = members;
      GenericParameters = genericParameters;
      BaseInterface = baseInterface;
    }

    public bool IsTopLevel => true;

    public ContractType Type => ContractType.Interface;

    public string Name { get; }

    public string Namespace { get; }

    public IEnumerable<IMemberInfo> Members { get; }

    public IInterfaceTypeInfo BaseInterface { get; }

    public IEnumerable<ITypeInfo> GenericParameters { get; }

    public string FullName => Namespace.Length > 0 ? $"{Namespace}.{Name}" : Name;

    public string GetDeclaration(string surroundingNamespace) {
      var qualifiedName = this.ToQualifiedName(surroundingNamespace);
      var arguments = string.Join(", ", GenericParameters.Select(typeInfo => typeInfo.GetDeclaration(surroundingNamespace)));
      return $"{qualifiedName}<{arguments}>";
    }

  }

}