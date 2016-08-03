using System;

namespace SignalrTypescriptGenerator.Models {

  public class TypeInfoPlaceholder : ITypeInfo {

    public ITypeInfo TypeInfo { get; private set; }

    public bool IsTopLevel {
      get {
        ThrowIfTypeInfoIsMissing();
        return TypeInfo.IsTopLevel;
      }
    }

    public ContractType Type {
      get {
        ThrowIfTypeInfoIsMissing();
        return TypeInfo.Type;
      }
    }

    public string Name {
      get {
        ThrowIfTypeInfoIsMissing();
        return TypeInfo.Name;
      }
    }

    public string Namespace {
      get {
        ThrowIfTypeInfoIsMissing();
        return TypeInfo.Namespace;
      }
    }

    public string FullName {
      get {
        ThrowIfTypeInfoIsMissing();
        return TypeInfo.FullName;
      }
    }

    public string GetDeclaration(string surroundingNamespace) {
      ThrowIfTypeInfoIsMissing();
      return TypeInfo.GetDeclaration(surroundingNamespace);
    }

    public void SetTypeInfo(ITypeInfo typeInfo) {
      if (typeInfo == null)
        throw new ArgumentNullException(nameof(typeInfo));
      while (GetType().IsInstanceOfType(typeInfo)) {
        var typeInfoPlaceholder = (TypeInfoPlaceholder) typeInfo;
        typeInfo = typeInfoPlaceholder.TypeInfo;
      }
      if (typeInfo == null)
        throw new ArgumentException("The type info of a placeholder cannot be set to another placeholder without a non-placeholder type info.");

      TypeInfo = typeInfo;
    }

    void ThrowIfTypeInfoIsMissing() {
      if (TypeInfo == null)
        throw new InvalidOperationException("No type info has been set.");
    }

  }

}