using C5;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using SCG = System.Collections.Generic;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Performs the actual parsing of a command line and provides methods for retrieving descriptive information
/// about the options and groups available.
/// </summary>
public sealed class CommandLineParser : IDisposable
{
	#region Private Fields

	/// <summary>
	/// The stack of lexers. This is used when file arguments are processed on the command line, in which case a new 
	/// lexer is created and pushed onto the stack so parsing will continue from that lexer instead. The current
	/// lexer is the one on the top of the stack.
	/// </summary>
	private readonly ArrayList<LexerStackRecord> _lexerStack = [];

	/// <summary>
	/// The option manager, i.e. the object used for storing the values of the options read. Supplied by the 
	/// user.
	/// </summary>
	private readonly object _optionManager;

	/// <summary>
	/// Dictionary mapping option names to the corresponding <see cref="Option"/> or <see cref="OptionAlias"/> object.
	/// </summary>
	private readonly TreeDictionary<string, IOption> _options;

	/// <summary>
	/// Set containing all defined option groups.
	/// </summary>
	private readonly TreeSet<OptionGroup> _optionGroups;

	/// <summary>
	/// The comparer to be used for comparing option names. This is either case sensitive, or case insensitive 
	/// depending on what was specified in the <see cref="CommandLineManagerAttribute"/> of the 
	/// command line manager.
	/// </summary>
	private readonly OptionNameComparer _optionNameComparer;

	/// <summary>
	/// The option styles enabled for this parser.
	/// </summary>
	private readonly OptionStyles _enabledOptionStyles;

	/// <summary>
	/// <see cref="NumberFormatInfo"/> dictating how numeric values should be parsed.
	/// </summary>
	readonly NumberFormatInfo _numberFormatInfo;

	/// <summary>
	/// The name of the executable file from the command line, or null if none was available.
	/// </summary>
	private string _executable = string.Empty;

	/// <summary>
	/// The list of the remaining arguments, i.e. those that were not options or values assigned to options.
	/// </summary>
	private readonly ArrayList<string> _remainingArguments = [];

	/// <summary>
	/// The list of errors.
	/// </summary>
	private readonly HashSet<ErrorInfo> _errors = [];

	/// <summary>
	/// The escape characters available to this parser.
	/// </summary>
	private readonly ArrayList<char> _escapeCharacters = [];

	/// <summary>
	/// Dictionary mapping quotation marks to <see cref="QuotationInfo"/> objects describing what
	/// characters may be escaped within those quotes.
	/// </summary>
	private readonly HashDictionary<char, QuotationInfo> _quotations = [];

	/// <summary>
	/// Dictionary mapping assignemnt characters to the option styles for which they are enabled.
	/// </summary>
	private readonly HashDictionary<char, OptionStyles> _assignmentCharacters = [];

	/// <summary>
	/// Separators that may be used for separating values in strings that may contain several values, such as 
	/// the Aliases or Prohibits attributes of <see cref="CommandLineOptionAttribute"/>.
	/// </summary>
	private static readonly char [] _separators = [' ', ',', ';'];

	/// <summary>
	/// The default escape characters.
	/// </summary>
	private static readonly char[] _defaultEscapeCharacters = ['\\'];

	/// <summary>
	/// Value indicating whether we are currently parsing.
	/// </summary>
	private bool _isParsing;

