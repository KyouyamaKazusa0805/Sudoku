namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 提供一个基本的指令模块。这个类型为抽象类型，需要派生出实例类型进行实现，并提供给 <see cref="MiraiBot"/> 实例调用。
/// </summary>
/// <seealso cref="MiraiBot"/>
public abstract class Command : IModule
{
	/// <summary>
	/// 用于反射期间绑定的 <see cref="BindingFlags"/> 类型的实例。
	/// </summary>
	private const BindingFlags StaticMembers = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;


	/// <summary>
	/// 表示模块触发的前缀符号。
	/// </summary>
	private static readonly string[] ModuleTriggeringPrefix = { "!", "！" };

	/// <summary>
	/// 表示模块查询参数或模块自身提示信息的占位符。
	/// </summary>
	private static readonly string[] HintRequestedTokens = { "?", "？" };


	/// <summary>
	/// 表示引发指令的名称，不带前缀符号。比如“！签到”的“签到”。
	/// </summary>
	protected string RaisingCommand => EqualityContract.GetCustomAttribute<CommandAttribute>()!.Name;

	/// <summary>
	/// 表示该指令需要依赖的指令名称。比如说游戏结束必须至少要求游戏开始。那么“！结束游戏”指令必须依赖于“！开始游戏”指令触发之后。
	/// 这个属性对于“结束游戏”指令的实现类型来说，它的依赖指令名就是“开始游戏”。如果类型不依赖于任何其他的指令，这个属性则会保持 <see langword="null"/> 值。
	/// </summary>
	protected string? RequiredEnvironmentCommand
		=> EqualityContract.GetGenericAttributeTypeArguments(typeof(DependencyModuleAttribute<>)) switch
		{
			[var typeArgument] => typeArgument.GetCustomAttribute<CommandAttribute>()!.Name,
			_ => null
		};

	/// <inheritdoc cref="RequiredRoleAttribute.SenderRole"/>
	protected GroupRoleKind RequiredSenderRole
		=> EqualityContract.GetCustomAttribute<RequiredRoleAttribute>()?.SenderRole
		?? GroupRoleKind.God | GroupRoleKind.Owner | GroupRoleKind.Manager | GroupRoleKind.DefaultMember;

	/// <inheritdoc cref="RequiredRoleAttribute.BotRole"/>
	protected GroupRoleKind RequiredBotRole => EqualityContract.GetCustomAttribute<RequiredRoleAttribute>()?.BotRole ?? GroupRoleKind.None;

	/// <summary>
	/// 为 <see cref="object.GetType"/> 的快捷调用。由于体系架构使用反射，因此该属性使用频率会非常高。
	/// </summary>
	protected Type EqualityContract => GetType();

	/// <inheritdoc/>
	bool? IModule.IsEnable { get; set; } = true;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">如果实现类型存在一些问题，就会产生此异常。</exception>
	public async void Execute(MessageReceiverBase @base)
	{
		if (!RequiredBotRole.IsFlag())
		{
			throw new InvalidOperationException("机器人无法处理该指令，因为机器人权限设置有问题：它只能是单 flag 的值。");
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

		var text = messageChain switch { [SourceMessage, PlainMessage { Text: var t }] => t, _ => null };
		if (text is null || !Array.Exists(ModuleTriggeringPrefix, prefix => text.StartsWith($"{prefix}{RaisingCommand}")))
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
					throw new InvalidOperationException($"目标属性存在解析异常，内部错误：{failedReason}");
				}
				default:
				{
					throw new InvalidOperationException("存在解析异常，但是属于其他情况。请打开调试器进行调试，了解错误来源。");
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
			Command module,
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
					if (!Array.Exists(ModuleTriggeringPrefix, prefix => arg == $"{prefix}{module.RaisingCommand}"))
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
	/// 用于专门处理机器人对该指令的理解。
	/// </summary>
	/// <param name="messageReceiver">机器人在群组里进行回复操作期间，会使用到的对象。</param>
	/// <returns>
	/// 返回一个 <see cref="Task"/> 实例，记录了异步执行期间的一些数据信息。
	/// </returns>
	protected abstract Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver);


	/// <summary>
	/// 通过 <see cref="PropertyInfo"/> 反射到的属性信息，进行该属性常量的求得。
	/// 如果属性标记了 <see cref="DefaultValueReferencingMemberAttribute"/> 特性，并且指向的成员合法，那么就会返回合理的结果到
	/// 参数 <paramref name="defaultValue"/> 上，并返回 <see langword="true"/>；否则 <see langword="false"/>。
	/// </summary>
	/// <param name="pi"><see cref="PropertyInfo"/> 类型的实例。</param>
	/// <param name="defaultValue">从外部接收使用的默认数值结果。该参数只用于返回值是 <see langword="true"/> 的时候。</param>
	/// <returns>一个 <see cref="bool"/> 结果，表示是否 <see cref="DefaultValueReferencingMemberAttribute"/> 特性标记合理。</returns>
	/// <seealso cref="DefaultValueReferencingMemberAttribute"/>
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
	/// 通过 <see cref="PropertyInfo"/> 反射到的属性信息，进行该属性常量的求得。
	/// 如果属性标记了 <see cref="DefaultValueAttribute{T}"/> 特性，那么就会返回合理的结果到
	/// 参数 <paramref name="defaultValue"/> 上，并返回 <see langword="true"/>；否则 <see langword="false"/>。
	/// </summary>
	/// <param name="pi"><inheritdoc cref="TryGetDefaultValueViaMemberName(PropertyInfo, out object?)" path="/param[@name='pi']"/></param>
	/// <param name="defaultValue">
	/// <inheritdoc cref="TryGetDefaultValueViaMemberName(PropertyInfo, out object?)" path="/param[@name='defaultValue']"/>
	/// </param>
	/// <returns>一个 <see cref="bool"/> 结果，表示是否 <see cref="DefaultValueAttribute{T}"/> 特性标记合理。</returns>
	/// <seealso cref="DefaultValueAttribute{T}"/>
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
	/// 一个转换类型，将 <see cref="Permissions"/> 实例转换为等同的 <see cref="GroupRoleKind"/> 实例。
	/// </summary>
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
/// 一个本地枚举类型，表示一个在解析期间产生的错误信息。
/// </summary>
file enum ParsingFailedReason : int
{
	/// <summary>
	/// 表示没有错误。
	/// </summary>
	None,

	/// <summary>
	/// 表示解析失败，因为解析的指令不属于当前指令（不匹配）。
	/// </summary>
	NotCurrentModule,

	/// <summary>
	/// 表示解析失败，因为目标属性没有找到。
	/// </summary>
	TargetPropertyNotFound,

	/// <summary>
	/// 表示解析失败，因为目标属性缺少 <see langword="get"/> 或 <see langword="set"/> 方法的至少一个。
	/// </summary>
	TargetPropertyMissingAccessor,

	/// <summary>
	/// 表示解析失败，因为目标属性是索引器（有参属性）。
	/// </summary>
	TargetPropertyIsIndexer,

	/// <summary>
	/// 表示解析失败，因为目标属性不是 <see cref="string"/> 类型，却缺少 <see cref="ValueConverterAttribute{T}"/> 的转换指示情况。
	/// </summary>
	/// <seealso cref="ValueConverterAttribute{T}"/>
	TargetPropertyMissingConverter,

	/// <summary>
	/// 表示解析失败，因为用户输入的结果在转换期间失败。比如某处要求输入整数，结果输入了别的无法转为整数数据的结果，例如字母。
	/// </summary>
	InvalidInput
}
