using System.Text.RegularExpressions;

sealed partial class Build
{
    /// <summary>
    ///     Regex for parsing Process arguments from the output.
    /// </summary>
    readonly Regex ArgumentsRegex = ArgumentsRegexGenerator();

    [GeneratedRegex("'(.+?)'", RegexOptions.Compiled)]
    private static partial Regex ArgumentsRegexGenerator();
}