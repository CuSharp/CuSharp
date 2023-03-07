target datalayout = "e-p:64:64:64-i1:8:8-i8:8:8-i16:16:16-i32:32:32-i64:64:64-i128:128:128-f32:32:32-f64:64:64-v16:16:16-v32:32:32-v64:64:64-v128:128:128-n16:32:64"
target triple = "nvptx64-nvidia-cuda"


define void @kernel(float* %A, float* %B, float* %C, i64* %arrsize) {
entry:
  %0 = call i32 @llvm.nvvm.read.ptx.sreg.ctaid.x()
  %1 = call i32 @llvm.nvvm.read.ptx.sreg.ntid.x()
  %mul = mul i32 %0, %1
  %2 = call i32 @llvm.nvvm.read.ptx.sreg.tid.x()
  %add = add i32 %mul, %2
  %idxprom = sext i32 %add to i64

  %size = load i64, i64* %arrsize, align 4
  %lt = icmp ult i64 %idxprom, %size
  br i1 %lt, label %calc, label %after

calc:

  %ptrA = getelementptr float, float* %A, i64 %idxprom
  %ptrB = getelementptr float, float* %B, i64 %idxprom
  %ptrC = getelementptr float, float* %C, i64 %idxprom

  br label %loop
loop: ;preds = %calc, %loop
  %idx = phi i32 [0, %calc], [%next, %loop]
  %next = add i32 %idx, 1

  %valA = load float, float* %ptrA, align 4
  %valB = load float, float* %ptrB, align 4
  %valC = fmul float %valA, %valB
  store float %valC, float* %ptrC, align 4
  call void @llvm.nvvm.barrier0()
  %eq = icmp eq i32 %next, 1000
  br i1 %eq, label %after, label %loop
  br label %after
after:
  ret void
}

declare i32 @llvm.nvvm.read.ptx.sreg.ctaid.x() nounwind readnone
declare i32 @llvm.nvvm.read.ptx.sreg.ntid.x() nounwind readnone
declare i32 @llvm.nvvm.read.ptx.sreg.tid.x() nounwind readnone

declare void @llvm.nvvm.barrier0()

!nvvm.annotations = !{!0}
!0 = !{void (float*,float*,float*, i64*)* @kernel, !"kernel", i32 1}

!nvvmir.version = !{!2}
!2 = !{i32 2, i32 0, i32 3, i32 1}