	/// <summary>
	/// Contains the object describing the command line options and groups for the option manager associated
	/// with this parser. It will be null before such an object is generated (upon request)
	/// </summary>
	private UsageInfo? _usageDescription;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="CommandLineParser"/> class.
	/// </summary>
	/// <param name="optionManager">The option manager.</param>
	/// <remarks><para>The option manager is the user defined object that is tagged with the various attributes
	/// available in this namespace that contains definitions of the available options.</para>
	/// <para>This constructor initializes the parser with the <see cref="NumberFormatInfo"/> specified 
	/// in <see cref="System.Globalization.CultureInfo.NumberFormat"/> of <see cref="CultureInfo.CurrentUICulture"/>. This may not be desired, since it affects
	/// the parsing of numbers by this parser.</para></remarks>
	public CommandLineParser(object optionManager) :
		this(optionManager, CultureInfo.CurrentUICulture.NumberFormat)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CommandLineParser"/> class.
	/// </summary>
	/// <param name="optionManager">The option manager.</param>
	/// <param name="numberFormatInfo">An object describing how numerical arguments should be parsed.</param>
	/// <remarks><para>The option manager is the user defined object that is tagged with the various attributes
	/// available in this namespace that contains definitions of the available options.</para>
	/// <para>This constructor initializes the parser with the <see cref="NumberFormatInfo"/> specified, which affects
	/// the way that numbers are parsed.</para>
	/// <para>By default the assignment characters available are the equal sign ('=') for all options and 
	/// the colon (':') for windows style options. See <see cref="AddAssignmentCharacter"/> for information
	/// on how to change this.</para>
	/// <para>The default escape character is the backslash ('\'). See <see cref="SetEscapeCharacters"/> for information
	/// on how to change this.</para>
	/// <para>The default quotation enabled is by using double quotes ('"') within which the backslash ('\') character
	/// and the double quote ('"') can be escaped.</para>
	/// <para>In unquoted values the space character and the backslash can be escaped.</para></remarks>
	/// <exception cref="ArgumentNullException"><paramref name="optionManager"/> or <paramref name="numberFormatInfo"/> was 
	/// a null reference</exception>
	/// <exception cref="AttributeException">an error in the configuration of the option manager was found.</exception>
	/// <exception cref="InvalidOperationException">Indicates an internal error and should normally never be thrown.</exception>
	public CommandLineParser(object optionManager, NumberFormatInfo numberFormatInfo)
	{
		// Set default escape characters
		SetEscapeCharacters(_defaultEscapeCharacters);

		// Set the default assignment characters
		AddAssignmentCharacter('=', OptionStyles.All);
		AddAssignmentCharacter(':', OptionStyles.Windows);

		// Set the default quotations
		QuotationInfo unquotedQuotation = new('\0');
		unquotedQuotation.AddEscapeCode(' ', ' ');
		unquotedQuotation.AddEscapeCode('\\', '\\');
		AddQuotation(unquotedQuotation);

		QuotationInfo doubleQuotedQuotation = new('\"');
		doubleQuotedQuotation.AddEscapeCode('\"', '\"');
		doubleQuotedQuotation.AddEscapeCode('\\', '\\');
		AddQuotation(doubleQuotedQuotation);

		ArgumentNullException.ThrowIfNull(optionManager);

		ArgumentNullException.ThrowIfNull(numberFormatInfo);

		_numberFormatInfo	= numberFormatInfo;
		_optionManager		= optionManager;

		CommandLineManagerAttribute managerAttr = Attribute.GetCustomAttribute(_optionManager.GetType(), typeof(CommandLineManagerAttribute)) as CommandLineManagerAttribute ??
			throw new AttributeException(typeof(CommandLineManagerAttribute), _optionManager.GetType(), CommandLineStrings.MissingRequiredAttributeForACommandLineManagerObjectCommandLineManagerAttribute);

		ApplicationName		= managerAttr.ApplicationName;
		ApplicationVersion		= managerAttr.Version;
		ApplicationDescription	= managerAttr.Description;
		ApplicationCopyright	= managerAttr.Copyright;
		_enabledOptionStyles	= managerAttr.EnabledOptionStyles;

		_optionNameComparer		= new OptionNameComparer(managerAttr.IsCaseSensitive);
		_options				= new TreeDictionary<string, IOption>(_optionNameComparer);
		_optionGroups			= new TreeSet<OptionGroup>(_optionNameComparer, _optionNameComparer);

		// Parse the CommandLineOptionGroupAttributes of the manager object and create all OptionGroup instances representing
		// these attributes.
		foreach (object attr in Attribute.GetCustomAttributes(_optionManager.GetType(), typeof(CommandLineOptionGroupAttribute)))
		{
			CommandLineOptionGroupAttribute? groupAttr = attr as CommandLineOptionGroupAttribute;
			Debug.Assert(groupAttr != null);

			if (String.IsNullOrEmpty(groupAttr.Id))
			{
				throw new AttributeException(typeof(CommandLineOptionGroupAttribute), _optionManager.GetType(),
					CommandLineStrings.InvalidIdOfGroupIdMustNotBeNullOrEmpty);
			}


			if (!_optionGroups.Add(new OptionGroup(groupAttr.Id, groupAttr.Name, groupAttr.Description, groupAttr.Require, groupAttr.HasRequireExplicitAssignment ? groupAttr.RequireExplicitAssignment : managerAttr.RequireExplicitAssignment, _optionNameComparer)))
			{
				throw new AttributeException(typeof(CommandLineOptionGroupAttribute), _optionManager.GetType(),
					String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.RedefinitionOfGroupWithId01,
					groupAttr.Id, _optionNameComparer.IsCaseSensitive ? CommandLineStrings.UsingCaseSensitiveNames :
					CommandLineStrings.UsingCaseInsensitiveNames));
			}
		}

		// Temporary list to contain the prohibitions of each option.
		TreeDictionary<string, TreeSet<string>> prohibitions = new(_optionNameComparer);

