namespace SignalrTypescriptGenerator.Models {

  public class PropertyInfo : IMemberInfo {

    public PropertyInfo(string name, ITypeInfo type) {
      Name = name;
      Type = type;
    }

    public string Name { get; }

    public string GetDeclaration(string surroundingNamespace) => $"{Name}: {Type.GetDeclaration(surroundingNamespace)}";

    public ITypeInfo Type { get; }

  }

}