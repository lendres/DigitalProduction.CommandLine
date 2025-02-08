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
			Cost	= mInfinity;
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

	private WordInfo[]?						mWordList;
	private readonly StringBuilder			mResult			= new();
	private readonly LineBreakResult[]?		mfCache;
	private readonly string?				mStr;
	private readonly int					mLineWidth;
	private const int						mInfinity		= int.MaxValue / 2;

	// We need a rectangular array here, so this warning is unwarranted.
	private readonly int[,]?				mCostCache;

	#endregion

	#region Constructors

	public OptimalWordWrappedString(string s, int lineWidth)
	{
		string[] lines = s.Split('\n');
		for (int c = 0; c < lines.Length; c++)
		{
			mStr = lines[c].Trim();
			mLineWidth = lineWidth;

			BuildWordList(mStr, mLineWidth);
			Debug.Assert(mWordList != null);
			mCostCache = new int[mWordList.Length, mWordList.Length];
			for (int x = 0; x < mWordList.Length; x++)
			{
				for (int y = 0; y < mWordList.Length; y++)
				{
					mCostCache[x, y] = -1;
				}
			}

			mfCache = new LineBreakResult[mWordList.Length];

			ArrayList<int> stack = [];

			LineBreakResult last = new(0, mWordList.Length - 1);
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
				mResult.Append(GetWords(start, next));
				if (!stack.IsEmpty)
				{
					mResult.Append(Environment.NewLine);
				}
				start = next + 1;
			}

			if (c != lines.Length - 1)
			{
				mResult.Append(Environment.NewLine);
			}
		}

		mWordList	= null;
		mfCache		= null;
		mStr		= null;
		mCostCache	= null;
	}

	#endregion

	#region Public Methods

	public override string ToString()
	{
		return mResult.ToString();
	}

	#endregion

	#region Private Methods

	private string GetWords(int i, int j)
	{
		Debug.Assert(mWordList != null);
		Debug.Assert(mStr != null);

		int start = mWordList[i].Position;
		int end = (j + 1 >= mWordList.Length) ? mStr.Length : mWordList[j + 1].Position - (mWordList[j + 1].SpacesBefore - mWordList[j].SpacesBefore);
		return mStr[start..end];
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
		mWordList = [.. mWordListAL];
	}

	private int SumWidths(int i, int j)
	{
		Debug.Assert(mWordList != null);
		return i == 0 ? mWordList[j].TotalLength : mWordList[j].TotalLength - mWordList[i - 1].TotalLength;
	}

	private int GetCost(int i, int j)
	{
		Debug.Assert(mWordList != null);
		Debug.Assert(mCostCache != null);

		int cost = mCostCache[i, j];

		if (cost == -1)
		{
			cost = mLineWidth - (mWordList[j].SpacesBefore - mWordList[i].SpacesBefore) - SumWidths(i, j);
			cost = cost < 0 ? mInfinity : cost * cost;
			mCostCache[i, j] = cost;
		}
		return cost;
	}

	private LineBreakResult FindLastOptimalBreak(int j)
	{
		Debug.Assert(mfCache != null);

		if (mfCache[j] != null)
		{
			return mfCache[j];
		}

		int cost = GetCost(0, j);
		if (cost < mInfinity)
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

		mfCache[j] = min;
		return min;
	}

	#endregion
}