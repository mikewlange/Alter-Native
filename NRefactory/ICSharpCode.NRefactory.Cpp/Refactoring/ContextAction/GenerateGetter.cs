// 
// GenerateGetter.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;

namespace ICSharpCode.NRefactory.Cpp.Refactoring
{
	public class GenerateGetter : IContextAction
	{
		public bool IsValid (RefactoringContext context)
		{
			var initializer = GetVariableInitializer (context);
			if (initializer == null || !initializer.NameToken.Contains (context.Location.Line, context.Location.Column))
				return false;
			var type = initializer.Parent.Parent as TypeDeclaration;
			if (type == null)
				return false;
			foreach (var member in type.Members) {
				if (member is PropertyDeclaration && ContainsGetter ((PropertyDeclaration)member, initializer))
					return false;
			}
			return initializer.Parent is FieldDeclaration;
		}
		
		public void Run (RefactoringContext context)
		{
			var initializer = GetVariableInitializer (context);
			var field = initializer.Parent as FieldDeclaration;
			
			using (var script = context.StartScript ()) {
				script.InsertWithCursor ("Create getter", GeneratePropertyDeclaration (context, field, initializer), Script.InsertPosition.After);
			}
		}
		
		static PropertyDeclaration GeneratePropertyDeclaration (RefactoringContext context, FieldDeclaration field, VariableInitializer initializer)
		{
			var mod = ICSharpCode.NRefactory.Cpp.Modifiers.Public;
			if (field.HasModifier (ICSharpCode.NRefactory.Cpp.Modifiers.Static))
				mod |= ICSharpCode.NRefactory.Cpp.Modifiers.Static;
			
			return new PropertyDeclaration () {
				Modifiers = mod,
				Name = context.GetNameProposal (initializer.Name, false),
				ReturnType = field.ReturnType.Clone (),
				Getter = new Accessor () {
					Body = new BlockStatement () {
						new ReturnStatement (new IdentifierExpression (initializer.Name))
					}
				}
			};
		}
		
		bool ContainsGetter (PropertyDeclaration property, VariableInitializer initializer)
		{
			if (property.Getter.IsNull || property.Getter.Body.Statements.Count () != 1)
				return false;
			var ret = property.Getter.Body.Statements.Single () as ReturnStatement;
			if (ret == null)
				return false;
			return ret.Expression.IsMatch (new IdentifierExpression (initializer.Name)) || 
				ret.Expression.IsMatch (new MemberReferenceExpression (new ThisReferenceExpression (), initializer.Name));
		}
		
		VariableInitializer GetVariableInitializer (RefactoringContext context)
		{
			return context.GetNode<VariableInitializer> ();
		}
	}
}

