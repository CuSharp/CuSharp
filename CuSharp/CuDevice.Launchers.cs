
using ManagedCuda;
using ManagedCuda.VectorTypes;
using System.Reflection;

namespace CuSharp;
public partial class CuDevice
{
    
    public void Launch(Action method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize)
    {
        object[] parameters = {};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }
    
    public void Launch<T0>(Action<T0> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0)
    {
        object[] parameters = {param0};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }

    public void Launch<T0,T1>(Action<T0,T1> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1) 
    {
        object[] parameters = {param0, param1};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);

    }

    public void Launch<T0,T1,T2>(Action<T0,T1,T2> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2) 
    {
        object[] parameters = {param0, param1, param2};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }

    public void Launch<T0,T1,T2,T3>(Action<T0,T1,T2,T3> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2,Tensor<T3> param3) 
    {
        object[] parameters = {param0, param1, param2, param3};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }

    public void Launch<T0,T1,T2,T3,T4>(Action<T0,T1,T2,T3,T4> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2,Tensor<T3> param3,Tensor<T4> param4) 
    {
        object[] parameters = {param0, param1, param2, param3, param4};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }

    public void Launch<T0,T1,T2,T3,T4,T5>(Action<T0,T1,T2,T3,T4,T5> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2,Tensor<T3> param3,Tensor<T4> param4,Tensor<T5> param5) 
    {
        object[] parameters = {param0, param1, param2, param3, param4, param5};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }

    public void Launch<T0,T1,T2,T3,T4,T5,T6>(Action<T0,T1,T2,T3,T4,T5,T6> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2,Tensor<T3> param3,Tensor<T4> param4,Tensor<T5> param5,Tensor<T6> param6) 
    {
        object[] parameters = {param0, param1,param2, param3, param4, param5, param6};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }

    public void Launch<T0,T1,T2,T3,T4,T5,T6,T7>(Action<T0,T1,T2,T3,T4,T5,T6,T7> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2,Tensor<T3> param3,Tensor<T4> param4,Tensor<T5> param5,Tensor<T6> param6,Tensor<T7> param7) 
    {
        object[] parameters = {param0, param1, param2, param3, param4, param5, param6, param7};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }

    public void Launch<T0,T1,T2,T3,T4,T5,T6,T7,T8>(Action<T0,T1,T2,T3,T4,T5,T6,T7,T8> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2,Tensor<T3> param3,Tensor<T4> param4,Tensor<T5> param5,Tensor<T6> param6,Tensor<T7> param7,Tensor<T8> param8) 
    {
        object[] parameters = {param0, param1, param2, param3, param4, param5, param6, param7, param8};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }
            
    public void Launch<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9>(Action<T0,T1,T2,T3,T4,T5,T6,T7,T8,T9> method, (uint, uint, uint) gridSize, (uint, uint, uint) blockSize, Tensor<T0> param0,Tensor<T1> param1,Tensor<T2> param2,Tensor<T3> param3,Tensor<T4> param4,Tensor<T5> param5,Tensor<T6> param6,Tensor<T7> param7,Tensor<T8> param8,Tensor<T9> param9) 
    {
        object[] parameters = {param0, param1, param2, param3, param4, param5, param6, param7, param8, param9};
        _launcher.CompileAndLaunch(method.GetMethodInfo(), gridSize, blockSize, parameters);
    }
}
