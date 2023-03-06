namespace Sudoku.Platforms.QQ.Modules;

/// <summary>
/// Represents a base type that defines a module that can be called by <see cref="MiraiBot"/> instance.
/// </summary>
public abstract partial class GroupModule : IModule
{
	/// <summary>
	/// Indicates all roles are included.
	/// </summary>
	protected const GroupRoleKind AllRoles = GroupRoleKind.GodAccount | GroupRoleKind.Owner | GroupRoleKind.Manager | GroupRoleKind.DefaultMember;


	/// <summary>
	/// Indicates whether the module is currently enabled. If disabled (i.e. holding with <see langword="false"/> value),
	/// this option won't be executed even if a person with a supported role emits a command executing this module.
	/// </summary>
	/// <remarks><i>This property is set <see langword="true"/> by default.</i></remarks>
	[Reserved]
	public virtual bool IsEnable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((IModule)this).IsEnable ?? false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => ((IModule)this).IsEnable = value;
	}

	/// <summary>
	/// Indicates the raising command.
	/// </summary>
	[Reserved]
	public abstract string RaisingCommand { get; }

	/// <summary>
	/// Indicates the required environment command.
	/// </summary>
	/// <remarks>
	/// <para>This property is used for controlling the case when it costs too much time to be executed.</para>
	/// <para><i>This property is set <see langword="null"/> by default.</i></para>
	/// </remarks>
	[Reserved]
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
	[Reserved]
	public virtual string[]? RaisingPrefix { get; } = CommonCommandPrefixes.Bang;

	/// <summary>
	/// Indicates the supported roles.
	/// The role is used for checking whether the module should be executed if a person emits a command to execute this module.
	/// </summary>
	/// <remarks>
	/// <para>You can use <see cref="GroupRoleKind"/>.<see langword="operator"/> | to merge multiple role kinds into one.</para>
	/// <para><i>By default, the value is all possible roles included.</i></para>
	/// </remarks>
	[Reserved]
	public virtual GroupRoleKind SupportedRoles => AllRoles;

	/// <summary>
	/// Indicates the required bot permission. The kind is only set with one flag, as the highest allowed permission.
	/// </summary>
	/// <remarks><i>
	/// By default, the value is <see cref="GroupRoleKind.None"/>, which means the operation does not require any higher permissions.
	/// </i></remarks>
	/// <seealso cref="GroupRoleKind.None"/>
	[Reserved]
	public virtual GroupRoleKind RequiredBotRole => GroupRoleKind.None;

	/// <summary>
	/// Indicates the triggering kind.
	/// </summary>
	[Reserved]
	public virtual ModuleTriggeringKind TriggeringKind => ModuleTriggeringKind.Default;

	/// <inheritdoc/>
	[Reserved]
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

		if (RunningContexts.TryGetValue(groupId, out var context) && context.ExecutingCommand is { } occupiedCommand
			&& occupiedCommand != RequiredEnvironmentCommand)
		{
#if false
			await gmr.SendMessageAsync("本群当前正在执行另外的命令操作。为确保读写数据完全性，机器人暂不允许同时执行别的操作。请等待指令结束后继续操作。");
#endif
			return;
		}

		var senderRole = sender.Permission.ToGroupRoleKind();
		var supportedRoles = SupportedRoles.GetAllFlags() ?? Array.Empty<GroupRoleKind>();
		if (!Array.Exists(supportedRoles, match))
		{
#if false
			await gmr.SendMessageAsync("操作失败。该操作需要用户具有更高的权限。");
#endif
			return;
		}

		if (RequiredBotRole != GroupRoleKind.None && RequiredBotRole < permission.ToGroupRoleKind())
		{
#if false
			await gmr.SendMessageAsync("机器人需要更高权限才可进行该操作。");
#endif
			return;
		}

		_ = (messageChain, TriggeringKind) switch
		{
			([SourceMessage, AtMessage { Target: BotNumber }, PlainMessage { Text: var text }], ModuleTriggeringKind.Mentioning)
				=> await handleMessageTextAsync(text),
			([SourceMessage, PlainMessage { Text: var text }], ModuleTriggeringKind.Default)
				=> await handleMessageTextAsync(text),
			_
				=> false
		};


		bool match(GroupRoleKind roleKind)
			=> roleKind != GroupRoleKind.GodAccount && senderRole == roleKind
			|| roleKind == GroupRoleKind.GodAccount && sender.Id == GodNumber;

		async Task<bool> handleMessageTextAsync(string messageText)
		{
			if (!messageText.ParseMessageTo(this, out var failedReason))
			{
				switch (failedReason)
				{
					case ParsingFailedReason.InvalidInput:
					{
						await gmr.SendMessageAsync("请检查指令输入是否正确。尤其是缺少空格。空格作为指令识别期间较为重要的分隔符号，请勿缺少。");
						return false;
					}
					case ParsingFailedReason.TargetPropertyIsReserved:
					{
						return false;
					}
					case ParsingFailedReason.NotCurrentModule:
					case ParsingFailedReason.None:
					{
						return true;
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

			await ExecuteCoreAsync(gmr);
			return true;
		}
	}

	/// <summary>
	/// The internal method that executes for the details of an module.
	/// </summary>
	/// <param name="groupMessageReceiver">The group message receiver.</param>
	/// <returns>
	/// A <see cref="Task"/> instance that provides with asynchronous information of the operation currently being executed.
	/// </returns>
	protected abstract Task ExecuteCoreAsync(GroupMessageReceiver groupMessageReceiver);
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
	/// Indicates the operation is failed because target property has no both getter and setter.
	/// </summary>
	TargetPropertyHasNoGetterOrSetter,

	/// <summary>
	/// Indicates the operation is failed because target property is an indexer.
	/// </summary>
	TargetPropertyIsIndexer,

	/// <summary>
	/// Indicates the operation is failed because target property is reserved one, it will be skipped in the assignment.
	/// </summary>
	TargetPropertyIsReserved,

	/// <summary>
	/// Indicates the operation is failed
	/// and returns non-<see cref="string"/> type, but this property has not marked <see cref="ArgumentValueConverterAttribute{T}"/>,
	/// which cause parser cannot convert the specified value into a <see cref="string"/>.
	/// </summary>
	/// <seealso cref="ArgumentValueConverterAttribute{T}"/>
	TargetPropertyHasNoValueConverterIfNotReturningString,

	/// <summary>
	/// Indicates the operation is failed because user has some invalid input.
	/// </summary>
	InvalidInput
}

/// <summary>
/// Represents an attribute type that describes the property is reserved one.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
file sealed class ReservedAttribute : Attribute
{
}

/// <summary>
/// Defines a message parser type. This type provides a way to parse plain text into separated values
/// used by <see cref="IModule"/> instances.
/// </summary>
/// <seealso cref="IModule"/>
file static class MessageParser
{
	/// <summary>
	/// Try to parse message, and assign converted message data into target <see cref="GroupModule"/> instance.
	/// </summary>
	/// <param name="this">The plain message to be parsed.</param>
	/// <param name="module">The module instance.</param>
	/// <param name="failedReason">Indicates the failed reason that is encountered.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the specified plain message is compatible
	/// with module specified as argument <paramref name="module"/>.
	/// </returns>
	public static bool ParseMessageTo(this string @this, GroupModule module, out ParsingFailedReason failedReason)
	{
		var moduleType = module.GetType();

		var isFirstArg = true;
		var args = MetaParser.Parse(@this, """[\""“”].+?[\""“”]|[^ ]+""");
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

			if (foundPropertyInfo is not { CanRead: true, CanWrite: true })
			{
				failedReason = ParsingFailedReason.TargetPropertyHasNoGetterOrSetter;
				return false;
			}

			if (foundPropertyInfo.GetIndexParameters().Length == 0)
			{
				failedReason = ParsingFailedReason.TargetPropertyIsIndexer;
				return false;
			}

			if (foundPropertyInfo.IsDefined(typeof(ReservedAttribute)))
			{
				failedReason = ParsingFailedReason.TargetPropertyIsReserved;
				return false;
			}

			if (i + 1 >= args.Length)
			{
				failedReason = ParsingFailedReason.InvalidInput;
				return false;
			}

			var nextArg = args[i + 1];
			switch (foundPropertyInfo.GetGenericAttributeTypeArguments(typeof(ArgumentValueConverterAttribute<>)))
			{
				case { Length: 0 }:
				{
					if (foundPropertyInfo.PropertyType != typeof(string))
					{
						failedReason = ParsingFailedReason.TargetPropertyHasNoValueConverterIfNotReturningString;
						return false;
					}

					foundPropertyInfo.SetValue(module, nextArg);

					break;
				}
				case [var valueConverterType]:
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
			}
		}

		failedReason = ParsingFailedReason.None;
		return true;
	}
}

/// <summary>
/// Provides with a local converter type.
/// </summary>
file static class LocalConverter
{
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
