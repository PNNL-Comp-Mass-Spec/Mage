// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("General", "RCS1079:Throwing of new NotImplementedException.", Justification = "This is a placeholder method", Scope = "member", Target = "~M:Mage.BaseModule.HandleDataRow(System.Object,Mage.MageDataEventArgs)")]
[assembly: SuppressMessage("General", "RCS1079:Throwing of new NotImplementedException.", Justification = "This is a placeholder method", Scope = "member", Target = "~M:Mage.BaseModule.Run(System.Object)")]
[assembly: SuppressMessage("Performance", "RCS1197:Optimize StringBuilder.Append/AppendLine call.", Justification = "Leave as-is for readability", Scope = "member", Target = "~M:Mage.SQLBuilder.BuildQuerySQL~System.String")]
[assembly: SuppressMessage("Usage", "RCS1246:Use element access.", Justification = "Prefer to use .First()", Scope = "member", Target = "~M:Mage.FileListFilter.SetupSearch")]
