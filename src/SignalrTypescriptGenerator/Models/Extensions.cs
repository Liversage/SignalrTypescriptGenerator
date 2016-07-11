using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalrTypescriptGenerator.Models {

  public static class Extensions {

    public static string ToParameterList(this IEnumerable<ParameterInfo> parameters, string surroundingNamespace) {
      if (parameters == null)
        throw new ArgumentNullException(nameof(parameters));
      if (surroundingNamespace == null)
        throw new ArgumentNullException(nameof(surroundingNamespace));
      return "(" + string.Join(", ", parameters.Select(parameter => $"{parameter.Name}: {parameter.Type.GetDeclaration(surroundingNamespace)}")) + ")";
    }

    public static string ToQualifiedName(this ITypeInfo typeInfo, string surroundingNamespace) {
      if (typeInfo == null)
        throw new ArgumentNullException(nameof(typeInfo));
      if (surroundingNamespace == null)
        throw new ArgumentNullException(nameof(surroundingNamespace));
      var @namespace = typeInfo.Namespace;
      @namespace = @namespace == surroundingNamespace ? string.Empty : RemoveSharedPrefix(@namespace, surroundingNamespace);
      return @namespace.Length > 0 ? $"{@namespace}.{typeInfo.Name}" : typeInfo.Name;
    }

    static string RemoveSharedPrefix(string @namespace, string surroundingNamespace) {
      var namespaceParts = @namespace.Split('.');
      var surroundingNamespaceParts = surroundingNamespace.Split('.');
      var sharedParts = namespaceParts.Zip(surroundingNamespaceParts, (a, b) => new { a, b }).Where(x => x.a == x.b).Select(x => x.a);
      var sharedPrefix = string.Join(".", sharedParts);
      @namespace = @namespace.Substring(sharedPrefix.Length);
      return @namespace.StartsWith(".") ? @namespace.Substring(1) : @namespace;
    }

  }

}