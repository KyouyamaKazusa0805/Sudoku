namespace Sudoku.Platforms.QQ.Modules;

/// <summary>
/// Represents a base type that defines a module that can be called by <see cref="MiraiBot"/> instance.
/// </summary>
/// <seealso cref="MiraiBot"/>
public abstract class GroupModule : IModule
{
	/// <summary>
	/// Indicates <see cref="BindingFlags"/> instance that binds with <see langword="static"/> members.
	/// </summary>
	private const BindingFlags StaticMembers = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;


	/// <summary>
	/// The module raising prefix.
	/// </summary>
	private static readonly string[] ModuleRaisingPrefix = { "!", "！" };

	/// <summary>
	/// Indicates the hint-requested tokens.
	/// </summary>
	private static readonly string[] HintRequestedTokens = { "?", "？" };


	/// <summary>
	/// Indicates the raising command.
	/// </summary>
	protected string RaisingCommand => EqualityContract.GetCustomAttribute<GroupModuleAttribute>()!.Name;

	/// <summary>
	/// Indicates the required environment command.
	/// </summary>
	/// <remarks>
	/// <para>This property is used for controlling the case when it costs too much time to be executed.</para>
	/// <para><i>This property is set <see langword="null"/> by default.</i></para>
	/// </remarks>
	protected string? RequiredEnvironmentCommand
		=> EqualityContract.GetGenericAttributeTypeArguments(typeof(RequiredDependencyModuleAttribute<>)) switch
		{
			[var typeArgument] => typeArgument.GetCustomAttribute<GroupModuleAttribute>()!.Name,
			_ => null
		};

	/// <summary>
	/// Indicates the triggering kind.
	/// </summary>
	protected ModuleTriggeringKind TriggeringKind
		=> EqualityContract.GetCustomAttribute<TriggeringKindAttribute>()?.Kind ?? ModuleTriggeringKind.Default;

	/// <inheritdoc cref="RequiredRoleAttribute.SenderRole"/>
	protected GroupRoleKind RequiredSenderRole
		=> EqualityContract.GetCustomAttribute<RequiredRoleAttribute>()?.SenderRole
		?? GroupRoleKind.God | GroupRoleKind.Owner | GroupRoleKind.Manager | GroupRoleKind.DefaultMember;

	/// <inheritdoc cref="RequiredRoleAttribute.BotRole"/>
	protected GroupRoleKind RequiredBotRole => EqualityContract.GetCustomAttribute<RequiredRoleAttribute>()?.BotRole ?? GroupRoleKind.None;

	/// <summary>
	/// Indicates the equality contract.
	/// </summary>
	protected Type EqualityContract => GetType();

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
		if (text is null || !Array.Exists(ModuleRaisingPrefix, prefix => text.StartsWith($"{prefix}{RaisingCommand}")))
		{
			return;
		}

		if (RunningContexts.TryGetValue(groupId, out var context) && context.ExecutingCommand is { } occupiedCommand
			&& occupiedCommand != RequiredEnvironmentCommand)
		{
			await gmr.SendMessageAsync("本群当前正在执行另外的命令操作。为确保读写数据完全性，机器人暂不允许同时执行别的操作。请等待指令结束后继续操作。");
			return;
		}

		var senderRole = ToGroupRoleKind(sender.Permission);
		var supportedRoles = RequiredSenderRole.GetAllFlags() ?? Array.Empty<GroupRoleKind>();
		if (!Array.Exists(supportedRoles, r => r switch { GroupRoleKind.God => sender.Id == GodNumber, _ => senderRole == r }))
		{
			await gmr.SendMessageAsync("操作失败。该操作需要用户具有更高的权限。");
			return;
		}

		if (RequiredBotRole != GroupRoleKind.None && RequiredBotRole < ToGroupRoleKind(permission))
		{
			await gmr.SendMessageAsync("机器人需要更高权限才可进行该操作。");
			return;
		}

		resetProperties();

