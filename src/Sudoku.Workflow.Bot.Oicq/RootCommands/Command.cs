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
	public string Name => EqualityContract.GetCustomAttribute<CommandAttribute>()!.Name;

	/// <summary>
	/// 为 <see cref="object.GetType"/> 的快捷调用。由于体系架构使用反射，因此该属性使用频率会非常高。
	/// </summary>
	protected Type EqualityContract => GetType();

	/// <summary>
	/// 表示用户需要至少满足多少级别的时候才可使用的指令。只要用户等级大于或等于该级别的时候，则可以使用该指令，否则将产生错误信息的提示。
	/// </summary>
	private int RequiredUserLevel => EqualityContract.GetCustomAttribute<RequiredUserLevelAttribute>()?.RequiredUserLevel ?? 0;

	/// <summary>
	/// 表示该指令需要依赖的指令名称。比如说游戏结束必须至少要求游戏开始。那么“！结束游戏”指令必须依赖于“！开始游戏”指令触发之后。
	/// 这个属性对于“结束游戏”指令的实现类型来说，它的依赖指令名就是“开始游戏”。如果类型不依赖于任何其他的指令，这个属性则会保持 <see langword="null"/> 值。
	/// </summary>
	private string? RequiredEnvironmentCommand
		=> EqualityContract.GetGenericAttributeTypeArguments(typeof(DependencyCommandAttribute<>)) switch
		{
			[var typeArgument] => typeArgument.GetCustomAttribute<CommandAttribute>()!.Name,
			_ => null
		};

	/// <inheritdoc cref="RequiredRoleAttribute.SenderRole"/>
	private GroupRoleKind RequiredSenderRole
		=> EqualityContract.GetCustomAttribute<RequiredRoleAttribute>()?.SenderRole
		?? GroupRoleKind.God | GroupRoleKind.Owner | GroupRoleKind.Manager | GroupRoleKind.DefaultMember;

	/// <inheritdoc cref="RequiredRoleAttribute.BotRole"/>
	private GroupRoleKind RequiredBotRole => EqualityContract.GetCustomAttribute<RequiredRoleAttribute>()?.BotRole ?? GroupRoleKind.None;

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
				Sender: { Id: var senderId } sender,
				GroupId: var groupId
			} gmr)
		{
			return;
		}

		var text = messageChain switch { [SourceMessage, PlainMessage { Text: var t }] => t, _ => null };
		if (text is null || !Array.Exists(ModuleTriggeringPrefix, prefix => text.StartsWith($"{prefix}{Name}")))
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

		if (RequiredUserLevel != 0
			&& (UserOperations.Read(senderId) is not { ExperiencePoint: var exp } || ScoringOperation.GetGrade(exp) < RequiredUserLevel))
		{
			await gmr.SendMessageAsync($"抱歉，该至少需要用户达到 {RequiredUserLevel} 级才可使用。");
			return;
		}

		resetProperties();

		if (!ParseMessageCore(text, this, out var failedReason, out var requestedHintArgumentName, out var requestedCommandHint))
		{
			await gmr.SendMessageAsync(
				failedReason switch
				{
					ParsingFailedReason.InvalidInput => "请检查指令输入是否正确。尤其是缺少空格。空格作为指令识别期间较为重要的分隔符号，请勿缺少。",
					ParsingFailedReason.PropertyNotFound => "输入的指令有误，导致参数信息不匹配。请不要省略一些固定词语，如“！购买 物品 强化卡”的“物品”。",
					ParsingFailedReason.PropertyMissingAccessor or ParsingFailedReason.PropertyIsIndexer or ParsingFailedReason.PropertyMissingConverter
						=> throw new InvalidOperationException($"Internal error: {failedReason}"),
					_ => throw new InvalidOperationException("Other invalid cases on parsing.")
				}
			);
			return;
		}

		switch (requestedHintArgumentName, requestedCommandHint)
		{
			// ！指令 参数 ？
			case (not null, _):
			{
				var propertiesInfo = EqualityContract.GetProperties();
				await gmr.SendMessageAsync(
					string.Join(
						$"{Environment.NewLine}{Environment.NewLine}",
						from argumentName in requestedHintArgumentName
						let chosenPropertyInfo = (
							from pi in propertiesInfo
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

				break;
			}

			// ！指令 ？
			case (_, true):
			{
				var propertiesInfo = EqualityContract.GetProperties();
				var indexedDictionary = new Dictionary<int, List<PropertyInfo>>();
				foreach (var propertyInfo in propertiesInfo)
				{
					if (propertyInfo.Name == nameof(Name))
					{
						continue;
					}

					var index = propertyInfo.GetCustomAttribute<DisplayingIndexAttribute>()?.Index
						?? throw new InvalidOperationException($"Attribute type '{nameof(DisplayingIndexAttribute)}' is required.");
					if (!indexedDictionary.TryAdd(index, new() { propertyInfo }))
					{
						indexedDictionary[index].Add(propertyInfo);
					}
				}

				var rawHintArguments =
					from index in indexedDictionary.Keys
					orderby index
					let currentPropertiesInfo = indexedDictionary[index]
					select currentPropertiesInfo switch
					{
						[var pi] when g(pi) is var (name, argumentDisplayer) => $"{name} <{argumentDisplayer}>",
						_ => $"({string.Join('|', from pi in currentPropertiesInfo let p = g(pi) select $"{p.Name} <{p.ArgumentDisplayer}>")})"
					};
				var hint = string.Join(' ', rawHintArguments) is var h && string.IsNullOrWhiteSpace(h) ? $"  {Name}" : $"  {Name} {h}";
				var usageText = EqualityContract.GetCustomAttributes<UsageAttribute>().ToArray() switch
				{
					var attributes and not []
						=>
						$"""
						
						---
						用法举例：
						{string.Join(
							Environment.NewLine,
							from attribute in attributes select $"{attribute.UsageText}：{attribute.Description}"
						)}
						""",
					_ => string.Empty
				};
				await gmr.SendMessageAsync(
					$$"""
					指令“{{Name}}”语法：
					{{hint}}{{usageText}}
					---
					符号说明：
					  * 小括号“(a|b)”：a 或 b 只需要给出任一个即可。
					  * 中括号“[a]”：表示参数 a 可以没有。
					  * 尖括号“<a>”：这里填入的是该参数配套的数值。
					---
					需要查询详细参数，请在参数名之后跟问号“？”以查询参数的详情信息，如“！查询 昵称 ？”。
					"""
				);

				break;


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static (string Name, string ArgumentDisplayer) g(PropertyInfo pi)
				{
					var name = pi.GetCustomAttribute<ArgumentAttribute>()!.Name;
					return (name, pi.GetCustomAttribute<ArgumentDisplayerAttribute>()?.ArgumentDisplayer ?? name);
				}
			}

			// ！指令 参数 值
			default:
			{
				await ExecuteCoreAsync(gmr);
				break;
			}
		}


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
	/// 核心处理解析命令行参数的方法。
	/// </summary>
	/// <param name="commandLine">命令行序列。</param>
	/// <param name="module">当前的指令。</param>
	/// <param name="failedReason">错误信息。如果解析失败的时候，该属性则不为 <see cref="ParsingFailedReason.None"/>。</param>
	/// <param name="requestedHintArgumentName">如果用户查询参数信息（命令行参数里的参数带问号的时候）。</param>
	/// <param name="requestedCommandHint">如果用户查询指令信息（指令后直接跟问号的时候）。</param>
	/// <returns>返回 <see cref="bool"/> 表示是否解析成功。</returns>
	private static bool ParseMessageCore(
		string commandLine,
		Command module,
		out ParsingFailedReason failedReason,
		[MaybeNullWhen(false)] out List<string>? requestedHintArgumentName,
		out bool requestedCommandHint
	)
	{
		(requestedHintArgumentName, requestedCommandHint, var moduleType) = (null, false, module.EqualityContract);
		switch (ParseCommandLine(commandLine, """[\""“”].+?[\""“”]|[^ ]+""", new[] { '"', '“', '”' }))
		{
			case []:
			{
				failedReason = ParsingFailedReason.InvalidInput;
				return false;
			}
			case [var commandName, var questionMark]
			when CommandEqualityComparer(commandName, module) && Array.Exists(HintRequestedTokens, token => token == questionMark):
			{
				failedReason = ParsingFailedReason.None;
				requestedCommandHint = true;
				return true;
			}
			case var args:
			{
				for (var i = 1; i < args.Length; i++)
				{
					var arg = args[i];
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
						failedReason = ParsingFailedReason.PropertyNotFound;
						return false;
					}

					if (foundPropertyInfo is not { CanRead: true, CanWrite: true })
					{
						failedReason = ParsingFailedReason.PropertyMissingAccessor;
						return false;
					}

					if (foundPropertyInfo.GetIndexParameters().Length != 0)
					{
						failedReason = ParsingFailedReason.PropertyIsIndexer;
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
								failedReason = ParsingFailedReason.PropertyMissingConverter;
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
		}
	}

	/// <summary>
	/// 表示命令名称匹配的方法。
	/// </summary>
	/// <param name="name">名称。</param>
	/// <param name="module">模块的名字。</param>
	/// <returns>一个 <see cref="bool"/> 结果。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool CommandEqualityComparer(string name, Command module)
		=> Array.Exists(ModuleTriggeringPrefix, prefix => name == $"{prefix}{module.Name}");

	/// <summary>
	/// 用来将一个字符串直接拆解成一个一个的参数序列。以空格分隔。如果带有引号，则引号是一个整体，里面可包含空格。
	/// </summary>
	/// <param name="s">字符串。</param>
	/// <param name="argumentMatcher">匹配字符串参数的正则表达式。</param>
	/// <param name="trimmedCharacters">表示最终拆解字符串的时候，需要额外去除的字符。比如引号。</param>
	/// <returns>解析后的参数序列，按次序排列。</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string[] ParseCommandLine(string s, [StringSyntax(StringSyntax.Regex)] string argumentMatcher, char[]? trimmedCharacters)
		=> from match in new Regex(argumentMatcher, RegexOptions.Singleline).Matches(s) select match.Value.Trim(trimmedCharacters);

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
