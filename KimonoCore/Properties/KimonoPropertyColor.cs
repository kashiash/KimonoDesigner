﻿using System;
using SkiaSharp;
using TextBase;

namespace KimonoCore
{
	/// <summary>
	/// Defines a `KimonoProperty` that represents a `KimonoColor` value.
	/// </summary>
	[Table("ColorProperty")]
	public class KimonoPropertyColor : KimonoProperty
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The `KimonoColor` value.</value>
		[Child]
		public KimonoColor Value { get; set; } = new KimonoColor();

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:KimonoCore.KimonoProperty"/> gets value from script.
		/// </summary>
		/// <value><c>true</c> if gets value from script; otherwise, <c>false</c>.</value>
		[Ignore]
		public override bool GetsValueFromScript
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:KimonoCore.KimonoPropertyColor"/> is obi script value.
		/// </summary>
		/// <value><c>true</c> if is obi script value; otherwise, <c>false</c>.</value>
		[Ignore]
		public override bool IsObiScriptValue
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:KimonoCore.KimonoPropertyText"/> class.
		/// </summary>
		public KimonoPropertyColor()
		{
			// Initialize
			Name = "Color Property";
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Evaluate this instance by executing any attached Obi Script to get the new
		/// value for the `KimonoProperty`.
		/// </summary>
		/// <returns>The result of the Obi Script execution as a `ObiScriptResult`.</returns>
		public override ObiScriptResult Evaluate()
		{
			// Is there a script attached?
			if (GetsValueFromScript)
			{
				// Execute the script to get the new value
				ObiScriptEngine.Runtime.Execute(ObiScript);

				// Was the script successful?
				if (ObiScriptEngine.EvaluationResult.Successful)
				{
					// Was the right type returned?
					if (ObiScriptEngine.EvaluationResult.Value is KimonoColor)
					{
						// Yes, save it
						Value = ObiScriptEngine.EvaluationResult.Value as KimonoColor;
					}
					else
					{
						// Report this as an error
						ObiScriptEngine.EvaluationResult.Successful = false;
						ObiScriptEngine.EvaluationResult.ErrorMessage = "Error: Script did not return a named color. " +
							"Call `Return.Color(\"name\");` to return the required value.";
					}
				}
			}

			// Return the result of executing the script
			return ObiScriptEngine.EvaluationResult;
		}
		#endregion

		#region Conversion Routines
		/// <summary>
		/// Converts this `KimonoProperty` to a `KimonoColor`.
		/// </summary>
		/// <returns>The color.</returns>
		public override KimonoColor ToColor()
		{
			return Value;
		}

		/// <summary>
		/// Converts to KimonoCore based source code
		/// </summary>
		/// <returns>The property as a KimonoCore object.</returns>
		public override string ToKimonoCore()
		{
			var sourceCode = "new KimonoPropertyColor() {\n" +
				$"\tName = \"{Name}\",\n" +
				$"\tGetsValueFromScript = {GetsValueFromScript.ToString().ToLower()},\n" +
				$"\tObiScript = \"{ScriptToString()}\",\n" +
				$"\tValue = {Value.ElementName}\n" +
				"};\n";

			// Return results
			return sourceCode;
		}

		/// <summary>
		/// Converts this property to Global scoped C# code.
		/// </summary>
		/// <returns>The property as C# code.</returns>
		/// <param name="outputLibrary">The `CodeOutputLibrary` to use.</param>
		public override string ToCSharpGlobal(CodeOutputLibrary outputLibrary)
		{
			var sourceCode = "";

			// Make base
			sourceCode += $"// Global property {Name}\n";

			// Take action based on usage
			switch (outputLibrary)
			{
				case CodeOutputLibrary.SkiaSharp:
					sourceCode += $"public static SKColor {ElementName} " +
						"{get; set;}" +
						$" = {Value.ElementName};\n";
					break;
				case CodeOutputLibrary.KimonoCore:
					sourceCode += $"public static KimonoPropertyColor {ElementName} " +
						"{get; set;}" +
						$" = {ToKimonoCore()}\n";
					break;

			}

			// Return results
			return sourceCode;
		}

		/// <summary>
		/// Converts this property to Local scoped C# code.
		/// </summary>
		/// <returns>The property as C# code.</returns>
		/// <param name="outputLibrary">The `CodeOutputLibrary` to use.</param>
		public override string ToCSharpLocal(CodeOutputLibrary outputLibrary)
		{
			var sourceCode = "";

			// Make base
			sourceCode += $"// Local property {Name}\n" +
				$"var {ElementName}";

			// Take action based on usage
			switch (outputLibrary)
			{
				case CodeOutputLibrary.SkiaSharp:
					sourceCode += $" = {Value.ElementName};\n";
					break;
				case CodeOutputLibrary.KimonoCore:
					sourceCode += $" = {ToKimonoCore()}\n";
					break;

			}

			// Return results
			return sourceCode;
		}

		/// <summary>
		/// Converts this property to Parameter scoped C# code.
		/// </summary>
		/// <returns>The property as C# code.</returns>
		/// <param name="outputLibrary">The `CodeOutputLibrary` to use.</param>
		public override string ToCSharpParameter(CodeOutputLibrary outputLibrary)
		{
			var sourceCode = $"KimonoPropertyColor {ElementName}";

			// Return results
			return sourceCode;
		}

		/// <summary>
		/// Converts this object to source code for the given OS, Language and Library.
		/// </summary>
		/// <returns>The object represented as source code in a `string`.</returns>
		/// <param name="outputOS">The `CodeOutputOS`.</param>
		/// <param name="outputLanguage">The `CodeOutputLanguage`.</param>
		/// <param name="outputLibrary">The `CodeOutputLibrary`.</param>
		public override string ToCode(CodeOutputOS outputOS, CodeOutputLanguage outputLanguage, CodeOutputLibrary outputLibrary)
		{
			var sourceCode = "";

			// Accumulate color
			if (Value != null)
			{
				// Yes, add as a supporting color
				KimonoCodeGenerator.AddSupportingColor(Value);
			}

			// Take action based on the output language
			switch (outputLanguage)
			{
				case CodeOutputLanguage.CSharp:
					sourceCode = ToCSharp(outputLibrary);
					break;
			}

			// Return results
			return sourceCode;
		}
		#endregion

		#region Cloning
		/// <summary>
		/// Clone this instance.
		/// </summary>
		/// <returns>The clone.</returns>
		public override KimonoProperty Clone()
		{
			// Make copy
			var newProperty = new KimonoPropertyColor()
			{
				Name = this.Name,
				Usage = this.Usage,
				IsObiScriptValue = this.IsObiScriptValue,
				ObiScript = this.ObiScript,
				GetsValueFromScript = this.GetsValueFromScript,
				Value = this.Value.Clone()
			};

			// Return clone
			return newProperty;
		}
		#endregion
	}
}
