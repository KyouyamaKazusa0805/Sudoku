namespace Sudoku.Platforms.QQ.Modules;

/// <summary>
/// Represents a base type that defines a module that can be called by <see cref="MiraiBot"/> instance.
/// </summary>
/// <seealso cref="MiraiBot"/>
public abstract class GroupModule : IModule
{
	/// <summary>
	/// Indicates all roles are included.
	/// </summary>
	protected const GroupRoleKind AllRoles = GroupRoleKind.God | GroupRoleKind.Owner | GroupRoleKind.Manager | GroupRoleKind.DefaultMember;


	/// <summary>
	/// Indicates the required environment command.
	/// </summary>
	/// <remarks>
	/// <para>This property is used for controlling the case when it costs too much time to be executed.</para>
	/// <para><i>This property is set <see langword="null"/> by default.</i></para>
	/// </remarks>
	public virtual string? RequiredEnvironmentCommand { get; }

	/// <summary>
	/// Indicates the prefix of the command used for raising command exeuction. Assign <see langword="null"/> to tell the runtime
	/// this module does not use any prefixes.
	/// </summary>
	/// <remarks>
	/// You can assign this property with values provided by type <see cref="CommonCommandPrefixes"/>.
	/// </remarks>
	/// <seealso cref="CommonCommandPrefixes"/>
	/// <completionlist cref="CommonCommandPrefixes"/>
	public virtual string[] RaisingPrefix { get; } = CommonCommandPrefixes.Bang;

	/// <summary>
	/// Indicates the supported roles.
	/// The role is used for checking whether the module should be executed if a person emits a command to execute this module.
	/// </summary>
	/// <remarks>
	/// <para>You can use <see cref="GroupRoleKind"/>.<see langword="operator"/> | to merge multiple role kinds into one.</para>
	/// <para><i>By default, the value is all possible roles included.</i></para>
	/// </remarks>
	public virtual GroupRoleKind RequiredSenderRole => AllRoles;

	/// <summary>
	/// Indicates the required bot permission. The kind is only set with one flag, as the highest allowed permission.
	/// </summary>
	/// <remarks><i>
	/// By default, the value is <see cref="GroupRoleKind.None"/>, which means the operation does not require any higher permissions.
	/// </i></remarks>
	/// <seealso cref="GroupRoleKind.None"/>
	public virtual GroupRoleKind RequiredBotRole => GroupRoleKind.None;

	/// <summary>
	/// Indicates the triggering kind.
	/// </summary>
	public virtual ModuleTriggeringKind TriggeringKind => ModuleTriggeringKind.Default;

	/// <summary>
	/// Indicates the raising command.
	/// </summary>
	protected internal string RaisingCommand => GetType().GetCustomAttribute<GroupModuleAttribute>()!.Name;

