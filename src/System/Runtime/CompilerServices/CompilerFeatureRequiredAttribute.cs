// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable CS1591

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.All)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
	public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;


	public string FeatureName { get; }

	public bool IsOptional { get; set; }
}
