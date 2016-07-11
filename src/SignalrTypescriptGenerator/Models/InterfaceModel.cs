namespace SignalrTypescriptGenerator.Models {

  public class InterfaceModel {

    public IInterfaceTypeInfo Interface { get; set; }

    public string SurroundingNamespace { get; set; }

    public bool Indent => SurroundingNamespace.Length > 0;

  }

  public class EnumModel {

    public IEnumTypeInfo Enum { get; set; }

    public string SurroundingNamespace { get; set; }

    public bool Indent => SurroundingNamespace.Length > 0;


  }

}