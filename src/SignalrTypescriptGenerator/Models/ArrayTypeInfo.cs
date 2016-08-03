using System;

namespace SignalrTypescriptGenerator.Models {

  public class ArrayTypeInfo : ITypeInfo {

    public ArrayTypeInfo(ITypeInfo elementType) {
      if (elementType == null)
        throw new ArgumentNullException(nameof(elementType));
      ElementType = elementType;
    }

    public bool IsTopLevel => false;

    public ContractType Type => ContractType.Other;

    public string Name => "Array";

    public string Namespace => string.Empty;

    public string FullName => Name;

    public ITypeInfo ElementType { get; }

    public string GetDeclaration(string surroundingNamespace) {
      return ElementType.GetDeclaration(surroundingNamespace) + "[]";
    }

  }

}