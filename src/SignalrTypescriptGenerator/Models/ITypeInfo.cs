namespace SignalrTypescriptGenerator.Models {

  public interface ITypeInfo {

    bool IsTopLevel { get; }

    ContractType Type { get; }

    string Name { get; }

    string Namespace { get; }

    string FullName { get; }

    string GetDeclaration(string surroundingNamespace);

  }

}