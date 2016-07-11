using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalrTypescriptGenerator.Models {

  public class ComplexTypeInfo : IInterfaceTypeInfo {

    public ComplexTypeInfo(string name, string @namespace, IEnumerable<IMemberInfo> members, IInterfaceTypeInfo baseInterface) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (@namespace == null)
        throw new ArgumentNullException(nameof(@namespace));
      if (members == null)
        throw new ArgumentNullException(nameof(members));
      Name = name;
      Namespace = @namespace;
      Members = members;
      BaseInterface = baseInterface;
      GenericParameters = Enumerable.Empty<ITypeInfo>();
      GenericTypeDefinition = null;
    }

    public ComplexTypeInfo(string name, string @namespace, IEnumerable<IMemberInfo> members, IEnumerable<ITypeInfo> genericParameters, IInterfaceTypeInfo baseInterface) {
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
      GenericTypeDefinition = null;
    }

    public ComplexTypeInfo(string name, string @namespace, IEnumerable<IMemberInfo> members, IEnumerable<ITypeInfo> genericParameters, ITypeInfo genericTypeDefinition, IInterfaceTypeInfo baseInterface) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      if (@namespace == null)
        throw new ArgumentNullException(nameof(@namespace));
      if (members == null)
        throw new ArgumentNullException(nameof(members));
      if (genericParameters == null)
        throw new ArgumentNullException(nameof(genericParameters));
      if (genericTypeDefinition == null)
        throw new ArgumentNullException(nameof(genericTypeDefinition));
      Name = name;
      Namespace = @namespace;
      Members = members;
      GenericParameters = genericParameters;
      GenericTypeDefinition = genericTypeDefinition;
      BaseInterface = baseInterface;
    }

    public ContractType Type => Name != "JQueryPromise" ? ContractType.Interface : ContractType.Other;

    public string Name { get; }

    public string Namespace { get; }

    public IEnumerable<IMemberInfo> Members { get; }

    public IInterfaceTypeInfo BaseInterface { get; }

    public IEnumerable<ITypeInfo> GenericParameters { get; }

    public ITypeInfo GenericTypeDefinition { get; }

    public string FullName => Namespace.Length > 0 ? $"{Namespace}.{Name}" : Name;

    public string GetDeclaration(string surroundingNamespace) {
      var qualifiedName = this.ToQualifiedName(surroundingNamespace);
      if (!GenericParameters.Any())
        return qualifiedName;
      var arguments = string.Join(", ", GenericParameters.Select(typeInfo => typeInfo.GetDeclaration(surroundingNamespace)));
      return $"{qualifiedName}<{arguments}>";
    }

  }

}