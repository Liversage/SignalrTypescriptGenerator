namespace SignalrTypescriptGenerator.Models {

  public interface IMemberInfo {

    string Name { get; }

    ITypeInfo Type { get; }

    string GetDeclaration(string surroundingNamespace);

  }

}