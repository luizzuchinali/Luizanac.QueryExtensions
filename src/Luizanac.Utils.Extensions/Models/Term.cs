using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Luizanac.Utils.Extensions.Models
{
	public class Term : IEquatable<Term>
	{
		public Term(string filters)
		{

			if (string.IsNullOrWhiteSpace(filters)) throw new ArgumentNullException(nameof(filters));

			var filterSplits = filters.Split(Operators, StringSplitOptions.RemoveEmptyEntries)
					.Select(t => t.Trim()).ToArray();
			Names = Regex.Split(filterSplits[0], EscapedPipePattern).Select(t => t.Trim()).ToArray();
			Values = filterSplits.Length > 1 ? Regex.Split(filterSplits[1], EscapedPipePattern).Select(t => t.Trim()).ToArray() : null;
			Operator = Array.Find(Operators, o => filters.Contains(o)) ?? "==";
			ParsedOperator = GetOperatorParsed(Operator);
			OperatorIsNegated = ParsedOperator != EFilterOperator.NotEquals && Operator.StartsWith("!");
		}

		private const string EscapedPipePattern = @"(?<!($|[^\\])(\\\\)*?\\)\|";

		private static readonly string[] Operators = new string[] {
			"@=",
			"!@=",
			"_=",
			"!_=",
			"!=",
			"==",
			">=",
			"<=",
			">",
			"<",
		};

		public string[] Names { get; private set; }

		public EFilterOperator ParsedOperator { get; private set; }

		public string[] Values { get; private set; }

		public string Operator { get; private set; }

		private EFilterOperator GetOperatorParsed(string @operator)
		{
			switch (@operator)
			{
				case "==":
					return EFilterOperator.Equals;
				case "!=":
					return EFilterOperator.NotEquals;
				case "<":
					return EFilterOperator.LessThan;
				case ">":
					return EFilterOperator.GreaterThan;
				case ">=":
					return EFilterOperator.GreaterThanOrEqualTo;
				case "<=":
					return EFilterOperator.LessThanOrEqualTo;
				case "@=":
					return EFilterOperator.Contains;
				case "!@=":
					return EFilterOperator.NotContains;
				case "_=":
					return EFilterOperator.StartsWith;
				case "!_=":
					return EFilterOperator.NotStartsWith;
				default:
					return EFilterOperator.Equals;
			}
		}

		public bool OperatorIsNegated { get; private set; }

		public bool Equals(Term other)
		{
			return Names.SequenceEqual(other.Names)
				&& Values.SequenceEqual(other.Values)
				&& Operator == other.Operator;
		}
	}
}
