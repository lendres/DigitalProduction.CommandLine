namespace DigitalProduction.CommandLine;

/// <summary>
/// Represents the word wrapping method to use for various operations performed by <see cref="StringFormatter"/>.
/// </summary>
public enum WordWrappingMethod
{
	/// <summary>
	/// Uses a greedy algorithm for performing the word wrapping, 
	/// that puts as many words on a line as possible, then moving on to the next line to do the 
	/// same until there are no more words left to place.
	/// </summary>
	/// <remarks>This is the fastest method, but will often create a less esthetically pleasing result than the
	/// <see cref="Optimal"/> method.</remarks>
	Greedy,

	/// <summary>
	/// Uses an algorithm attempting to create an optimal solution of breaking the lines, where the optimal solution is the one
	/// where the remaining space on the end of each line is as small as possible.
	/// </summary>
	/// <remarks>This method creates esthetically more pleasing results than those created by the <see cref="Greedy"/> method, 
	/// but it does so at a significantly slower speed.  This method will work fine for wrapping shorter strings for console 
	/// output, but should probably not be used for a real time WYSIWYG word processor or something similar.</remarks>
	Optimal
}