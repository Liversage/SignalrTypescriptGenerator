using System.Linq;

namespace SignalrTypescriptGenerator {

  public static class StringExtensions {

    public static string ToCamelCase(this string identifier) {
      if (string.IsNullOrEmpty(identifier))
        return identifier;
      var initialUpperCaseCharacterCount = identifier.TakeWhile(char.IsUpper).Count();
      if (initialUpperCaseCharacterCount == 0)
        return identifier;
      if (initialUpperCaseCharacterCount == 1)
        return char.ToLowerInvariant(identifier[0]) + identifier.Substring(1);
      var initialCharacterCountToLowerCase = initialUpperCaseCharacterCount - 1;
      var camelCasedCharacters = identifier
        .Take(initialCharacterCountToLowerCase)
        .Select(char.ToLowerInvariant)
        .Concat(identifier.Skip(initialCharacterCountToLowerCase));
      return new string(camelCasedCharacters.ToArray());
    }

  }

}