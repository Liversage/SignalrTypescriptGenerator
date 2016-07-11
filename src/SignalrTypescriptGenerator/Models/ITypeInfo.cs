namespace SignalrTypescriptGenerator.Models {

  public interface ITypeInfo {

    ContractType Type { get; }

    string Name { get; }

    string Namespace { get; }

    string FullName { get; }

    string GetDeclaration(string surroundingNamespace);

  }

}