// The project is copied from https://github.com/Antecer/QQChannelBot
// Some of codes has been modified by me for unifying the namespace naming and coding styles, after copied.
//
// Original author: Antecer
// License: MIT license
// Copyright (c) 2021-2022 Antecer. All rights reserved.

global using System;
global using System.Buffers;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Drawing;
global using System.Globalization;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Net.WebSockets;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using Sudoku.Bot.Communication.JsonConverters;
global using Sudoku.Bot.Communication.Models;
global using Sudoku.Bot.Communication.Models.Interaction;
global using Sudoku.Bot.Communication.Models.MessageTemplates;
global using Sudoku.Bot.Communication.Models.Returning;
global using Sudoku.Bot.Communication.Resources;
global using Sudoku.Bot.Communication.Triggering;
global using Timer = System.Timers.Timer;
