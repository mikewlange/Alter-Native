﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.Cpp.TypeSystem
{
	[Serializable]
	public class CppUnresolvedTypeDefinition : DefaultUnresolvedTypeDefinition
	{
		readonly UsingScope usingScope;
		
		public CppUnresolvedTypeDefinition(UsingScope usingScope, string name)
			: base(usingScope.NamespaceName, name)
		{
			this.usingScope = usingScope;
			this.AddDefaultConstructorIfRequired = true;
		}
		
		public CppUnresolvedTypeDefinition(CppUnresolvedTypeDefinition declaringTypeDefinition, string name)
			: base(declaringTypeDefinition, name)
		{
			this.usingScope = declaringTypeDefinition.usingScope;
			this.AddDefaultConstructorIfRequired = true;
		}
		
		public override ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext)
		{
			return new CppTypeResolveContext(parentContext.CurrentAssembly, usingScope.Resolve(parentContext.Compilation), parentContext.CurrentTypeDefinition);
		}
	}
}
