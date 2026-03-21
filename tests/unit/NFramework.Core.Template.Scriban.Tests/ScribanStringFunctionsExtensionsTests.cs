using Shouldly;
using Xunit;

namespace NFramework.Core.Template.Scriban.Tests;

/// <summary>
/// Unit tests for ScribanStringFunctionsExtensions
/// </summary>
public class ScribanStringFunctionsExtensionsTests
{
    #region CamelCase Tests

    [Fact]
    public void CamelCase_WithPascalCaseInput_ShouldConvertToCamelCase()
    {
        // Arrange
        string input = "UserName";

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("username");
    }

    [Fact]
    public void CamelCase_WithSnakeCaseInput_ShouldConvertToCamelCase()
    {
        // Arrange
        string input = "user_name";

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("user_Name");
    }

    [Fact]
    public void CamelCase_WithKebabCaseInput_ShouldConvertToCamelCase()
    {
        // Arrange
        string input = "user-name";

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("userName");
    }

    [Fact]
    public void CamelCase_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void CamelCase_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void CamelCase_WithSingleCharacterInput_ShouldReturnLowerCase()
    {
        // Arrange
        string input = "A";

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("a");
    }

    [Fact]
    public void CamelCase_WithAllUpperCaseInput_ShouldConvertToCamelCase()
    {
        // Arrange
        string input = "ALLCAPS";

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("allcaps");
    }

    [Fact]
    public void CamelCase_WithMixedCaseInput_ShouldConvertToCamelCase()
    {
        // Arrange
        string input = "myUserName";

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("myusername");
    }

    #endregion

    #region PascalCase Tests

    [Fact]
    public void PascalCase_WithSnakeCaseInput_ShouldConvertToPascalCase()
    {
        // Arrange
        string input = "user_name";

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("User_Name");
    }

    [Fact]
    public void PascalCase_WithCamelCaseInput_ShouldConvertToPascalCase()
    {
        // Arrange
        string input = "userName";

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("Username");
    }

    [Fact]
    public void PascalCase_WithKebabCaseInput_ShouldConvertToPascalCase()
    {
        // Arrange
        string input = "user-name";

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("UserName");
    }

    [Fact]
    public void PascalCase_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void PascalCase_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void PascalCase_WithMixedSegments_ShouldPreserveUnderscoreSegments()
    {
        // Arrange
        string input = "MY_USER_NAME";

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("My_User_Name");
    }

    [Fact]
    public void PascalCase_WithSingleWordInput_ShouldCapitalizeFirstLetter()
    {
        // Arrange
        string input = "hello";

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("Hello");
    }

    [Fact]
    public void PascalCase_WithSpaceSeparatedInput_ShouldConvertToPascalCase()
    {
        // Arrange
        string input = "user name";

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("UserName");
    }

    #endregion

    #region SnakeCase Tests

    [Fact]
    public void SnakeCase_WithPascalCaseInput_ShouldConvertToSnakeCase()
    {
        // Arrange
        string input = "UserName";

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("user_name");
    }

    [Fact]
    public void SnakeCase_WithCamelCaseInput_ShouldConvertToSnakeCase()
    {
        // Arrange
        string input = "myUserName";

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("my_user_name");
    }

    [Fact]
    public void SnakeCase_WithKebabCaseInput_ShouldConvertToSnakeCase()
    {
        // Arrange
        string input = "user-name";

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("user_name");
    }

    [Fact]
    public void SnakeCase_WithSpaceSeparatedInput_ShouldConvertToSnakeCase()
    {
        // Arrange
        string input = "user name";

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("user_name");
    }

    [Fact]
    public void SnakeCase_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void SnakeCase_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void SnakeCase_WithAcronymInput_ShouldConvertToSnakeCase()
    {
        // Arrange
        string input = "JSONParser";

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("jsonparser");
    }

    #endregion

    #region KebabCase Tests

    [Fact]
    public void KebabCase_WithPascalCaseInput_ShouldConvertToKebabCase()
    {
        // Arrange
        string input = "UserName";

        // Act
        string result = ScribanStringFunctionsExtensions.KebabCase(input);

        // Assert
        result.ShouldBe("user-name");
    }

    [Fact]
    public void KebabCase_WithCamelCaseInput_ShouldConvertToKebabCase()
    {
        // Arrange
        string input = "myUserName";

        // Act
        string result = ScribanStringFunctionsExtensions.KebabCase(input);

        // Assert
        result.ShouldBe("my-user-name");
    }

    [Fact]
    public void KebabCase_WithSnakeCaseInput_ShouldConvertToKebabCase()
    {
        // Arrange
        string input = "user_name";

        // Act
        string result = ScribanStringFunctionsExtensions.KebabCase(input);

        // Assert
        result.ShouldBe("user-name");
    }

    [Fact]
    public void KebabCase_WithSpaceSeparatedInput_ShouldConvertToKebabCase()
    {
        // Arrange
        string input = "user name";

        // Act
        string result = ScribanStringFunctionsExtensions.KebabCase(input);

        // Assert
        result.ShouldBe("user-name");
    }

    [Fact]
    public void KebabCase_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.KebabCase(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void KebabCase_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.KebabCase(input);

        // Assert
        result.ShouldBe("");
    }

    #endregion

    #region Abbreviation Tests

    [Fact]
    public void Abbreviation_WithMultipleWords_ShouldReturnAbbreviation()
    {
        // Arrange
        string input = "User Name";

        // Act
        string result = ScribanStringFunctionsExtensions.Abbreviation(input);

        // Assert
        result.ShouldBe("un");
    }