	/// <inheritdoc/>
	bool? IModule.IsEnable { get; set; } = true;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when properties overridden by derived type is invalid.</exception>
	public async void Execute(MessageReceiverBase @base)
	{
		if (!RequiredBotRole.IsFlag())
		{
			throw new InvalidOperationException("This module contains multiple highest allowed permission kinds.");
		}

		if (@base is not GroupMessageReceiver
			{
				BotPermission: var permission,
				MessageChain: var messageChain,
				Sender: var sender,
				GroupId: var groupId
			} gmr)
		{
			return;
		}

		var text = (messageChain, TriggeringKind) switch
		{
			([SourceMessage, AtMessage { Target: BotNumber }, PlainMessage { Text: var t }], ModuleTriggeringKind.Mentioning) => t,
			([SourceMessage, PlainMessage { Text: var t }], ModuleTriggeringKind.Default) => t,
			_ => null
		};
		if (text is null || !Array.Exists(RaisingPrefix, prefix => text.StartsWith($"{prefix}{RaisingCommand}")))
		{
			return;
		}

		if (RunningContexts.TryGetValue(groupId, out var context) && context.ExecutingCommand is { } occupiedCommand
			&& occupiedCommand != RequiredEnvironmentCommand)
		{
			await gmr.SendMessageAsync("本群当前正在执行另外的命令操作。为确保读写数据完全性，机器人暂不允许同时执行别的操作。请等待指令结束后继续操作。");
			return;
		}

		var senderRole = sender.Permission.ToGroupRoleKind();
		var supportedRoles = RequiredSenderRole.GetAllFlags() ?? Array.Empty<GroupRoleKind>();
		if (!Array.Exists(supportedRoles, r => r switch { GroupRoleKind.God => sender.Id == GodNumber, _ => senderRole == r }))
		{
			await gmr.SendMessageAsync("操作失败。该操作需要用户具有更高的权限。");
			return;
		}

		if (RequiredBotRole != GroupRoleKind.None && RequiredBotRole < permission.ToGroupRoleKind())
		{
			await gmr.SendMessageAsync("机器人需要更高权限才可进行该操作。");
			return;
		}

		ResetProperties();

		if (!text.ParseMessageTo(this, out var failedReason, out var requestedHintArgumentName))
		{
			switch (failedReason)
			{
				case ParsingFailedReason.InvalidInput:
				{
					await gmr.SendMessageAsync("请检查指令输入是否正确。尤其是缺少空格。空格作为指令识别期间较为重要的分隔符号，请勿缺少。");
					return;
				}
				case ParsingFailedReason.TargetPropertyNotFound:
				{
					await gmr.SendMessageAsync(
						"你输入的指令有误，导致你要具体指定的参数信息不能成功匹配。请使用完整的指令，不要省略一些固定词语，如“！购买 物品 强化卡”的“物品”。"
					);

					return;
				}
				case ParsingFailedReason.NotCurrentModule:
				{
					return;
				}
				case ParsingFailedReason.TargetPropertyHasNoGetterOrSetter:
				case ParsingFailedReason.TargetPropertyIsIndexer:
				case ParsingFailedReason.TargetPropertyHasNoValueConverterIfNotReturningString:
				{
					throw new InvalidOperationException($"Attribute marking is invalid. Internal failed reason: {failedReason}");
				}
				default:
				{
					throw new InvalidOperationException("The specified failed reason is not defined.");
				}
			}
		}

		if (requestedHintArgumentName is null)
		{
			await ExecuteCoreAsync(gmr);
		}
		else
		{
			var cachedPropertiesInfo = GetType().GetProperties();
			await gmr.SendMessageAsync(
				string.Join(
					$"{Environment.NewLine}{Environment.NewLine}",
					from argumentName in requestedHintArgumentName
					let chosenPropertyInfo = (
						from pi in cachedPropertiesInfo
						where pi.GetCustomAttribute<DoubleArgumentAttribute>()?.Name == argumentName
						select pi
					).FirstOrDefault()
					where chosenPropertyInfo is not null
					let hintText = chosenPropertyInfo.GetCustomAttribute<HintAttribute>()?.Hint ?? "<暂无介绍信息>"
					select
						$"""
						参数 “{argumentName}” 的提示：
						  * {hintText}
						"""
				)
			);
		}
	}

	/// <summary>
	/// The internal method that executes for the details of an module.
	/// </summary>
	/// <param name="messageReceiver">The group message receiver.</param>
	/// <returns>
	/// A <see cref="Task"/> instance that provides with asynchronous information of the operation currently being executed.
	/// </returns>
	protected abstract Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver);

	/// <summary>
	/// To reset properties.
	/// </summary>
	protected virtual void ResetProperties()
	{
		var containingType = GetType();
		foreach (var propertyInfo in containingType.GetProperties())
		{
			if (propertyInfo is not { CanRead: true, CanWrite: true, PropertyType: var propType })
			{
				continue;
			}

			if (!propertyInfo.IsDefined(typeof(DoubleArgumentAttribute)))
			{
				continue;
			}

			if (propertyInfo.TryGetDefaultValueViaMemberName(out var defaultValue))
			{
				propertyInfo.SetValue(this, defaultValue);
			}
			else if (propertyInfo.TryGetDefaultValue(out defaultValue))
			{
				propertyInfo.SetValue(this, defaultValue);
			}
			else
			{
				propertyInfo.SetValue(this, DefaultValueCreator.CreateInstance(propType));
			}
		}
	}
}

