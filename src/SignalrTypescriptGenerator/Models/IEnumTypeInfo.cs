using System.Collections.Generic;

namespace SignalrTypescriptGenerator.Models {

  public interface IEnumTypeInfo : ITypeInfo {

    IEnumerable<IMemberInfo> Members { get; }

  }

}