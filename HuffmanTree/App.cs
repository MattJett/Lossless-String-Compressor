using System;
using System.Diagnostics;
using System.Threading;

namespace LosslessStringCompressor
{
	class App
	{
		public static void Main(string[] args)
		{
			string input;
			string encodeResult;
			string decodeResult;
			string dataRepresentation;
			var time = new Stopwatch();

			try
			{
				Console.Write("=== HUFFMAN ENCODER ===\n\n");
				while ((input = Console.ReadLine()) != null && input.Length > 1)
				{
					// ENCODE w/ timer
					time.Start();
					encodeResult = Huffman.Encode(input.Trim());
					time.Stop();

					// NODES w/ logical position and character
					dataRepresentation = Huffman.DataLiteral();

					// DECODE
					decodeResult = Huffman.Decode();

					// OUTPUT
					Console.WriteLine("Input\n-----\nNodes : " + dataRepresentation + "\nEncode: " + encodeResult + "\nDecode: " + decodeResult + "\n\n\n" +
						"Diagnostics\n-----------\n" + "Length: " + encodeResult.Length + "\n" + "Speed: " + (Decimal.Divide(time.ElapsedTicks, Stopwatch.Frequency) * 1000) + "ms" + "\n\n\n");

					// NOTE: I made this class static, I felt there was no need to make this an object. 
					// therefore I use this Clear() method to wipe my class clean after each input is entered.
					Huffman.Clear();
					time.Reset();
				}
			}
			catch (Exception)
			{
				Console.WriteLine("ERROR: Input was invalid, run program again.\n");
			}
		}
	}
}
