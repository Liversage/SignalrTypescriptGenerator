using System.Collections.Generic;

namespace SignalrTypescriptGenerator.Models {

  public interface IInterfaceTypeInfo : ITypeInfo {

    IInterfaceTypeInfo BaseInterface { get; }

    IEnumerable<IMemberInfo> Members { get; }

  }

}