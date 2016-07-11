using System;
using System.Collections.Generic;

namespace SignalrTypescriptGenerator.Models {

  public class FunctionTypeInfo : ITypeInfo {

    public FunctionTypeInfo(IEnumerable<ParameterInfo> parameters, ITypeInfo returnType) {
      if (parameters == null)
        throw new ArgumentNullException(nameof(parameters));
      if (returnType == null)
        throw new ArgumentNullException(nameof(returnType));
      Parameters = parameters;
      ReturnType = returnType;
    }

    public ContractType Type => ContractType.Other;

    public string Name => "function";

    public string Namespace => string.Empty;

    public string FullName => Name;

    public IEnumerable<ParameterInfo> Parameters { get; }

    public ITypeInfo ReturnType { get; }

    public string GetDeclaration(string surroundingNamespace) => $"{Parameters.ToParameterList(surroundingNamespace)} => {ReturnType.GetDeclaration(surroundingNamespace)}";

  }

}