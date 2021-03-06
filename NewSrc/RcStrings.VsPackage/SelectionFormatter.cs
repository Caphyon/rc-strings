﻿//------------------------------------------------------------------------------
// <copyright file="RcStrings.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.Shell;

namespace Caphyon.RcStrings.VsPackage
{
  public static class SelectionFormatter
  {
    public static string Format(string aEditorSelection)
    {
      var result = aEditorSelection.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

      for (var index = 0; index < result.Length; ++index)
      {
        result[index] = FormatLine(result[index].Trim());
      }

      aEditorSelection = string.Join("", result);
      return aEditorSelection;
    }

    private static string FormatLine(string aEditorSelection)
    {
      if (aEditorSelection.StartsWith("_T(") && aEditorSelection.EndsWith(")"))
        aEditorSelection = FormatForText(aEditorSelection.Substring(2));
      else if (aEditorSelection.StartsWith("TEXT(") && aEditorSelection.EndsWith(")"))
        aEditorSelection = FormatForText(aEditorSelection.Substring(4));

      if (!aEditorSelection.EndsWith("\""))
        return aEditorSelection;

      if (aEditorSelection.StartsWith("L"))
        aEditorSelection = FormatForL(aEditorSelection.Substring(1));
      else if (aEditorSelection.StartsWith("R\""))
        aEditorSelection = FormatForR(aEditorSelection.Substring(1));
      else if (aEditorSelection.StartsWith("u8"))
        aEditorSelection = FormatForL(aEditorSelection.Substring(2));
      else if (aEditorSelection.StartsWith("u") || aEditorSelection.StartsWith("U"))
        aEditorSelection = FormatForL(aEditorSelection.Substring(1));
      aEditorSelection = FormatForQuotes(aEditorSelection);

      return aEditorSelection;
    }

    private static string FormatForL(string aEditorSelection)
    {
      if (aEditorSelection.StartsWith("\""))
          return FormatForQuotes(aEditorSelection);
      return FormatLine(aEditorSelection);
    }

    private static string FormatForR(string aEditorSelection)
    {
      return FormatForParanthesis(FormatForQuotes(aEditorSelection));
    }

    private static string FormatForText(string aEditorSelection)
    {
      aEditorSelection = FormatForParanthesis(aEditorSelection);
      aEditorSelection = FormatForQuotes(aEditorSelection);
      return FormatLine(aEditorSelection);
    }

    private static string FormatForParanthesis(string aEditorSelection)
    {
      if (aEditorSelection.StartsWith("(") && aEditorSelection.EndsWith(")"))
        aEditorSelection = aEditorSelection.TrimPrefix("(").TrimSuffix(")");
      return aEditorSelection;
    }

    private static string FormatForQuotes(string aEditorSelection)
    {
      aEditorSelection = aEditorSelection.TrimPrefix("\"").TrimSuffix("\"");
      return aEditorSelection;
    }
  }
}