    [Fact]
    public void Abbreviation_WithPascalCaseInput_ShouldReturnAbbreviation()
    {
        // Arrange
        string input = "UserName";

        // Act
        string result = ScribanStringFunctionsExtensions.Abbreviation(input);

        // Assert
        result.ShouldBe("un");
    }

    [Fact]
    public void Abbreviation_WithThreeWords_ShouldReturnThreeLetters()
    {
        // Arrange
        string input = "My User Name";

        // Act
        string result = ScribanStringFunctionsExtensions.Abbreviation(input);

        // Assert
        result.ShouldBe("mun");
    }

    [Fact]
    public void Abbreviation_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.Abbreviation(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Abbreviation_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.Abbreviation(input);

        // Assert
        result.ShouldBe("");
    }

    #endregion

    #region Plural Tests

    [Fact]
    public void Plural_WithSingularWord_ShouldReturnPlural()
    {
        // Arrange
        string input = "user";

        // Act
        string result = ScribanStringFunctionsExtensions.Plural(input);

        // Assert
        result.ShouldBe("users");
    }

    [Fact]
    public void Plural_WithIrregularSingular_ShouldReturnPlural()
    {
        // Arrange
        string input = "person";

        // Act
        string result = ScribanStringFunctionsExtensions.Plural(input);

        // Assert
        result.ShouldBe("people");
    }

    [Fact]
    public void Plural_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.Plural(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Plural_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.Plural(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Plural_WithWordEndingInS_ShouldHandleCorrectly()
    {
        // Arrange
        string input = "class";

        // Act
        string result = ScribanStringFunctionsExtensions.Plural(input);

        // Assert
        result.ShouldBe("classes");
    }

    #endregion

    #region Singular Tests

    [Fact]
    public void Singular_WithPluralWord_ShouldReturnSingular()
    {
        // Arrange
        string input = "users";

        // Act
        string result = ScribanStringFunctionsExtensions.Singular(input);

        // Assert
        result.ShouldBe("user");
    }

    [Fact]
    public void Singular_WithIrregularPlural_ShouldReturnSingular()
    {
        // Arrange
        string input = "people";

        // Act
        string result = ScribanStringFunctionsExtensions.Singular(input);

        // Assert
        result.ShouldBe("person");
    }

    [Fact]
    public void Singular_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.Singular(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Singular_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.Singular(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Singular_WithWordEndingInIes_ShouldReturnSingular()
    {
        // Arrange
        string input = "parties";

        // Act
        string result = ScribanStringFunctionsExtensions.Singular(input);

        // Assert
        result.ShouldBe("party");
    }

    #endregion

    #region Words Tests

    [Fact]
    public void Words_WithPascalCaseInput_ShouldSplitIntoWords()
    {
        // Arrange
        string input = "UserName";

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("User Name");
    }

    [Fact]
    public void Words_WithSnakeCaseInput_ShouldSplitIntoWords()
    {
        // Arrange
        string input = "user_name";

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("user name");
    }

    [Fact]
    public void Words_WithKebabCaseInput_ShouldSplitIntoWords()
    {
        // Arrange
        string input = "user-name";

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("user name");
    }

    [Fact]
    public void Words_WithCamelCaseInput_ShouldSplitIntoWords()
    {
        // Arrange
        string input = "myUserName";

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("my User Name");
    }

    [Fact]
    public void Words_WithEmptyInput_ShouldReturnEmpty()
    {
        // Arrange
        string input = "";

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Words_WithNullInput_ShouldReturnEmpty()
    {
        // Arrange
        string? input = null;

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Words_WithMultipleSeparators_ShouldNormalizeSpaces()
    {
        // Arrange
        string input = "user--name___test";

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("user name test");
    }

    [Fact]
    public void Words_WithSpaceInput_ShouldReturnTrimmed()
    {
        // Arrange
        string input = "  user name  ";

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("user name");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void CamelCase_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 123;

        // Act
        string result = ScribanStringFunctionsExtensions.CamelCase(input);

        // Assert
        result.ShouldBe("123");
    }

    [Fact]
    public void PascalCase_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 456;

        // Act
        string result = ScribanStringFunctionsExtensions.PascalCase(input);

        // Assert
        result.ShouldBe("456");
    }

    [Fact]
    public void SnakeCase_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 789;

        // Act
        string result = ScribanStringFunctionsExtensions.SnakeCase(input);

        // Assert
        result.ShouldBe("789");
    }

    [Fact]
    public void KebabCase_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 101;

        // Act
        string result = ScribanStringFunctionsExtensions.KebabCase(input);

        // Assert
        result.ShouldBe("101");
    }

    [Fact]
    public void Abbreviation_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 202;

        // Act
        string result = ScribanStringFunctionsExtensions.Abbreviation(input);

        // Assert
        result.ShouldBe("2");
    }

    [Fact]
    public void Plural_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 303;

        // Act
        string result = ScribanStringFunctionsExtensions.Plural(input);

        // Assert
        result.ShouldBe("303");
    }

    [Fact]
    public void Singular_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 404;

        // Act
        string result = ScribanStringFunctionsExtensions.Singular(input);

        // Assert
        result.ShouldBe("404");
    }

    [Fact]
    public void Words_WithIntegerInput_ShouldConvertToString()
    {
        // Arrange
        object input = 505;

        // Act
        string result = ScribanStringFunctionsExtensions.Words(input);

        // Assert
        result.ShouldBe("505");
    }

    #endregion
}
