using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.CatSystem2.Compiler {
	class Program {
		enum CompileType {
			Anm,
			Cst,
			Fes,
		}
		private static void DisplayHelp() {
			Console.WriteLine("CatSystem2 UTF-8 Compiler");
			Console.WriteLine("usage: catcompiler <anm|cst|fes> <input> [output]");
			Console.WriteLine("       catcompiler --type <anm|cst|fes> --input <input> [--output <output>]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("  -h, -?, --help       Show this help text");
			Console.WriteLine("  -t, --type <type>    Script type to compile (anm, cst, fes)");
			Console.WriteLine("  -i, --input <path>   Input file or wildcard");
			Console.WriteLine("  -o, --output <path>  Output directory (defaults to input directory)");
		}

		private static bool IsHelpArg(string arg) {
			return arg == "-h" || arg == "-?" || arg == "--help";
		}

		private static bool TryParseArguments(string[] args, out CompileType type, out string input, out string output) {
			type = default;
			input = null;
			output = null;

			if (args.Length == 0) {
				DisplayHelp();
				return false;
			}
			if (args.Length == 1 && IsHelpArg(args[0])) {
				DisplayHelp();
				input = string.Empty;
				return false;
			}

			// Keep legacy positional syntax while supporting explicit flags for terminal scripts.
			if (!args[0].StartsWith("-")) {
				if (args.Length < 2 || args.Length > 3) {
					Console.WriteLine("Invalid positional arguments.");
					DisplayHelp();
					return false;
				}
				if (!Enum.TryParse(args[0], true, out type)) {
					Console.WriteLine("Expected script type: anm, cst, or fes.");
					return false;
				}
				input = args[1];
				output = (args.Length == 3 ? args[2] : null);
				return true;
			}

			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];
				if (IsHelpArg(arg)) {
					DisplayHelp();
					input = string.Empty;
					return false;
				}
				switch (arg) {
				case "-t":
				case "--type":
					if (++i >= args.Length || !Enum.TryParse(args[i], true, out type)) {
						Console.WriteLine("Invalid or missing value for --type.");
						return false;
					}
					break;
				case "-i":
				case "--input":
					if (++i >= args.Length) {
						Console.WriteLine("Missing value for --input.");
						return false;
					}
					input = args[i];
					break;
				case "-o":
				case "--output":
					if (++i >= args.Length) {
						Console.WriteLine("Missing value for --output.");
						return false;
					}
					output = args[i];
					break;
				default:
					Console.WriteLine($"Unexpected argument \"{arg}\".");
					return false;
				}
			}

			if (string.IsNullOrWhiteSpace(input)) {
				Console.WriteLine("Input is required.");
				return false;
			}
			return true;
		}

		static int Main(string[] args) {
			if (!TryParseArguments(args, out CompileType type, out string input, out string outputArg)) {
				return (input == string.Empty ? 0 : 1);
			}
			try {
				Console.WriteLine("CatSystem2 UTF-8 Compiler");
				string output = outputArg ?? Path.GetDirectoryName(input);
				if (string.IsNullOrEmpty(output))
					output = Directory.GetCurrentDirectory();
				Directory.CreateDirectory(output);
				switch (type) {
				case CompileType.Anm:
					CatUtils.CompileAnimationFiles(input, output);
					break;
				case CompileType.Cst:
					CatUtils.CompileSceneFiles(input, output);
					break;
				case CompileType.Fes:
					CatUtils.CompileScreenFiles(input, output);
					break;
				}
				return 0;
			} catch (Exception ex) {
				Console.WriteLine(ex);
				return 1;
			}
		}
	}
}
