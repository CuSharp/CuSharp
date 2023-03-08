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

namespace CuSharp.Decompiler.Instructions;

/// <summary>
/// Base class for visitor pattern.
/// </summary>
public abstract class ILVisitor<T>
{
    /// <summary>Called by Visit*() methods that were not overridden</summary>
    protected abstract T Default(ILInstruction inst);

    protected internal virtual T VisitInvalidBranch(InvalidBranch inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitInvalidExpression(InvalidExpression inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitNop(Nop inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitILFunction(ILFunction function)
    {
        return Default(function);
    }

    protected internal virtual T VisitBlockContainer(BlockContainer container)
    {
        return Default(container);
    }

    protected internal virtual T VisitBlock(Block block)
    {
        return Default(block);
    }

    protected internal virtual T VisitPinnedRegion(PinnedRegion inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitBinaryNumericInstruction(BinaryNumericInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitNumericCompoundAssign(NumericCompoundAssign inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitUserDefinedCompoundAssign(UserDefinedCompoundAssign inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicCompoundAssign(DynamicCompoundAssign inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitBitNot(BitNot inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitArglist(Arglist inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitBranch(Branch inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLeave(Leave inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitIfInstruction(IfInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitNullCoalescingInstruction(NullCoalescingInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitSwitchInstruction(SwitchInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitSwitchSection(SwitchSection inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitTryCatch(TryCatch inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitTryCatchHandler(TryCatchHandler inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitTryFinally(TryFinally inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitTryFault(TryFault inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLockInstruction(LockInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitUsingInstruction(UsingInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDebugBreak(DebugBreak inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitComp(Comp inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitCall(Call inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitCallVirt(CallVirt inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitCallIndirect(CallIndirect inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitCkfinite(Ckfinite inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitConv(Conv inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdLoc(LdLoc inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdLoca(LdLoca inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitStLoc(StLoc inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitAddressOf(AddressOf inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitThreeValuedBoolAnd(ThreeValuedBoolAnd inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitThreeValuedBoolOr(ThreeValuedBoolOr inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitNullableUnwrap(NullableUnwrap inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitNullableRewrap(NullableRewrap inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdStr(LdStr inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdcI4(LdcI4 inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdcI8(LdcI8 inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdcF4(LdcF4 inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdcF8(LdcF8 inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdcDecimal(LdcDecimal inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdNull(LdNull inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdFtn(LdFtn inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdVirtFtn(LdVirtFtn inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdVirtDelegate(LdVirtDelegate inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdTypeToken(LdTypeToken inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdMemberToken(LdMemberToken inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLocAlloc(LocAlloc inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLocAllocSpan(LocAllocSpan inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitCpblk(Cpblk inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitInitblk(Initblk inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdFlda(LdFlda inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdsFlda(LdsFlda inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitCastClass(CastClass inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitIsInst(IsInst inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdObj(LdObj inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitStObj(StObj inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitBox(Box inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitUnbox(Unbox inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitUnboxAny(UnboxAny inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitNewObj(NewObj inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitNewArr(NewArr inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDefaultValue(DefaultValue inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitThrow(Throw inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitRethrow(Rethrow inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitSizeOf(SizeOf inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdLen(LdLen inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitLdElema(LdElema inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitGetPinnableReference(GetPinnableReference inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitStringToInt(StringToInt inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitExpressionTreeCast(ExpressionTreeCast inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitUserDefinedLogicOperator(UserDefinedLogicOperator inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicLogicOperatorInstruction(DynamicLogicOperatorInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicBinaryOperatorInstruction(DynamicBinaryOperatorInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicUnaryOperatorInstruction(DynamicUnaryOperatorInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicConvertInstruction(DynamicConvertInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicGetMemberInstruction(DynamicGetMemberInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicSetMemberInstruction(DynamicSetMemberInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicGetIndexInstruction(DynamicGetIndexInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicSetIndexInstruction(DynamicSetIndexInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicInvokeMemberInstruction(DynamicInvokeMemberInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicInvokeConstructorInstruction(DynamicInvokeConstructorInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicInvokeInstruction(DynamicInvokeInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDynamicIsEventInstruction(DynamicIsEventInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitMatchInstruction(MatchInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitMakeRefAny(MakeRefAny inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitRefAnyType(RefAnyType inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitRefAnyValue(RefAnyValue inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitYieldReturn(YieldReturn inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitAwait(Await inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDeconstructInstruction(DeconstructInstruction inst)
    {
        return Default(inst);
    }

    protected internal virtual T VisitDeconstructResultInstruction(DeconstructResultInstruction inst)
    {
        return Default(inst);
    }
}

/// <summary>
/// Base class for visitor pattern.
/// </summary>
public abstract class ILVisitor
{
    /// <summary>Called by Visit*() methods that were not overridden</summary>
    protected abstract void Default(ILInstruction inst);

    protected internal virtual void VisitInvalidBranch(InvalidBranch inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitInvalidExpression(InvalidExpression inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitNop(Nop inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitILFunction(ILFunction function)
    {
        Default(function);
    }

    protected internal virtual void VisitBlockContainer(BlockContainer container)
    {
        Default(container);
    }

    protected internal virtual void VisitBlock(Block block)
    {
        Default(block);
    }

    protected internal virtual void VisitPinnedRegion(PinnedRegion inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitBinaryNumericInstruction(BinaryNumericInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitNumericCompoundAssign(NumericCompoundAssign inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitUserDefinedCompoundAssign(UserDefinedCompoundAssign inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicCompoundAssign(DynamicCompoundAssign inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitBitNot(BitNot inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitArglist(Arglist inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitBranch(Branch inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLeave(Leave inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitIfInstruction(IfInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitNullCoalescingInstruction(NullCoalescingInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitSwitchInstruction(SwitchInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitSwitchSection(SwitchSection inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitTryCatch(TryCatch inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitTryCatchHandler(TryCatchHandler inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitTryFinally(TryFinally inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitTryFault(TryFault inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLockInstruction(LockInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitUsingInstruction(UsingInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDebugBreak(DebugBreak inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitComp(Comp inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitCall(Call inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitCallVirt(CallVirt inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitCallIndirect(CallIndirect inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitCkfinite(Ckfinite inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitConv(Conv inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdLoc(LdLoc inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdLoca(LdLoca inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitStLoc(StLoc inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitAddressOf(AddressOf inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitThreeValuedBoolAnd(ThreeValuedBoolAnd inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitThreeValuedBoolOr(ThreeValuedBoolOr inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitNullableUnwrap(NullableUnwrap inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitNullableRewrap(NullableRewrap inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdStr(LdStr inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdcI4(LdcI4 inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdcI8(LdcI8 inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdcF4(LdcF4 inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdcF8(LdcF8 inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdcDecimal(LdcDecimal inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdNull(LdNull inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdFtn(LdFtn inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdVirtFtn(LdVirtFtn inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdVirtDelegate(LdVirtDelegate inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdTypeToken(LdTypeToken inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdMemberToken(LdMemberToken inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLocAlloc(LocAlloc inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLocAllocSpan(LocAllocSpan inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitCpblk(Cpblk inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitInitblk(Initblk inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdFlda(LdFlda inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdsFlda(LdsFlda inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitCastClass(CastClass inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitIsInst(IsInst inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdObj(LdObj inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitStObj(StObj inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitBox(Box inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitUnbox(Unbox inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitUnboxAny(UnboxAny inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitNewObj(NewObj inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitNewArr(NewArr inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDefaultValue(DefaultValue inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitThrow(Throw inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitRethrow(Rethrow inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitSizeOf(SizeOf inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdLen(LdLen inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitLdElema(LdElema inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitGetPinnableReference(GetPinnableReference inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitStringToInt(StringToInt inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitExpressionTreeCast(ExpressionTreeCast inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitUserDefinedLogicOperator(UserDefinedLogicOperator inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicLogicOperatorInstruction(DynamicLogicOperatorInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicBinaryOperatorInstruction(DynamicBinaryOperatorInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicUnaryOperatorInstruction(DynamicUnaryOperatorInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicConvertInstruction(DynamicConvertInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicGetMemberInstruction(DynamicGetMemberInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicSetMemberInstruction(DynamicSetMemberInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicGetIndexInstruction(DynamicGetIndexInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicSetIndexInstruction(DynamicSetIndexInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicInvokeMemberInstruction(DynamicInvokeMemberInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicInvokeConstructorInstruction(DynamicInvokeConstructorInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicInvokeInstruction(DynamicInvokeInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDynamicIsEventInstruction(DynamicIsEventInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitMatchInstruction(MatchInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitMakeRefAny(MakeRefAny inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitRefAnyType(RefAnyType inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitRefAnyValue(RefAnyValue inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitYieldReturn(YieldReturn inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitAwait(Await inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDeconstructInstruction(DeconstructInstruction inst)
    {
        Default(inst);
    }

    protected internal virtual void VisitDeconstructResultInstruction(DeconstructResultInstruction inst)
    {
        Default(inst);
    }
}