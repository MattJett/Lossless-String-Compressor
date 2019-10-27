using System;
using System.Collections.Generic;
using System.Linq;

namespace LosslessStringCompressor
{
	#region HUFFMAN CLASS
	/// <summary>
	///		The main <c>Huffman</c> class.
	///		This class takes in a standard input,
	///		implements its own huffman tree structure.
	/// </summary>
	/// <remarks>
	///		There are three public methods in this class.
	///		<list type="bullet">
	///			<item>
	///				<term>Encode</term>
	///				<description>Encodes standard input into it's huffman code.</description>
	///			</item>
	///			<item>
	///				<term>Decode</term>
	///				<description>Decodes the huffman code back into it's original message.</description>
	///			</item>
	///			<item>
	///				<term>DataLiteral</term>
	///				<description>Returns each character in tree and the position in tree.</description>
	///			</item>
	///		</list>
	/// </remarks>
	public static class Huffman
	{
		#region PROPERTIES
		static string _input { get; set; }
		static string _encoded { get; set; }
		static string _decoded { get; set; }
		static Node _root { get; set; }
		static List<Node> _huffman { get; set; }
		static Node[] _nodes { get; set; }
		#endregion

		#region PUBLIC CLASS METHODS
		/// <summary>
		///		This method returns a single line string output of each distinct character's
		///		logical position in ascending order in the tree in integer form followed by the character itself.
		/// </summary>
		/// <returns>
		///		String literal formated as: <c>{position} {character}</c> to standard output.
		/// </returns>
		public static string DataLiteral()
		{
			string literal = "";
			var reorderNodes = _nodes.OrderBy(n => n._logical).ToList();

			foreach (var n in reorderNodes)
				literal += n._logical + " " + n._character + " ";

			return literal;
		}

		/// <summary>
		///		This method will take in standard input and
		///		count the distinct character frequencies,
		///		sort them in ascending order,
		///		convert the distinct characters into individual nodes,
		///		build a huffman tree from these nodes,
		///		traverse back down the tree calculating the code for each character,
		///		then append each character's code from the input into a single line of 0's and 1's.
		/// </summary>
		/// <param name="input">Standard input</param>
		/// <returns>
		///		Input's huffman code to standard output.
		/// </returns>
		public static string Encode(string input)
		{
			// SOURCE: https://stackoverflow.com/questions/39472429/count-all-character-occurrences-in-a-string-c-sharp
			// I used this variation with a little help from the link above only on syntax. I'm familur with LINQ and decided to go this route for the sake of brevity.
			// This sexy LINQ query takes care of:
			// 1: Showing only the distinct chars from the input string. GroupBy()
			// 2: Selecting each distinct found char and making a new Node object out of it. Select()
			// 3: Using the anoynomous object's key as the new Node's _character property and using the same anoynomous object's occurences as the new Node's _frequency property. OrderBy()
			// 4: Convert this list to an array for easier handling. ToArray()
			// 5: Finally sorting the Node objects in ascending order by the _frequency property.
			// 6: Saving this new list or pool of Nodes to a variable and to piece together my Huffman Tree.
			// 7: PROFIT! ;)
			_huffman = input.GroupBy(c => c).Select(o => new Node(o.Key, o.Count())).OrderBy(n => n._frequency).ThenBy(n => n._character).ToList();
			_nodes = _huffman.ToArray<Node>();

			BuildTree();
			Traverse();

			foreach (char c in input)
				_encoded += Array.Find(_nodes, n => n._character == c)._binary;

			return _encoded;
		}

		/// <summary>
		///		This method will take in the same input's encoded message and
		///		convert the 0's and 1's back into it's original message verbatim.
		/// </summary>
		/// <returns>
		///		The decoded message exactly how it appeared from standard input to standard output.
		/// </returns>
		public static string Decode()
		{
			Node node = _root;
			foreach (char d in _encoded)
			{
				if (node._leftChild != null && node._rightChild != null)
					node = (d == '0') ? node._leftChild : node._rightChild;
				if (node.IsLeaf())
				{
					_decoded += node._character;
					node = _root;
				}
			}
			return _decoded;
		}

		/// <summary>
		///		A clearing method to wipe data from static method properties.
		///		Needed in order to keep entering new data in.
		/// </summary>
		public static void Clear()
		{
			_input = null;
			_encoded = null;
			_decoded = null;
			_root = null;
			_huffman = null;
			_nodes = null;
		}
		#endregion

		#region PRIVATE CLASS METHODS
		private static void BuildTree()
		{
			_huffman.Sort();
			while (_huffman.Count > 1)
			{
				var parent = new Node();
				if (_huffman[0]._frequency > _huffman[1]._frequency)
					_huffman.Sort();
				var min = _huffman[0];
				var secondMin = _huffman[1];

				if (parent._leftChild == null)
				{
					parent._leftChild = min;
					min._parent = parent;
					_huffman.Remove(min);
				}
				if (parent._rightChild == null)
				{
					parent._rightChild = secondMin;
					secondMin._parent = parent;
					_huffman.Remove(secondMin);
				}

				parent._frequency = min._frequency + secondMin._frequency;
				_huffman.Insert(0, parent);
			}
			if (_huffman != null || _huffman[0]._parent == null)
				_root = _huffman[0];
		}

		private static void Traverse()
		{
			Traverse(_root, 1);

			void Traverse(Node node, int pos)
			{
				// Base Case
				if (node.IsLeaf())
				{
					node._logical = pos;
					if (node._parent == null)
						node._binary = Convert.ToString(node._logical, 2).Substring(0);
					else
						node._binary = Convert.ToString(node._logical, 2).Substring(1);
					return;
				}

				Traverse(node._leftChild, pos * 2);
				Traverse(node._rightChild, (pos * 2) + 1);
			}
		}
		#endregion
	}
	#endregion

	#region NODE CLASS
	class Node : IComparable<Node>
	{
		// PROPERTIES
		internal char? _character { get; set; }
		internal int? _frequency { get; set; }
		internal string _binary { get; set; }
		internal int _logical { get; set; }
		internal Node _leftChild { get; set; }
		internal Node _rightChild { get; set; }
		internal Node _parent { get; set; }

		// CONSTRUCTORS
		public Node() { }
		public Node(char character, int frequency)
		{
			_character = character;
			_frequency = frequency;
			_leftChild = null;
			_rightChild = null;
			_parent = null;
		}

		// CLASS METHODS
		public bool IsLeaf()
		{
			return this._leftChild == null && this._rightChild == null;
		}

		public int CompareTo(Node other)
		{
			return this._frequency == other._frequency ? 0 : this._frequency < other._frequency ? -1 : 1;
		}

		public override string ToString()
		{
			return string.Format("Character: {0}   \nFrequency: {1}   \nBinary: {2}   \nLogical: {3}", _character, _frequency, _binary, _logical);
		}
	}
	#endregion
}
