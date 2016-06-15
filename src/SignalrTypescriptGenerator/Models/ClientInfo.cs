using System.Collections.Generic;

namespace SignalrTypescriptGenerator.Models {

  public class ClientInfo {

    public ClientInfo() {
      FunctionDetails = new List<FunctionDetails>();
    }

    public string ModuleName { get; set; }

    public string InterfaceName { get; set; }

    public List<FunctionDetails> FunctionDetails { get; set; }

  }

}