/// <summary>
/// Indicates the failed reason while parse a message text.
/// </summary>
file enum ParsingFailedReason : int
{
	/// <summary>
	/// Indicates the operation is not failed.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the operation is failed because the parser has detected that the message text is not used by this module.
	/// </summary>
	NotCurrentModule,

	/// <summary>
	/// Indicates the operation is failed because target property is not found.
	/// </summary>
	TargetPropertyNotFound,

	/// <summary>
	/// Indicates the operation is failed because target property has no both getter and setter.
	/// </summary>
	TargetPropertyHasNoGetterOrSetter,

	/// <summary>
	/// Indicates the operation is failed because target property is an indexer.
	/// </summary>
	TargetPropertyIsIndexer,

	/// <summary>
	/// Indicates the operation is failed
	/// and returns non-<see cref="string"/> type, but this property has not marked <see cref="ValueConverterAttribute{T}"/>,
	/// which cause parser cannot convert the specified value into a <see cref="string"/>.
	/// </summary>
	/// <seealso cref="ValueConverterAttribute{T}"/>
	TargetPropertyHasNoValueConverterIfNotReturningString,

	/// <summary>
	/// Indicates the operation is failed because user has some invalid input.
	/// </summary>
	InvalidInput
}

/// <summary>
/// Defines a message parser type. This type provides a way to parse plain text into separated values
/// used by <see cref="IModule"/> instances.
/// </summary>
/// <seealso cref="IModule"/>
file static class MessageParser
{
	/// <summary>
	/// Indicates <see cref="BindingFlags"/> instance that binds with <see langword="static"/> members.
	/// </summary>
	private const BindingFlags StaticMembers = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;


	/// <summary>
	/// Indicates the hint-requested tokens.
	/// </summary>
	private static readonly string[] HintRequestedTokens = new[] { "？", "?" };


	/// <summary>
	/// Try to parse message, and assign converted message data into target <see cref="GroupModule"/> instance.
	/// </summary>
	/// <param name="this">The plain message to be parsed.</param>
	/// <param name="module">The module instance.</param>
	/// <param name="failedReason">Indicates the failed reason that is encountered.</param>
	/// <param name="requestedHintArgumentName">Indicates the requested hint argument name.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the specified plain message is compatible
	/// with module specified as argument <paramref name="module"/>.
	/// </returns>
	public static bool ParseMessageTo(
		this string @this,
		GroupModule module,
		out ParsingFailedReason failedReason,
		[MaybeNullWhen(false)] out List<string>? requestedHintArgumentName
	)
	{
		requestedHintArgumentName = null;

		var moduleType = module.GetType();

		var isFirstArg = true;
		var args = @this.ParseCommandLine("""[\""“”].+?[\""“”]|[^ ]+""", '"', '“', '”');
		for (var i = 0; i < args.Length; i++)
		{
			var arg = args[i];
			if (isFirstArg)
			{
				var commandFound = module switch
				{
					{ RaisingPrefix: { } prefixes, RaisingCommand: var cmd } => Array.Exists(prefixes, prefix => arg == $"{prefix}{cmd}"),
					{ RaisingCommand: var cmd } => arg == cmd
				};
				if (!commandFound)
				{
					failedReason = ParsingFailedReason.NotCurrentModule;
					return false;
				}

				isFirstArg = false;
				continue;
			}

			var foundPropertyInfo = default(PropertyInfo?);
			foreach (var tempPropertyInfo in moduleType.GetProperties())
			{
				if (tempPropertyInfo.GetCustomAttribute<DoubleArgumentAttribute>() is { Name: var name } && name == arg)
				{
					foundPropertyInfo = tempPropertyInfo;
					break;
				}
			}

			if (foundPropertyInfo is null)
			{
				failedReason = ParsingFailedReason.TargetPropertyNotFound;
				return false;
			}

			if (foundPropertyInfo is not { CanRead: true, CanWrite: true })
			{
				failedReason = ParsingFailedReason.TargetPropertyHasNoGetterOrSetter;
				return false;
			}

			if (foundPropertyInfo.GetIndexParameters().Length != 0)
			{
				failedReason = ParsingFailedReason.TargetPropertyIsIndexer;
				return false;
			}

			if (i + 1 >= args.Length)
			{
				failedReason = ParsingFailedReason.InvalidInput;
				return false;
			}

			var nextArg = args[i + 1];
			switch (
				foundPropertyInfo.GetGenericAttributeTypeArguments(typeof(ValueConverterAttribute<>)),
				Array.Exists(HintRequestedTokens, e => nextArg == e)
			)
			{
				case ([], false):
				{
					if (foundPropertyInfo.PropertyType != typeof(string))
					{
						failedReason = ParsingFailedReason.TargetPropertyHasNoValueConverterIfNotReturningString;
						return false;
					}

					foundPropertyInfo.SetValue(module, nextArg);

					break;
				}
				case ([var valueConverterType], false):
				{
					try
					{
						var instance = (IValueConverter)Activator.CreateInstance(valueConverterType)!;
						var targetConvertedValue = instance.Convert(nextArg);
						foundPropertyInfo.SetValue(module, targetConvertedValue);
					}
					catch (CommandConverterException)
					{
						failedReason = ParsingFailedReason.InvalidInput;
						return false;
					}

					break;
				}
				case (_, true):
				{
					(requestedHintArgumentName ??= new()).Add(args[i]);

					break;
				}
			}

			i++;
		}

		failedReason = ParsingFailedReason.None;
		return true;
	}

	/// <summary>
	/// Try to get the default value of the target property specified as <see cref="PropertyInfo"/> reflection data.
	/// </summary>
	/// <param name="this">The <see cref="PropertyInfo"/> instance.</param>
	/// <param name="defaultValue">The default value of the property.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the specified property has marked the current attribute type.</returns>
	public static bool TryGetDefaultValueViaMemberName(this PropertyInfo @this, [MaybeNullWhen(false)] out object? defaultValue)
	{
		switch (@this.GetCustomAttribute<DefaultValueReferencingMemberAttribute>())
		{
			case { DefaultValueInvokerName: var name }:
			{
				foreach (var memberInfo in @this.DeclaringType!.GetMembers(StaticMembers))
				{
					switch (memberInfo)
					{
						case FieldInfo { Name: var fieldName } f when fieldName == name:
						{
							defaultValue = f.GetValue(null);
							return true;
						}
						case PropertyInfo { CanRead: true, Name: var propertyName } p when propertyName == name:
						{
							defaultValue = p.GetValue(null, null);
							return true;
						}
						case MethodInfo { ReturnType: var returnType, Name: var methodName } m
						when methodName == name && returnType != typeof(void):
						{
							var parameters = m.GetParameters();
							if (parameters.Length != 0)
							{
								continue;
							}

							defaultValue = m.Invoke(null, null);
							return true;
						}
					}
				}

				goto default;
			}
			default:
			{
				defaultValue = null;
				return false;
			}
		}
	}

	/// <summary>
	/// Try to get the default value of the target property specified as <see cref="PropertyInfo"/> reflection data.
	/// </summary>
	/// <param name="this">The <see cref="PropertyInfo"/> instance.</param>
	/// <param name="defaultValue">The default value of the property.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the specified property has marked the current attribute type.</returns>
	public static bool TryGetDefaultValue(this PropertyInfo @this, [MaybeNullWhen(false)] out object? defaultValue)
	{
		(var @return, defaultValue) = @this.GetCustomGenericAttribute(typeof(DefaultValueAttribute<>)) switch
		{
			{ } attribute
				=> (
					true,
					typeof(DefaultValueAttribute<>).MakeGenericType(@this.PropertyType)
						.GetProperty(nameof(DefaultValueAttribute<object>.DefaultValue))!
						.GetValue(attribute)!
				),
			_ => (false, null)
		};

		return @return;
	}

	/// <summary>
	/// Parses a line of command, separating the command line into multiple <see cref="string"/> arguments using spaces and quotes.
	/// </summary>
	/// <param name="this">The command line.</param>
	/// <param name="argumentMatcherRegex">Indicates the custom argument matcher regular expression.</param>
	/// <param name="trimmedCharacters">Indicates the trimmed characters.</param>
	/// <returns>Parsed arguments, represented as an array of <see cref="string"/> values.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string[] ParseCommandLine(
		this string @this,
		[StringSyntax(StringSyntaxAttribute.Regex)] string argumentMatcherRegex,
		params char[]? trimmedCharacters
	) => from match in new Regex(argumentMatcherRegex, RegexOptions.Singleline).Matches(@this) select match.Value.Trim(trimmedCharacters);

	/// <summary>
	/// Gets the equivalent <see cref="GroupRoleKind"/> instance from the specified <see cref="Permissions"/> instance.
	/// </summary>
	/// <param name="permission">The permission.</param>
	/// <returns>The target <see cref="GroupRoleKind"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GroupRoleKind ToGroupRoleKind(this Permissions permission)
		=> permission switch
		{
			Permissions.Owner => GroupRoleKind.Owner,
			Permissions.Administrator => GroupRoleKind.Manager,
			Permissions.Member => GroupRoleKind.DefaultMember
		};
}
