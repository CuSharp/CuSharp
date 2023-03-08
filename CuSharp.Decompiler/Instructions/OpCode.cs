// Copyright (c) 2014-2020 Daniel Grunwald
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

namespace Dotnet4Gpu.Decompilation.Instructions
{
    /// <summary>
    /// Enum representing the type of an <see cref="ILInstruction"/>.
    /// </summary>
    public enum OpCode : byte
    {
        /// <summary>Represents invalid IL. Semantically, this instruction is considered to throw some kind of exception.</summary>
        InvalidBranch,
        /// <summary>Represents invalid IL. Semantically, this instruction is considered to produce some kind of value.</summary>
        InvalidExpression,
        /// <summary>No operation. Takes 0 arguments and returns void.</summary>
        Nop,
        /// <summary>A container of IL blocks.</summary>
        ILFunction,
        /// <summary>A container of IL blocks.</summary>
        BlockContainer,
        /// <summary>A block of IL instructions.</summary>
        Block,
        /// <summary>A region where a pinned variable is used (initial representation of future fixed statement).</summary>
        PinnedRegion,
        /// <summary>Common instruction for add, sub, mul, div, rem, bit.and, bit.or, bit.xor, shl and shr.</summary>
        BinaryNumericInstruction,
        /// <summary>Common instruction for numeric compound assignments.</summary>
        NumericCompoundAssign,
        /// <summary>Common instruction for user-defined compound assignments.</summary>
        UserDefinedCompoundAssign,
        /// <summary>Common instruction for dynamic compound assignments.</summary>
        DynamicCompoundAssign,
        /// <summary>Bitwise NOT</summary>
        BitNot,
        /// <summary>Retrieves the RuntimeArgumentHandle.</summary>
        Arglist,
        /// <summary>Unconditional branch. <c>goto target;</c></summary>
        Branch,
        /// <summary>Unconditional branch to end of block container. Return is represented using IsLeavingFunction and an (optional) return value. The block container evaluates to the value produced by the argument of the leave instruction.</summary>
        Leave,
        /// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
        IfInstruction,
        /// <summary>Null coalescing operator expression. <c>if.notnull(valueInst, fallbackInst)</c></summary>
        NullCoalescingInstruction,
        /// <summary>Switch statement</summary>
        SwitchInstruction,
        /// <summary>Switch section within a switch statement</summary>
        SwitchSection,
        /// <summary>Try-catch statement.</summary>
        TryCatch,
        /// <summary>Catch handler within a try-catch statement.</summary>
        TryCatchHandler,
        /// <summary>Try-finally statement</summary>
        TryFinally,
        /// <summary>Try-fault statement</summary>
        TryFault,
        /// <summary>Lock statement</summary>
        LockInstruction,
        /// <summary>Using statement</summary>
        UsingInstruction,
        /// <summary>Breakpoint instruction</summary>
        DebugBreak,
        /// <summary>Comparison. The inputs must be both integers; or both floats; or both object references. Object references can only be compared for equality or inequality. Floating-point comparisons evaluate to 0 (false) when an input is NaN, except for 'NaN != NaN' which evaluates to 1 (true).</summary>
        Comp,
        /// <summary>Non-virtual method call.</summary>
        Call,
        /// <summary>Virtual method call.</summary>
        CallVirt,
        /// <summary>Unsafe function pointer call.</summary>
        CallIndirect,
        /// <summary>Checks that the input float is not NaN or infinite.</summary>
        Ckfinite,
        /// <summary>Numeric cast.</summary>
        Conv,
        /// <summary>Loads the value of a local variable. (ldarg/ldloc)</summary>
        LdLoc,
        /// <summary>Loads the address of a local variable. (ldarga/ldloca)</summary>
        LdLoca,
        /// <summary>Stores a value into a local variable. (IL: starg/stloc)
        /// Evaluates to the value that was stored (for byte/short variables: evaluates to the truncated value, sign/zero extended back to I4 based on variable.Type.GetSign())</summary>
        StLoc,
        /// <summary>Stores the value into an anonymous temporary variable, and returns the address of that variable.</summary>
        AddressOf,
        /// <summary>Three valued logic and. Inputs are of type bool? or I4, output is of type bool?. Unlike logic.and(), does not have short-circuiting behavior.</summary>
        ThreeValuedBoolAnd,
        /// <summary>Three valued logic or. Inputs are of type bool? or I4, output is of type bool?. Unlike logic.or(), does not have short-circuiting behavior.</summary>
        ThreeValuedBoolOr,
        /// <summary>The input operand must be one of:
        ///   1. a nullable value type
        ///   2. a reference type
        ///   3. a managed reference to a type parameter.
        /// If the input is non-null, evaluates to the (unwrapped) input.
        /// If the input is null, jumps to the innermost nullable.rewrap instruction that contains this instruction.
        /// In case 3 (managed reference), the dereferenced value is the input being tested, and the nullable.unwrap instruction returns the managed reference unmodified (if the value is non-null).</summary>
        NullableUnwrap,
        /// <summary>Serves as jump target for the nullable.unwrap instruction.
        /// If the input evaluates normally, evaluates to the input value (wrapped in Nullable&lt;T&gt; if the input is a non-nullable value type).If a nullable.unwrap instruction encounters a null input and jumps to the (endpoint of the) nullable.rewrap instruction,the nullable.rewrap instruction evaluates to null.</summary>
        NullableRewrap,
        /// <summary>Loads a constant string.</summary>
        LdStr,
        /// <summary>Loads a constant 32-bit integer.</summary>
        LdcI4,
        /// <summary>Loads a constant 64-bit integer.</summary>
        LdcI8,
        /// <summary>Loads a constant 32-bit floating-point number.</summary>
        LdcF4,
        /// <summary>Loads a constant 64-bit floating-point number.</summary>
        LdcF8,
        /// <summary>Loads a constant decimal.</summary>
        LdcDecimal,
        /// <summary>Loads the null reference.</summary>
        LdNull,
        /// <summary>Load method pointer</summary>
        LdFtn,
        /// <summary>Load method pointer</summary>
        LdVirtFtn,
        /// <summary>Virtual delegate construction</summary>
        LdVirtDelegate,
        /// <summary>Loads runtime representation of metadata token</summary>
        LdTypeToken,
        /// <summary>Loads runtime representation of metadata token</summary>
        LdMemberToken,
        /// <summary>Allocates space in the stack frame</summary>
        LocAlloc,
        /// <summary>Allocates space in the stack frame and wraps it in a Span</summary>
        LocAllocSpan,
        /// <summary>memcpy(destAddress, sourceAddress, size);</summary>
        Cpblk,
        /// <summary>memset(address, value, size)</summary>
        Initblk,
        /// <summary>Load address of instance field</summary>
        LdFlda,
        /// <summary>Load static field address</summary>
        LdsFlda,
        /// <summary>Casts an object to a class.</summary>
        CastClass,
        /// <summary>Test if object is instance of class or interface.</summary>
        IsInst,
        /// <summary>Indirect load (ref/pointer dereference).</summary>
        LdObj,
        /// <summary>Indirect store (store to ref/pointer).
        /// Evaluates to the value that was stored (when using type byte/short: evaluates to the truncated value, sign/zero extended back to I4 based on type.GetSign())</summary>
        StObj,
        /// <summary>Boxes a value.</summary>
        Box,
        /// <summary>Compute address inside box.</summary>
        Unbox,
        /// <summary>Unbox a value.</summary>
        UnboxAny,
        /// <summary>Creates an object instance and calls the constructor.</summary>
        NewObj,
        /// <summary>Creates an array instance.</summary>
        NewArr,
        /// <summary>Returns the default value for a type.</summary>
        DefaultValue,
        /// <summary>Throws an exception.</summary>
        Throw,
        /// <summary>Rethrows the current exception.</summary>
        Rethrow,
        /// <summary>Gets the size of a type in bytes.</summary>
        SizeOf,
        /// <summary>Returns the length of an array as 'native unsigned int'.</summary>
        LdLen,
        /// <summary>Load address of array element.</summary>
        LdElema,
        /// <summary>Retrieves a pinnable reference for the input object.
        /// The input must be an object reference (O).
        /// If the input is an array/string, evaluates to a reference to the first element/character, or to a null reference if the array is null or empty.
        /// Otherwise, uses the GetPinnableReference method to get the reference, or evaluates to a null reference if the input is null.
        /// </summary>
        GetPinnableReference,
        /// <summary>Maps a string value to an integer. This is used in switch(string).</summary>
        StringToInt,
        /// <summary>ILAst representation of Expression.Convert.</summary>
        ExpressionTreeCast,
        /// <summary>Use of user-defined &amp;&amp; or || operator.</summary>
        UserDefinedLogicOperator,
        /// <summary>ILAst representation of a short-circuiting binary operator inside a dynamic expression.</summary>
        DynamicLogicOperatorInstruction,
        /// <summary>ILAst representation of a binary operator inside a dynamic expression (maps to Binder.BinaryOperation).</summary>
        DynamicBinaryOperatorInstruction,
        /// <summary>ILAst representation of a unary operator inside a dynamic expression (maps to Binder.UnaryOperation).</summary>
        DynamicUnaryOperatorInstruction,
        /// <summary>ILAst representation of a cast inside a dynamic expression (maps to Binder.Convert).</summary>
        DynamicConvertInstruction,
        /// <summary>ILAst representation of a property get method call inside a dynamic expression (maps to Binder.GetMember).</summary>
        DynamicGetMemberInstruction,
        /// <summary>ILAst representation of a property set method call inside a dynamic expression (maps to Binder.SetMember).</summary>
        DynamicSetMemberInstruction,
        /// <summary>ILAst representation of an indexer get method call inside a dynamic expression (maps to Binder.GetIndex).</summary>
        DynamicGetIndexInstruction,
        /// <summary>ILAst representation of an indexer set method call inside a dynamic expression (maps to Binder.SetIndex).</summary>
        DynamicSetIndexInstruction,
        /// <summary>ILAst representation of a method call inside a dynamic expression (maps to Binder.InvokeMember).</summary>
        DynamicInvokeMemberInstruction,
        /// <summary>ILAst representation of a constuctor invocation inside a dynamic expression (maps to Binder.InvokeConstructor).</summary>
        DynamicInvokeConstructorInstruction,
        /// <summary>ILAst representation of a delegate invocation inside a dynamic expression (maps to Binder.Invoke).</summary>
        DynamicInvokeInstruction,
        /// <summary>ILAst representation of a call to the Binder.IsEvent method inside a dynamic expression.</summary>
        DynamicIsEventInstruction,
        /// <summary>ILAst representation of C# patterns</summary>
        MatchInstruction,
        /// <summary>Push a typed reference of type class onto the stack.</summary>
        MakeRefAny,
        /// <summary>Push the type token stored in a typed reference.</summary>
        RefAnyType,
        /// <summary>Push the address stored in a typed reference.</summary>
        RefAnyValue,
        /// <summary>Yield an element from an iterator.</summary>
        YieldReturn,
        /// <summary>C# await operator.</summary>
        Await,
        /// <summary>Deconstruction statement</summary>
        DeconstructInstruction,
        /// <summary>Represents a deconstructed value</summary>
        DeconstructResultInstruction,
        /// <summary>Matches any node</summary>
        AnyNode,
    }
}