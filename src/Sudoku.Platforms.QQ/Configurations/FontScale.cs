namespace Sudoku.Platforms.QQ.Configurations;

/// <summary>
/// Indicates the font rendering scale values.
/// </summary>
/// <param name="ValueScale">Indicates the value scale.</param>
/// <param name="CandidateScale">Indicates the candidate scale.</param>
/// <remarks>
/// <para><inheritdoc cref="ISudokuPainterFactory.WithFontScale(decimal)" path="//param[@name='fontScale']/para[2]"/></para>
/// <para><inheritdoc cref="ISudokuPainterFactory.WithFontScale(decimal)" path="//param[@name='fontScale']/para[3]"/></para>
/// <para>
/// In addition, I don't use keyword <see langword="readonly"/> to modify the type on purpose,
/// because <see langword="record struct"/> types will automatically generate for <see langword="init"/> properties if so;
/// otherwise, we'll get a normal property will both normal <see langword="get"/> and <see langword="set"/> accessors.
/// </para>
/// </remarks>
public record struct FontScale(decimal ValueScale, decimal CandidateScale);