		// Traverse the CommandLineOptionAttribute attributes of the manager object and create Option instances
		// to represent the command line options as well as OptionAlias instances for the aliases.
		foreach (MemberInfo member in _optionManager.GetType().GetMembers())
		{
			foreach (object attr in member.GetCustomAttributes(typeof(CommandLineOptionAttribute), false))
			{
				CommandLineOptionAttribute? optionAttr = attr as CommandLineOptionAttribute;
				Debug.Assert(optionAttr != null);

				Option option = new(optionAttr, member, _optionManager, _optionGroups, _numberFormatInfo);

				Debug.Assert(!String.IsNullOrEmpty(option.Name));
				if (_options.Contains(option.Name))
				{
					throw new AttributeException(typeof(CommandLineOptionAttribute), member,
						String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.RedefinitionOfCommandLineOptionWithParameterName01,
						option.Name, _optionNameComparer.IsCaseSensitive ? CommandLineStrings.UsingCaseSensitiveNames :
					CommandLineStrings.UsingCaseInsensitiveNames));
				}

				_options.Add(option.Name, option);

				// Check special rules concering option styles and option names
				if (optionAttr.BoolFunction == BoolFunction.UsePrefix)
				{
					if (!OptionStyleManager.IsAllEnabled(managerAttr.EnabledOptionStyles, OptionStyles.Plus))
					{
						throw new AttributeException(typeof(CommandLineOptionAttribute), member,
							String.Format(CultureInfo.CurrentUICulture,
							CommandLineStrings.BoolFunctionMayNotBeSetTo0WhenOptionStyle1IsNotSpecifiedInThe2Attribute,
							BoolFunction.UsePrefix.ToString(),
							OptionStyles.Plus.ToString(),
							typeof(CommandLineManagerAttribute).Name));
					}
					else if (OptionStyleManager.IsAllEnabled(managerAttr.EnabledOptionStyles, OptionStyles.Windows))
					{
						throw new AttributeException(typeof(CommandLineOptionAttribute), member,
							String.Format(CultureInfo.CurrentUICulture,
							CommandLineStrings.BoolFunctionMayNotBeSetTo0WhenOptionStyle1IsSpecifiedInThe2Attribute,
							BoolFunction.UsePrefix.ToString(),
							OptionStyles.Windows.ToString(),
							typeof(CommandLineManagerAttribute).Name));
					}
					else if (OptionStyleManager.IsAllEnabled(managerAttr.EnabledOptionStyles, OptionStyles.Group) && option.Name.Length > 1)
					{
						throw new AttributeException(typeof(CommandLineOptionAttribute), member,
							String.Format(CultureInfo.CurrentUICulture,
							CommandLineStrings.BoolFunctionCanNotBeSetTo0ForOptionWithANameLongerThanOneCharacterWhenOptionStyle1IsSpecifiedInThe2Attribute,
							BoolFunction.UsePrefix.ToString(),
							OptionStyles.Group.ToString(),
							typeof(CommandLineManagerAttribute).Name));
					}
				}

				// Find all aliases and create OptionAlias instances for these.
				if (optionAttr.Aliases != null)
				{
					string[] aliasNames = optionAttr.Aliases.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
					foreach (string alias in aliasNames)
					{
						if (_options.Contains(alias))
						{
							throw new AttributeException(typeof(CommandLineOptionAttribute), member,
								String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.AliasName0IsAlreadyInUseByAnotherOption,
								alias));
						}
						else if (option.BoolFunction == BoolFunction.UsePrefix &&
							OptionStyleManager.IsAllEnabled(managerAttr.EnabledOptionStyles, OptionStyles.Group) && alias.Length > 1)
						{
							throw new AttributeException(typeof(CommandLineOptionAttribute), member,
								String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.Alias0IsInvalidForOption1BoolFunctionIsSetTo2AndThe3OptionStyleIsEnabledWhichProhibitsAnyNameLongerThanOneCharacter,
								alias, option.Name, option.BoolFunction, OptionStyles.Group));
						}
						_options.Add(alias, new OptionAlias(alias, option));
						option.AddAlias(alias);
					}
				}

				if (optionAttr.Prohibits != null)
				{
					string [] targetArray = optionAttr.Prohibits.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

					if (!prohibitions.Contains(option.Name))
					{
						prohibitions.Add(option.Name, new TreeSet<string>(_optionNameComparer, _optionNameComparer));
					}

					TreeSet<string> targets = prohibitions[option.Name];
					targets.AddAll(targetArray);

					foreach (string target in targetArray)
					{
						if (!prohibitions.Contains(target))
						{
							prohibitions.Add(target, []);
						}
						prohibitions[target].Add(option.Name);
					}
				}
			}
		}

		// Find and add the options that each option prohibits.
		foreach (SCG.KeyValuePair<string, TreeSet<string>> entry in prohibitions)
		{
			if (entry.Value != null && entry.Value.Count > 0)
			{
				string key = entry.Key;
				if (!_options.Find(ref key, out IOption ioption))
				{
					throw new InvalidOperationException(CommandLineStrings.InternalErrorOptionSpecifiedInProhibitionDoesNotExist);
				}

				Option option;
				if (ioption.IsAlias)
				{
					option = ioption.DefiningOption;
				}
				else
				{
					option = (Option)ioption;
				}

				foreach (string name in entry.Value)
				{
					string currentName = name;
					if (!_options.Find(ref currentName, out IOption target))
					{
						throw new AttributeException(
							String.Format(
								CultureInfo.CurrentUICulture,
								CommandLineStrings.UndefinedOption0ReferencedFromProhibitionSectionOfOption1,
								currentName,
								option.Name
							)
						);
					}

					if (target.IsAlias)
					{
						target = target.DefiningOption;
					}

					if (!target.ProhibitedBy.Contains(option))
					{
						target.ProhibitedBy.Add(option);
					}
				}
			}
		}
	}

	#endregion

	#region Public Properties

	/// <summary>
	/// Gets or sets the name of the application.
	/// </summary>
	/// <value>The name of the application.</value>
	public string ApplicationName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the application version.
	/// </summary>
	/// <value>The application version.</value>
	public string ApplicationVersion { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the application copyright.
	/// </summary>
	/// <value>The application copyright.</value>
	public string ApplicationCopyright { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the application description.
	/// </summary>
	/// <value>The application description.</value>
	public string ApplicationDescription { get; set; } = string.Empty;

	/// <summary>
	/// Gets the path to the executable of this application if it was included on the command line (it normally is), or null otherwise.
	/// </summary>
	/// <value>the path to the executable of this application if it was included on the command line.</value>
	/// <remarks>This should only be evaluated after <see cref="Parse()"/> has been called.</remarks>
	public string ExecutablePath { get => _executable; }

	/// <summary>
	/// Gets a value indicating whether any errors were encountered during parsing.
	/// </summary>
	/// <value>
	/// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>This should only be evaluated after <see cref="Parse()"/> has been called.</remarks>
	public bool HasErrors { get  => !_errors.IsEmpty; }

	/// <summary>
	/// Gets a collection containing any parse errors that occured.
	/// </summary>
	/// <value>The parse errors that occured.</value>
	/// <remarks>This should only be evaluated after <see cref="Parse()"/> has been called.</remarks>
	public ICollection<ErrorInfo> Errors { get  => _errors; }

	/// <summary>
	/// Gets the remaining arguments specified on the command line, i.e. those that were not interpreted as options
	/// or values assigned to options.
	/// </summary>
	/// <value>The remaining arguments.</value>
	public ArrayList<string> RemainingArguments { get  => _remainingArguments; }

	/// <summary>
	/// Gets an object containing the descriptive properties of the option manager from which this instance was
	/// created.
	/// </summary>
	/// <value>The usage description.</value>
	/// <remarks>The returned <see cref="UsageInfo"/> can be used to change descriptions of the options
	/// and groups of this parser, and to retrieve a formatted string suitable for displaying as a help 
	/// message to the user.</remarks>
	public UsageInfo UsageInfo
	{
		get
		{
			_usageDescription ??= new UsageInfo(_options, _enabledOptionStyles, this);
			return _usageDescription;
		}
	}

	#endregion

	#region Private Properties

	/// <summary>
	/// Gets the next token that will be read by the current lexer, or null if no more tokens are available
	/// from the current lexer.
	/// </summary>
	/// <value>the next token that will be read by the current lexer, or null if no more tokens are available
	/// from the current lexer.</value>
	/// <remarks>This will not remove the token from the buffer, but rather works as a peek.</remarks>
	private Token? LA1
	{
		get
		{
			try
			{
				Debug.Assert(CurrentLexer != null);
				LA1Token ??= CurrentLexer.GetNextToken();
			}
			catch (MissingClosingQuoteException)
			{
				ReportError(ParseErrorCodes.MissingClosingQuote, CommandLineStrings.MissingClosingQuoteForQuotedValue);
				// No more tokes will be available after this message, (since the lexer parsed
				// to the end of input). So we leave mLA1Token at null.
			}
			catch (MissingOptionNameException)
			{
				ReportError(ParseErrorCodes.EmptyOptionName, CommandLineStrings.EmptyOptionNameIsNotAllowed);
				// Since an empty option name is not allowed, we discard it
				// This if statement is only to make the recursive call happen. We need to 
				// use the value of LA1 somehow to call it.
				if (LA1 == null)
				{
					return null;
				}
			}
			return LA1Token;
		}
	}

	/// <summary>
	/// Gets the current lexer.
	/// </summary>
	/// <value>The current lexer or null if there are no lexers.</value>
	private Lexer? CurrentLexer
	{
		get
		{
			if (_lexerStack.IsEmpty)
			{
				return null;
			}
			return _lexerStack[^1].Lexer;
		}
	}

	/// <summary>
	/// Gets the current file being read by the lexer.
	/// </summary>
	/// <value>The current file being read by the current lexer, or a null reference if no file is being read.</value>
	private string? CurrentFile
	{
		get
		{
			if (CurrentLexer == null)
			{
				return null;
			}
			return _lexerStack[^1].FileName;
		}
	}

	/// <summary>
	/// Gets or sets the lookahead token.
	/// </summary>
	/// <value>The lookahead token.</value>
	/// <remarks>This method should not be used by any method other than <see cref="LA1"/>. Use <see cref="LA1"/> instead.</remarks>
	private Token? LA1Token
	{
		get
		{
			if (_lexerStack.IsEmpty)
			{
				return null;
			}
			return _lexerStack[^1].LA1Token;
		}

		set
		{
			Debug.Assert(!_lexerStack.IsEmpty);
			_lexerStack[^1].LA1Token = value;
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Performs the actual parsing of the command line, setting (or calling) the relevant members of the
	/// option manager.
	/// </summary>
	/// <remarks><para>The command line from <see cref="Environment.CommandLine"/> will be used for parsing, assuming
	/// that the first argument is the path to the executable of this application.</para>
	/// <para>After parsing has been completed the <see cref="HasErrors"/> property and <see cref="Errors"/> property
	/// should be examined to determine if the command line was correct or not. If there are errors, the state
	/// of the option manager object (i.e. which properties has been set or not) is undefined.</para></remarks>
	public void Parse()
	{
		Parse(Environment.CommandLine, true);
	}

	/// <summary>
	/// Performs the actual parsing of a command line, setting (or calling) the relevant members of the
	/// option manager.
	/// </summary>
	/// <param name="commandLine">A string containing the command line passed to be parsed.</param>
	/// <param name="containsExecutable">if set to <c>true</c> it is assumed that the
	/// first argument on the command line is the path to the executable used to run this program.</param>
	/// <remarks>After parsing has been completed the <see cref="HasErrors"/> property and <see cref="Errors"/> property
	/// should be examined to determine if the command line was correct or not. If there are errors, the state
	/// of the option manager object (i.e. which properties has been set or not) is undefined.</remarks>
	public void Parse(string commandLine, bool containsExecutable)
	{
		StringReader stringReader = new(commandLine);
		Parse(stringReader, containsExecutable);
		stringReader.Dispose();
	}

	/// <summary>
	/// Performs the actual parsing of a command line, setting (or calling) the relevant members of the
	/// option manager.
	/// </summary>
	/// <param name="commandLine">A string containing the command line passed to be parsed. It is assumed that the
	/// first argument on the command line is the path to the executable used to run this program.</param>
	/// <remarks>After parsing has been completed the <see cref="HasErrors"/> property and <see cref="Errors"/> property
	/// should be examined to determine if the command line was correct or not. If there are errors, the state
	/// of the option manager object (i.e. which properties has been set or not) is undefined.</remarks>
	public void Parse(string commandLine)
	{
		Parse(commandLine, true);
	}

	/// <summary>
	/// Performs the actual parsing of a command line, setting (or calling) the relevant members of the
	/// option manager.
	/// </summary>
	/// <param name="args">An array containing the arguments passed to this program.</param>
	/// <param name="containsExecutable">if set to <c>true</c> it is assumed that the
	/// first argument in this list is the path to the executable used to run this program.</param>
	/// <remarks>After parsing has been completed the <see cref="HasErrors"/> property and <see cref="Errors"/> property
	/// should be examined to determine if the command line was correct or not. If there are errors, the state
	/// of the option manager object (i.e. which properties has been set or not) is undefined.</remarks>
	public void Parse(string[] args, bool containsExecutable)
	{
		Parse(String.Join(" ", args), containsExecutable);
	}

	/// <summary>
	/// Performs the actual parsing of a command line, setting (or calling) the relevant members of the
	/// option manager.
	/// </summary>
	/// <param name="args">An array containing the arguments passed to this program. It is assumed that the
	/// first argument in this list is the path to the executable used to run this program.</param>
	/// <remarks>After parsing has been completed the <see cref="HasErrors"/> property and <see cref="Errors"/> property
	/// should be examined to determine if the command line was correct or not. If there are errors, the state
	/// of the option manager object (i.e. which properties has been set or not) is undefined.</remarks>
	public void Parse(string[] args)
	{
		Parse(args, true);
	}

	/// <summary>
	/// Performs the actual parsing of a command line, setting (or calling) the relevant members of the
	/// option manager.
	/// </summary>
	/// <param name="input">A <see cref="System.IO.TextReader"/> from which the command line can be read.</param>
	/// <remarks>The command line parser will assume that the first
	/// argument read from the command line is the path to the executable that were used to run this program.
	/// After parsing has been completed the <see cref="HasErrors"/> property and <see cref="Errors"/> property
	/// should be examined to determine if the command line was correct or not. If there are errors, the state 
	/// of the option manager object (i.e. which properties has been set or not) is undefined.</remarks>
	public void Parse(TextReader input)
	{
		Parse(input, true);
	}

	/// <summary>
	/// Performs the actual parsing of a command line, setting (or calling) the relevant members of the
	/// option manager.
	/// </summary>
	/// <param name="input">A <see cref="System.IO.TextReader"/> from which the command line can be read.</param>
	/// <param name="containsExecutable">if set to <c>true</c> the command line parser will assume that the first
	/// argument read from the command line is the path to the executable that were used to run this program.</param>
	/// <remarks>After parsing has been completed the <see cref="HasErrors"/> property and <see cref="Errors"/> property
	/// should be examined to determine if the command line was correct or not. If there are errors, the state 
	/// of the option manager object (i.e. which properties has been set or not) is undefined.</remarks>
	public void Parse(TextReader input, bool containsExecutable)
	{
		Debug.Assert(_lexerStack.IsEmpty);
		_isParsing = true;
		
		try
		{
			PushLexer(new Lexer(input, _escapeCharacters, _quotations, _assignmentCharacters), null);

			// Set the option styles for the lexer.
			Debug.Assert(CurrentLexer != null);
			CurrentLexer.EnabledOptionStyles = _enabledOptionStyles;

			if (containsExecutable)
			{
				ValueToken valueToken = CurrentLexer.GetNextValueToken();
				if (valueToken != null)
				{
					_executable = valueToken.Value;
				}
			}

			// Perform the actual parsing of the input.
			while (CurrentLexer != null)
			{
				if (LA1 == null && !PopFinishedLexers())
				{
					break;
				}

				Token token = LA1!;
				switch (token.TokenType)
				{
					case TokenTypes.ValueToken:
						_remainingArguments.Add(((ValueToken)GetNextToken()!).Value);
						break;
					case TokenTypes.AssignmentToken:
						ReportError(ParseErrorCodes.UnexpectedAssignment, CommandLineStrings.Unexpected0CharacterOnCommandLine, ((AssignmentToken)token).AssignmentCharacter);
						SkipTokens(1);
						break;
					case TokenTypes.OptionNameToken:
						MatchOptionName();
						break;
					case TokenTypes.EndToken:
						CurrentLexer.EnabledOptionStyles = OptionStyles.None;
						SkipTokens(1);
						break;
					case TokenTypes.OptionFileToken:
						MatchOptionFile();
						break;
					default:
						throw new InvalidOperationException(CommandLineStrings.InternalErrorUnimplementedTokenTypeReturnedToCommandLineParser);
				}
			}

			// Verify that the MinOccurs restriction of each option has been satisfied
			foreach (SCG.KeyValuePair<string, IOption> entry in _options)
			{
				if (!entry.Value.IsAlias)
				{
					Option option = (Option)entry.Value;
					if (option.MinOccurs > option.SetCount)
					{
						if (option.SetCount == 0)
						{
							ReportOptionError(ParseErrorCodes.MissingRequiredOption, option.Name,
								CommandLineStrings.MissingRequiredOption0, option.Name);
						}
						else if (option.MinOccurs == option.MaxOccurs)
						{
							ReportOptionError(ParseErrorCodes.IllegalCardinality, option.Name,
								CommandLineStrings.Only0OccurenceSOfOption1FoundItMustBeSpecifiedExactly2TimeS,
								option.SetCount, option.Name, option.MinOccurs);
						}
						else
						{
							ReportOptionError(ParseErrorCodes.IllegalCardinality, option.Name,
								CommandLineStrings.Only0OccurenceSOfOption1FoundItMustBeSpecifiedAtLeast2TimeS,
								option.SetCount, option.Name, option.MinOccurs);
						}
					}
				}
			}

			// Verify that the group requirements have been satisified
			foreach (OptionGroup group in _optionGroups)
			{
				int optionCount = 0;
				if (group.Require != OptionGroupRequirement.None)
				{
					// If there are requirements from this group, count the number of options
					// from the group that was actually defined on the commandline. (Multiple 
					// occurences of the same option is only counted as one though)
					foreach (SCG.KeyValuePair<string, Option> entry in group.Options)
					{
						if (entry.Value.SetCount > 0)
						{
							optionCount++;
						}
					}
				}

				switch (group.Require)
				{
					case OptionGroupRequirement.None:
						// No requirements, so nothing to check
						break;
					case OptionGroupRequirement.AtMostOne:
						if (optionCount > 1)
						{
							ReportError(ParseErrorCodes.IllegalCardinality, CommandLineStrings.AtMostOneOfTheOptions0MayBeSpecifiedAtOnce, group.GetOptionNamesAsString());
						}
						break;
					case OptionGroupRequirement.AtLeastOne:
						if (optionCount < 1)
						{
							ReportError(ParseErrorCodes.IllegalCardinality, CommandLineStrings.AtLeastOneOfTheOption0MustBeSpecified, group.GetOptionNamesAsString());
						}
						break;
					case OptionGroupRequirement.ExactlyOne:
						if (optionCount == 0)
						{
							ReportError(ParseErrorCodes.MissingRequiredOption, CommandLineStrings.OneOfTheOptions0MustBeSpecified, group.GetOptionNamesAsString());
						}
						else if (optionCount > 1)
						{
							ReportError(ParseErrorCodes.IllegalCardinality, CommandLineStrings.OnlyOneOfTheOptions0MayBeSpecified, group.GetOptionNamesAsString());
						}
						break;
					case OptionGroupRequirement.All:
						if (optionCount != group.Options.Count)
						{
							ReportError(ParseErrorCodes.MissingRequiredOption, CommandLineStrings.AllOfTheOptions0MustBeSpecified, group.GetOptionNamesAsString());
						}
						break;
					default:
						throw new InvalidOperationException(CommandLineStrings.InternalErrorNonImplementedGroupRequirementSpecified);
				}
			}
		}
		finally
		{
			_isParsing = false;
		}
		Debug.Assert(_lexerStack.IsEmpty);
	}

	/// <summary>
	/// Sets the escape characters recognized by this parser for escaping characters. 
	/// </summary>
	/// <param name="characters">The escape characters that should be recognized.</param>
	/// <remarks>The only escape character set by default is the backslash ('\') character which is 
	/// pretty much standard for escaping characters. But should so be desired, other characters
	/// can be set here. No sanity checking of this argument is performed though, so be careful 
	/// which characters you chose.</remarks>
	public void SetEscapeCharacters(SCG.IEnumerable<char> characters)
	{
		if (_isParsing)
		{
			throw new InvalidOperationException("Escape characters may not be set while parsing");
		}

		foreach (char ch in characters)
		{
			if (char.IsWhiteSpace(ch) || char.IsLetterOrDigit(ch))
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture,
					"Character '{0}' may not be used as an escape character; only characters not considered white space, letters or digits may be used.", ch));
			}
		}

		_escapeCharacters.Clear();
		_escapeCharacters.AddAll(characters);
	}

	/// <summary>
	/// Adds the specified quotation as a recognized quotation by this parser.
	/// </summary>
	/// <param name="quotationInfo">The quotation info.</param>
	/// <remarks><para>By default only the double quote is recognized as a quotation character, within which 
	/// the double quote itself and the backslash character can be escaped. In unquoted values the 
	/// space character and the backslash can be escaped.</para>
	/// <para>To specify quotations for unquoted strings, specify a <see cref="QuotationInfo"/> with its 
	/// quotation mark set to the null character ('\0').</para></remarks>
	/// <exception cref="ArgumentNullException"><paramref name="quotationInfo"/> was a null reference.</exception>
	/// <exception cref="InvalidOperationException">This method was called during parsing.</exception>
	public void AddQuotation(QuotationInfo quotationInfo)
	{
		ArgumentNullException.ThrowIfNull(quotationInfo);

		if (_isParsing)
		{
			throw new InvalidOperationException("Quotation information must not be changed while parse in progress");
		}

		_quotations.UpdateOrAdd(quotationInfo.QuotationMark, quotationInfo);
	}

	/// <summary>
	/// Removes the quotation with the specified quotation mark from this parser.
	/// </summary>
	/// <param name="quotationMark">The quotation mark representing the quotation to remove.</param>
	/// <exception cref="InvalidOperationException">This method was called during parsing.</exception>
	public void RemoveQuotation(char quotationMark)
	{
		if (_isParsing)
		{
			throw new InvalidOperationException("Quotation information must not be changed while parse in progress");
		}

		_quotations.Remove(quotationMark);
	}

	/// <summary>
	/// Removes all quotations from this parser.
	/// </summary>
	/// <exception cref="InvalidOperationException">This method was called during parsing.</exception>
	public void ClearQuotations()
	{
		if (_isParsing)
		{
			throw new InvalidOperationException("Quotation information must not be changed while parse in progress");
		}

		_quotations.Clear();
	}

	/// <summary>
	/// Adds the available assignment characters for the specified option style.
	/// </summary>
	/// <param name="assignmentCharacter">The assignment character.</param>
	/// <param name="targetStyles">The target styles to which this assignment character will be available.</param>
	/// <remarks>The default assignment characters available are the equal sign '=' for any option style and the 
	/// colon ':' for windows style options.
	/// <note>Adding an assignment character that already exists will <i>overwrite</i> the definition of that
	/// assignment character with this one, so be sure to include all the styles to which you want the 
	/// assignment character to apply to in <paramref name="targetStyles"/></note>
	/// <note>No sanity checks are performed on the assignment characters assigned, so choose these wisely.</note></remarks>
	/// <exception cref="InvalidOperationException">This method was called during parsing.</exception>
	public void AddAssignmentCharacter(char assignmentCharacter, OptionStyles targetStyles)
	{
		if (_isParsing)
		{
			throw new InvalidOperationException(CommandLineStrings.AssignmentCharactersMustNotBeChangedWhileParseInProgress);
		}

		_assignmentCharacters.UpdateOrAdd(assignmentCharacter, targetStyles);
	}

	/// <summary>
	/// Removes the specified assignment character from this instance.
	/// </summary>
	/// <param name="assignmentCharacter">The assignment character to remove.</param>
	/// <exception cref="InvalidOperationException">This method was called during parsing.</exception>
	public void RemoveAssignmentCharacter(char assignmentCharacter)
	{
		if (_isParsing)
		{
			throw new InvalidOperationException(CommandLineStrings.AssignmentCharactersMustNotBeChangedWhileParseInProgress);
		}

		_assignmentCharacters.Remove(assignmentCharacter);
	}

	/// <summary>
	/// Removes all assignment characters from this parser.
	/// </summary>
	public void ClearAssignmentCharacters()
	{
		if (_isParsing)
		{
			throw new InvalidOperationException(CommandLineStrings.AssignmentCharactersMustNotBeChangedWhileParseInProgress);
		}

		_assignmentCharacters.Clear();
	}

	/// <summary>
	/// Gets the quotation info for the specified quotation mark.
	/// </summary>
	/// <param name="quotationMark">The quotation mark.</param>
	/// <returns>a <see cref="QuotationInfo"/> instance describing the quotations for the specified 
	/// quotation mark.</returns>
	public QuotationInfo? GetQuotationInfo(char quotationMark)
	{
		if (!_quotations.Find(ref quotationMark, out QuotationInfo quotationInfo))
		{
			return null;
		}
		return quotationInfo;
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose()
	{
		Dispose(false);
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Matches an option file on the command line, creates a new lexer for that file and pushes
	/// that onto the lexer stack.
	/// </summary>
	private void MatchOptionFile()
	{
		Debug.Assert(LA1 != null);
		Debug.Assert(LA1.TokenType == TokenTypes.OptionFileToken);

		OptionFileToken fileToken = (OptionFileToken)GetNextToken()!;

		Debug.Assert(fileToken.FileName != null);

		try
		{
			Debug.Assert(CurrentLexer != null);
			FileStream fs = new(fileToken.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			PushLexer(new Lexer(new StreamReader(fs), CurrentLexer), fileToken.FileName);
		}
		catch (FileNotFoundException)
		{
			ReportError(ParseErrorCodes.FileNotFound, CommandLineStrings.FileNotFound0, fileToken.FileName);
		}
		catch (ArgumentException)
		{
			ReportError(ParseErrorCodes.FileNotFound, CommandLineStrings.TheSpecifiedPath0IsInvalid, fileToken.FileName);
		}
		catch (DirectoryNotFoundException)
		{
			ReportError(ParseErrorCodes.FileNotFound, CommandLineStrings.TheSpecifiedPath0IsInvalid, fileToken.FileName);
		}
		catch (UnauthorizedAccessException)
		{
			ReportError(ParseErrorCodes.FileNotFound, CommandLineStrings.AccessToTheSpecifiedFile0IsDenied, fileToken.FileName);
		}
		catch (PathTooLongException)
		{
			ReportError(ParseErrorCodes.FileNotFound, CommandLineStrings.TheSpecifiedPath0IsTooLong, fileToken.FileName);
		}
	}

	// Not that complex, just kind of long
	/// <summary>
	/// Matches the name of an option and performs the neccessary callbacks to the option manager to 
	/// set the value of the option.
	/// </summary>
	private void MatchOptionName()
	{
		Debug.Assert(LA1 != null);
		Debug.Assert(LA1.TokenType == TokenTypes.OptionNameToken);

		OptionNameToken optionNameToken = (OptionNameToken)GetNextToken()!;
		string currentName = optionNameToken.Name;
		if (!_options.Find(ref currentName, out IOption option))
		{
			ReportOptionError(ParseErrorCodes.UnknownOption, optionNameToken.Text, CommandLineStrings.UnknownOption0, optionNameToken.Name);

			// Skip an assignment token and value if it follows
			if (LA1 != null && LA1.TokenType == TokenTypes.AssignmentToken)
			{
				SkipTokens(2);
			}

			return;
		}

		// Determine if this option is prohibited by another option already set
		foreach (Option prohibiter in option.ProhibitedBy)
		{
			if (prohibiter.SetCount > 0)
			{
				ReportOptionError(ParseErrorCodes.OptionProhibited, optionNameToken.Text, CommandLineStrings.Option0MayNotBeSpecifiedTogetherWithOption1, option.Name, prohibiter.Name);

				// Skip any assignment following this option
				if (LA1 != null && LA1.TokenType == TokenTypes.AssignmentToken)
				{
					SkipTokens(2);
				}
				else if (option.AcceptsValue && LA1 != null && LA1.TokenType == TokenTypes.ValueToken)
				{
					SkipTokens(1);
				}
				return;
			}
		}

		// Determine whether we need an assignment token 
		if (option.RequireExplicitAssignment && !option.HasDefaultValue && (LA1 == null || LA1.TokenType != TokenTypes.AssignmentToken))
		{
			ReportOptionError(ParseErrorCodes.MissingValue, optionNameToken.Text, CommandLineStrings.MissingRequiredValueForOption0, option.Name);
			// Increase SetCount to avoid additional error about this option not being specified
			option.SetCount++;
			return;
		}

		// Determine whether an explicit assignment is prohibited (bool type not using value)
		if (!option.AcceptsValue && LA1 != null && LA1.TokenType == TokenTypes.AssignmentToken)
		{
			ReportOptionError(ParseErrorCodes.AssignmentToNonValueOption, optionNameToken.Text, CommandLineStrings.Option0DoesNotAcceptAValue, option.Name);
			SkipTokens(1);
			return;
		}

		// Should we set this option to the default value and be done with it?
		if (option.RequireExplicitAssignment && (LA1 == null || LA1.TokenType != TokenTypes.AssignmentToken))
		{
			option.SetDefaultValue();
			return;
		}

		// Now we know that any value that follows should be assigned to this token, so we skip any 
		// following assignment token
		if (LA1 != null && LA1.TokenType == TokenTypes.AssignmentToken)
		{
			SkipTokens(1);
		}

		// Determine whether we require a value
		if (option.RequiresValue && (LA1 == null || LA1.TokenType != TokenTypes.ValueToken))
		{
			ReportOptionError(ParseErrorCodes.MissingValue, optionNameToken.Text, CommandLineStrings.MissingRequiredValueForOption0, option.Name);
			option.SetCount++;
			return;
		}

		// Is this a boolean value with another function than "Value"?
		if (option.IsBooleanType && option.BoolFunction != BoolFunction.Value)
		{
			if (!CheckMaxOccurs(optionNameToken, option))
			{
				return;
			}
			switch (option.BoolFunction)
			{
				case BoolFunction.TrueIfPresent:
					option.Value = true;
					break;
				case BoolFunction.FalseIfPresent:
					option.Value = false;
					break;
				case BoolFunction.UsePrefix:
					if (OptionStyleManager.IsAllEnabled(optionNameToken.OptionStyle, OptionStyles.Plus))
						option.Value = true;
					else
						option.Value = false;
					break;
				default:
					throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture,
						CommandLineStrings.InternalErrorUnimplementedBoolFunction0UsedInOption1, option.BoolFunction.ToString(),
						option.Name));
			}
			return;
		}

		// Check if this option has already been specified the maximum allowed number of times
		if (LA1 != null && LA1.TokenType == TokenTypes.ValueToken)
		{
			ValueToken valueToken = (ValueToken)GetNextToken()!;
			if (!CheckMaxOccurs(optionNameToken, option))
			{
				return;
			}

			try
			{
				try
				{
					option.Value = valueToken.Value;
				}
				catch (TargetInvocationException tie)
				{
					if (tie.InnerException?.GetType() == typeof(FormatException) ||
						tie.InnerException?.GetType() == typeof(OverflowException) ||
						tie.InnerException?.GetType() == typeof(InvalidEnumerationValueException) ||
						tie.InnerException?.GetType() == typeof(InvalidOptionValueException))
					{
						throw tie.InnerException;
					}
					else
					{
						throw;
					}
				}
			}
			catch (FormatException)
			{
				ReportOptionError(ParseErrorCodes.InvalidFormat, option.Name, CommandLineStrings.InvalidValue0ForOption1, valueToken.Value, option.Name);
			}
			catch (OverflowException)
			{
				ReportOptionError(ParseErrorCodes.Overflow, option.Name, CommandLineStrings.ValueFor03OutOfRangeExpectedNumericBetween1And2, option.Name, option.MinValue, option.MaxValue, valueToken.Value);
			}
			catch (InvalidEnumerationValueException)
			{
				// In case 'option' is an alias
				Option definingOption = (Option)option.DefiningOption;

				StringBuilder validValues = new();
				Debug.Assert(definingOption.ValidEnumerationValues != null);

				SCG.IEnumerator<string> iter = definingOption.ValidEnumerationValues.GetEnumerator();
				bool hasNext = iter.MoveNext();
				while (hasNext)
				{
					string currentValue	= iter.Current;
					hasNext				= iter.MoveNext();

					if (hasNext)
					{
						validValues.Append('\"');
						validValues.Append(currentValue);
						validValues.Append("\", ");
					}
					else
					{
						validValues.Append(String.Format(CultureInfo.CurrentUICulture, CommandLineStrings.LastItemOfExclusiveList, currentValue));
					}
				}
				ReportOptionError(ParseErrorCodes.InvalidFormat, option.Name, CommandLineStrings.InvalidValue0ForOption1TheValueMustBeOneOf2, valueToken.Value, option.Name, validValues.ToString());
			}
			catch (InvalidOptionValueException iove)
			{
				StringBuilder errorMessage = new();
				if (iove.IncludeDefaultMessage)
				{
					errorMessage.Append(String.Format(CultureInfo.CurrentUICulture, "The value \"{0}\" is not valid for option \"{1}\"", valueToken.Value, option.Name));
				}

				if (!String.IsNullOrEmpty(iove.Message))
				{
					if (iove.IncludeDefaultMessage)
					{
						errorMessage.Append("; ");
					}

					errorMessage.Append(iove.Message);
				}
				ReportOptionError(ParseErrorCodes.InvalidValue, option.Name, errorMessage.ToString());
			}
		}
	}

	/// <summary>
	/// Checks that no option was specified more times than specified by its MaxOccurs attribute.
	/// </summary>
	/// <param name="optionNameToken">The option name token.</param>
	/// <param name="option">The option.</param>
	/// <returns></returns>
	private bool CheckMaxOccurs(OptionNameToken optionNameToken, IOption option)
	{
		if (option.SetCount >= option.MaxOccurs && option.MaxOccurs > 0)
		{
			if (option.MaxOccurs == 1)
			{
				ReportOptionError(ParseErrorCodes.IllegalCardinality, optionNameToken.Text, CommandLineStrings.Option0MustNotBeSpecifiedMultipleTimes, option.Name);
			}
			else
			{
				ReportOptionError(ParseErrorCodes.IllegalCardinality, optionNameToken.Text, CommandLineStrings.Option0MustNotBeSpecifiedMoreThan1Times, option.Name, option.MaxOccurs);
			}
			return false;
		}
		return true;
	}

	/// <summary>
	/// Adds the specified error to the list of errors.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	/// <param name="optionName">Name of the option.</param>
	/// <param name="message">The message (which may contain the same formatting supported by <see cref="String.Format(string, object)"/>).</param>
	/// <param name="paramList">Additional parameters as described by the message format specifiers.</param>
	private void ReportOptionError(ParseErrorCodes errorCode, string? optionName, string message, params object?[] paramList)
	{
		string formattedMessage = String.Format(CultureInfo.CurrentUICulture, message, paramList);
		ErrorInfo error = new(
			errorCode,
			formattedMessage,
			optionName,
			CurrentFile,
			CurrentLexer == null ? null : (CurrentFile == null ? null : (int?)CurrentLexer.CurrentLine)
		);
		_errors.Add(error);
	}

	/// <summary>
	/// Adds the specified error to the list of errors.
	/// </summary>
	/// <param name="errorCode">The error code.</param>
	/// <param name="message">The message (which may contain the same formatting supported by <see cref="String.Format(string, object)"/>).</param>
	/// <param name="paramList">Additional parameters as described by the message format specifiers.</param>
	private void ReportError(ParseErrorCodes errorCode, string message, params object[] paramList)
	{
		ReportOptionError(errorCode, null, message, paramList);
	}

	/// <summary>
	/// Skips the specified number of tokens from the lexer.
	/// </summary>
	/// <param name="count">The number of lexers to skip.</param>
	/// <returns>true if all the specified tokens were skipped, or false if the end of stream was reached before.</returns>
	private bool SkipTokens(int count)
	{
		while (count-- > 0 && GetNextToken() != null);
		return count == 0;
	}

	/// <summary>
	/// Gets the next token from the current lexer and removes it from the buffer.
	/// </summary>
	/// <returns>the next token from the current lexer</returns>
	private Token? GetNextToken()
	{
		Token? returnToken = LA1;
		LA1Token = null;
		return returnToken;
	}

	/// <summary>
	/// Pops all lexers from the lexer stack that have no more tokens available.
	/// </summary>
	/// <returns>true if there are more lexers available, otherwise; false</returns>
	private bool PopFinishedLexers()
	{
		Debug.Assert(LA1 == null);
		while (!_lexerStack.IsEmpty && LA1 == null)
		{
			_lexerStack.Pop();
		}
		return !_lexerStack.IsEmpty;
	}

	/// <summary>
	/// Pushes the specified lexer onto the lexer stack, hence making it the current lexer.
	/// </summary>
	/// <param name="lexer">The lexer.</param>
	/// <param name="fileName">Name of the file used for input to the lexer, or null if no file name is available.</param>
	private void PushLexer(Lexer lexer, string? fileName)
	{
		_lexerStack.Push(new LexerStackRecord(lexer, fileName));
	}

	/// <summary>
	/// Releases unmanaged and - optionally - managed resources
	/// </summary>
	/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			_escapeCharacters.Dispose();
			_remainingArguments.Dispose();
			if (_lexerStack is ArrayList<LexerStackRecord> list)
			{
				list.Dispose();
			}
			_optionGroups.Dispose();
		}
	}

	#endregion
}