		if (!parseMessageCore(text, this, out var failedReason, out var requestedHintArgumentName))
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
				case ParsingFailedReason.TargetPropertyMissingAccessor:
				case ParsingFailedReason.TargetPropertyIsIndexer:
				case ParsingFailedReason.TargetPropertyMissingConverter:
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
			var cachedPropertiesInfo = EqualityContract.GetProperties();
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
						参数 “{argumentName}”：
						  * {hintText}
						"""
				)
			);
		}


		static bool parseMessageCore(
			string @this,
			GroupModule module,
			out ParsingFailedReason failedReason,
			[MaybeNullWhen(false)] out List<string>? requestedHintArgumentName
		)
		{
			requestedHintArgumentName = null;

			var moduleType = module.EqualityContract;

			var isFirstArg = true;
			var args = ParseCommandLine(@this, """[\""“”].+?[\""“”]|[^ ]+""", '"', '“', '”');
			for (var i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				if (isFirstArg)
				{
					if (!Array.Exists(ModuleRaisingPrefix, prefix => arg == $"{prefix}{module.RaisingCommand}"))
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
					failedReason = ParsingFailedReason.TargetPropertyMissingAccessor;
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
							failedReason = ParsingFailedReason.TargetPropertyMissingConverter;
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

		static string[] ParseCommandLine(string s, [StringSyntax(StringSyntaxAttribute.Regex)] string argumentMatcherRegex, params char[]? trimmedCharacters)
			=>
			from match in new Regex(argumentMatcherRegex, RegexOptions.Singleline).Matches(s)
			select match.Value.Trim(trimmedCharacters);

		void resetProperties()
		{
			foreach (var propertyInfo in EqualityContract.GetProperties())
			{
				if (propertyInfo is not { CanRead: true, CanWrite: true, PropertyType: var propType })
				{
					continue;
				}

				if (!propertyInfo.IsDefined(typeof(DoubleArgumentAttribute)))
				{
					continue;
				}

				if (TryGetDefaultValueViaMemberName(propertyInfo, out var defaultValue))
				{
					propertyInfo.SetValue(this, defaultValue);
				}
				else if (TryGetDefaultValue(propertyInfo, out defaultValue))
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
	/// The internal method that executes for the details of an module.
	/// </summary>
	/// <param name="messageReceiver">The group message receiver.</param>
	/// <returns>
	/// A <see cref="Task"/> instance that provides with asynchronous information of the operation currently being executed.
	/// </returns>
	protected abstract Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver);


	/// <summary>
	/// Try to get the default value of the target property specified as <see cref="PropertyInfo"/> reflection data.
	/// </summary>
	/// <param name="pi">The <see cref="PropertyInfo"/> instance.</param>
	/// <param name="defaultValue">The default value of the property.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the specified property has marked the current attribute type.</returns>
	private static bool TryGetDefaultValueViaMemberName(PropertyInfo pi, [MaybeNullWhen(false)] out object? defaultValue)
	{
		switch (pi.GetCustomAttribute<DefaultValueReferencingMemberAttribute>())
		{
			case { DefaultValueInvokerName: var name }:
			{
				foreach (var memberInfo in pi.DeclaringType!.GetMembers(StaticMembers))
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
	/// <param name="pi">The <see cref="PropertyInfo"/> instance.</param>
	/// <param name="defaultValue">The default value of the property.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the specified property has marked the current attribute type.</returns>
	private static bool TryGetDefaultValue(PropertyInfo pi, [MaybeNullWhen(false)] out object? defaultValue)
	{
		var attributeType = typeof(DefaultValueAttribute<>);

		(var @return, defaultValue) = pi.GetCustomGenericAttribute(attributeType) switch
		{
			{ } a => (true, attributeType.MakeGenericType(pi.PropertyType).GetProperty(nameof(DefaultValueAttribute<object>.DefaultValue))!.GetValue(a)!),
			_ => (false, null)
		};

		return @return;
	}

	/// <summary>
	/// Gets the equivalent <see cref="GroupRoleKind"/> instance from the specified <see cref="Permissions"/> instance.
	/// </summary>
	/// <param name="permission">The permission.</param>
	/// <returns>The target <see cref="GroupRoleKind"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static GroupRoleKind ToGroupRoleKind(Permissions permission)
		=> permission switch
		{
			Permissions.Owner => GroupRoleKind.Owner,
			Permissions.Administrator => GroupRoleKind.Manager,
			Permissions.Member => GroupRoleKind.DefaultMember
		};
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
	/// Indicates the operation is failed because target property does not contain both getter and setter.
	/// </summary>
	TargetPropertyMissingAccessor,

	/// <summary>
	/// Indicates the operation is failed because target property is an indexer.
	/// </summary>
	TargetPropertyIsIndexer,

	/// <summary>
	/// Indicates the operation is failed because the target property returns non-<see cref="string"/> type,
	/// but this property has not marked <see cref="ValueConverterAttribute{T}"/>,
	/// causing parser cannot convert the specified value into a <see cref="string"/>.
	/// </summary>
	/// <seealso cref="ValueConverterAttribute{T}"/>
	TargetPropertyMissingConverter,

	/// <summary>
	/// Indicates the operation is failed because user has some invalid input.
	/// </summary>
	InvalidInput
}
