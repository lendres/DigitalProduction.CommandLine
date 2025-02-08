using C5;
using System;
using System.Diagnostics;
using System.Text;

namespace DigitalProduction.CommandLine;

/// <summary>
/// Class performing an "optimal solution" word wrapping creating a somewhat more estetically pleasing layout. 
/// </summary>
/// <remarks><para>This is based on the 
/// "optimal solution" as described on the Wikipedia page for "Word Wrap" (http://en.wikipedia.org/wiki/Word_wrap).
/// The drawback of this method compared to the simple "greedy" technique is that this is much, much slower. However for 
/// short strings to print as console messages this will not be a problem, but using it in a WYSIWYG word processor is probably
/// not a very good idea.</para></remarks>
internal class OptimalWordWrappedString
{
	#region Private Types

	private class LineBreakResult
	{
		public int Cost;
		public int K;

		public LineBreakResult()
		{
			Cost	= _infinity;
			K		= -1;
		}

		public LineBreakResult(int cost, int k)
		{
			Cost	= cost;
			K		= k;
		}
	}

	private struct WordInfo
	{
		public int SpacesBefore;
		public int Position;
		public int Length;
		public int TotalLength;
	}

	#endregion

	#region Private Fields

	private WordInfo[]?						_wordList;
	private readonly StringBuilder			_result			= new();
	private readonly LineBreakResult[]?		_cache;
	private readonly string?				_string;
	private readonly int					_lineWidth;
	private const int						_infinity		= int.MaxValue / 2;

	// We need a rectangular array here, so this warning is unwarranted.
	private readonly int[,]?				_costCache;

	#endregion

	#region Constructors

	public OptimalWordWrappedString(string s, int lineWidth)
	{
		string[] lines = s.Split('\n');
		for (int c = 0; c < lines.Length; c++)
		{
			_string = lines[c].Trim();
			_lineWidth = lineWidth;

			BuildWordList(_string, _lineWidth);
			Debug.Assert(_wordList != null);
			_costCache = new int[_wordList.Length, _wordList.Length];
			for (int x = 0; x < _wordList.Length; x++)
			{
				for (int y = 0; y < _wordList.Length; y++)
				{
					_costCache[x, y] = -1;
				}
			}

			_cache = new LineBreakResult[_wordList.Length];

			ArrayList<int> stack = [];

			LineBreakResult last = new(0, _wordList.Length - 1);
			stack.Push(last.K);
			while (last.K >= 0)
			{
				last = FindLastOptimalBreak(last.K);
				if (last.K >= 0)
				{
					stack.Push(last.K);
				}
			}

			int start = 0;
			while (!stack.IsEmpty)
			{
				int next = stack.Pop();
				_result.Append(GetWords(start, next));
				if (!stack.IsEmpty)
				{
					_result.Append(Environment.NewLine);
				}
				start = next + 1;
			}

			if (c != lines.Length - 1)
			{
				_result.Append(Environment.NewLine);
			}
		}

		_wordList	= null;
		_cache		= null;
		_string		= null;
		_costCache	= null;
	}

	#endregion

	#region Public Methods

	public override string ToString()
	{
		return _result.ToString();
	}

	#endregion

	#region Private Methods

	private string GetWords(int i, int j)
	{
		Debug.Assert(_wordList != null);
		Debug.Assert(_string != null);

		int start = _wordList[i].Position;
		int end = (j + 1 >= _wordList.Length) ? _string.Length : _wordList[j + 1].Position - (_wordList[j + 1].SpacesBefore - _wordList[j].SpacesBefore);
		return _string[start..end];
	}

	private void BuildWordList(string s, int lineWidth)
	{
		Debug.Assert(!s.Contains('\n'));

		ArrayList<WordInfo> mWordListAL = [];

		bool lookingForWs = false;
		WordInfo we = new()
		{
			Position = 0
		};
		int spaces		= 0;
		int totalLength	= 0;
		for (int i = 0; i < s.Length; i++)
		{
			char ch = s[i];
			if (lookingForWs && ch == ' ')
			{
				spaces++;
				if (we.Position != i)
				{
					mWordListAL.Add(we);
				}
				we = new WordInfo
				{
					SpacesBefore = spaces,
					Position = i + 1
				};
				lookingForWs = false;
				continue;
			}
			else if (ch != ' ')
			{
				lookingForWs = true;
			}

			we.Length++;
			totalLength++;
			we.TotalLength = totalLength;

			if (we.Length == lineWidth)
			{
				mWordListAL.Add(we);
				we = new WordInfo
				{
					SpacesBefore = spaces,
					Position = i + 1
				};
			}
		}
		mWordListAL.Add(we);
		_wordList = [.. mWordListAL];
	}

	private int SumWidths(int i, int j)
	{
		Debug.Assert(_wordList != null);
		return i == 0 ? _wordList[j].TotalLength : _wordList[j].TotalLength - _wordList[i - 1].TotalLength;
	}

	private int GetCost(int i, int j)
	{
		Debug.Assert(_wordList != null);
		Debug.Assert(_costCache != null);

		int cost = _costCache[i, j];

		if (cost == -1)
		{
			cost = _lineWidth - (_wordList[j].SpacesBefore - _wordList[i].SpacesBefore) - SumWidths(i, j);
			cost = cost < 0 ? _infinity : cost * cost;
			_costCache[i, j] = cost;
		}
		return cost;
	}

	private LineBreakResult FindLastOptimalBreak(int j)
	{
		Debug.Assert(_cache != null);

		if (_cache[j] != null)
		{
			return _cache[j];
		}

		int cost = GetCost(0, j);
		if (cost < _infinity)
		{
			return new LineBreakResult(cost, -1);
		}

		LineBreakResult min = new();
		for (int k = 0; k < j; k++)
		{
			int result = FindLastOptimalBreak(k).Cost + GetCost(k + 1, j);
			if (result < min.Cost)
			{
				min.Cost = result;
				min.K = k;
			}
		}

		_cache[j] = min;
		return min;
	}

	#endregion
}