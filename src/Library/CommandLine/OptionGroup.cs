using C5;
using System;
using System.Diagnostics;
using System.Text;
using SCG = System.Collections.Generic;

namespace DigitalProduction.CommandLine;

internal class OptionGroup
{
	#region Construction

	public OptionGroup(string id, string name, string description, OptionGroupRequirement require, bool requireExplicitAssignment, SCG.IComparer<string> keyComparer)
	{
		Debug.Assert(!String.IsNullOrEmpty(id));
		Debug.Assert(keyComparer != null);

		Id = id;
		Name = name;
		Description = description;
		Require = require;
		Options = new TreeDictionary<string, Option>(keyComparer);
		RequireExplicitAssignment = requireExplicitAssignment;
	}

	#endregion

	#region Properties

	public string Id { get; private set; }

	public OptionGroupRequirement Require { get; private set; }

	public string Description { get; set; }

	public string Name { get; set; }

	public IDictionary<string, Option> Options { get; private set; }

	public bool RequireExplicitAssignment { get; private set; }

	#endregion

	#region Methods

	public string GetOptionNamesAsString()
	{
		StringBuilder names = new();
		bool isFirst = true;
		foreach (SCG.KeyValuePair<string, Option> entry in Options)
		{
			if (!isFirst)
			{
				names.Append(", ");
			}
			else
			{
				isFirst = false;
			}

			names.Append('\"');
			names.Append(entry.Key);
			names.Append('\"');
		}
		return names.ToString();
	}

	#endregion
}