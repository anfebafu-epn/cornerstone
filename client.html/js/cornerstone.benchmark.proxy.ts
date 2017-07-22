/// <reference path="cornerstone.integrator.ts"/>


function Call_benchmark_Benchmark_Test1(name) {
    var mcc = new cornerstone_integrator_transport_MethodCall();
    mcc.set_LogicModule("benchmark");
    mcc.set_LogicClass("cornerstone.benchmark.Benchmark");
    mcc.set_LogicMethod("Logic_Test1");
    mcc.set_LogicParams([]);
    mcc.set_Name(name);
    return mcc; 
}
function Proxy_benchmark_Benchmark_Test1(callback : (n : number) => any) {
    return ServiceCall(Call_benchmark_Benchmark_Test1('N'), callback);
}

function Call_benchmark_Benchmark_Test2(Par:string, name) {
    var mcc = new cornerstone_integrator_transport_MethodCall();
    mcc.set_LogicModule("benchmark");
    mcc.set_LogicClass("cornerstone.benchmark.Benchmark");
    mcc.set_LogicMethod("Logic_Test2");
    mcc.set_LogicParams([Par]);
    mcc.set_Name(name);
    return mcc; 
}
function Proxy_benchmark_Benchmark_Test2(Par:string, callback : (n : string) => any) {
    return ServiceCall(Call_benchmark_Benchmark_Test2(Par, 'N'), callback);
}

function Call_benchmark_Benchmark_Test3(name) {
    var mcc = new cornerstone_integrator_transport_MethodCall();
    mcc.set_LogicModule("benchmark");
    mcc.set_LogicClass("cornerstone.benchmark.Benchmark");
    mcc.set_LogicMethod("Logic_Test3");
    mcc.set_LogicParams([]);
    mcc.set_Name(name);
    return mcc; 
}
function Proxy_benchmark_Benchmark_Test3(callback : (n : Date) => any) {
    return ServiceCall(Call_benchmark_Benchmark_Test3('N'), callback);
}

function Call_benchmark_Benchmark_Benchmark1(name) {
    var mcc = new cornerstone_integrator_transport_MethodCall();
    mcc.set_LogicModule("benchmark");
    mcc.set_LogicClass("cornerstone.benchmark.Benchmark");
    mcc.set_LogicMethod("Logic_Benchmark1");
    mcc.set_LogicParams([]);
    mcc.set_Name(name);
    return mcc; 
}
function Proxy_benchmark_Benchmark_Benchmark1(callback : (n : number) => any) {
    return ServiceCall(Call_benchmark_Benchmark_Benchmark1('N'), callback);
}

function Call_benchmark_Benchmark_Benchmark2(name) {
    var mcc = new cornerstone_integrator_transport_MethodCall();
    mcc.set_LogicModule("benchmark");
    mcc.set_LogicClass("cornerstone.benchmark.Benchmark");
    mcc.set_LogicMethod("Logic_Benchmark2");
    mcc.set_LogicParams([]);
    mcc.set_Name(name);
    return mcc; 
}
function Proxy_benchmark_Benchmark_Benchmark2(callback : (n : string) => any) {
    return ServiceCall(Call_benchmark_Benchmark_Benchmark2('N'), callback);
}

function Call_benchmark_Benchmark_Benchmark3(name) {
    var mcc = new cornerstone_integrator_transport_MethodCall();
    mcc.set_LogicModule("benchmark");
    mcc.set_LogicClass("cornerstone.benchmark.Benchmark");
    mcc.set_LogicMethod("Logic_Benchmark3");
    mcc.set_LogicParams([]);
    mcc.set_Name(name);
    return mcc; 
}
function Proxy_benchmark_Benchmark_Benchmark3(callback : (n : Date) => any) {
    return ServiceCall(Call_benchmark_Benchmark_Benchmark3('N'), callback);
}

