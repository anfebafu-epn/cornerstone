/// <reference path="js/JQuery/typescript/jquery.d.ts"/>
/// <reference path="js/KendoUI/typescript/kendo.all.d.ts"/>
/// <reference path="js/MouseTrap/typescript/mousetrap.d.ts"/>
/// <reference path="js/MouseTrap/typescript/mousetrap-global-bind.d.ts"/>
/// <reference path="js/cornerstone.loader.ts"/>
/// <reference path="js/cornerstone.integrator.ts"/>
head.ready(function () {

    function onSelect(e) {
        if (e.item.innerText.trim() == "Connection Test") {
            $('#Comunicacion').show();
            $('#BenchmarkSimple').hide();
            $('#BenchmarkCarga').hide();
            $('#BenchmarkSecuencial').hide();
            $('#Multicall').hide();
        }
        else if (e.item.innerText.trim() == "Simple Benchmark") {
            $('#Comunicacion').hide();
            $('#BenchmarkSimple').show();
            $('#BenchmarkCarga').hide();
            $('#BenchmarkSecuencial').hide();
            $('#Multicall').hide();
        }
        else if (e.item.innerText.trim() == "Load Benchmark") {
            $('#Comunicacion').hide();
            $('#BenchmarkSimple').hide();
            $('#BenchmarkCarga').show();
            $('#BenchmarkSecuencial').hide();
            $('#Multicall').hide();
        }
        else if (e.item.innerText.trim() == "Sequential Benchmark") {
            $('#Comunicacion').hide();
            $('#BenchmarkSimple').hide();
            $('#BenchmarkCarga').hide();
            $('#BenchmarkSecuencial').show();
            $('#Multicall').hide();
        }
        else if (e.item.innerText.trim() == "Multicall") {
            $('#Comunicacion').hide();
            $('#BenchmarkSimple').hide();
            $('#BenchmarkCarga').hide();
            $('#BenchmarkSecuencial').hide();
            $('#Multicall').show();
        }
    }


    // Control Initialization (WE)
    $('#menuPrincipal').kendoMenu({
        select: onSelect
    });

    //var ServiceArr = [
    //"http://localhost:53149/LogicService.ashx", // .net
    //"http://localhost:8080/LogicService", // java
    //"http://158.69.74.23:8080/cornerstone.java/LogicService" // java nube
    //"http://localhost:8080/cornerstone.java/LogicService", //jboss
    //"http://localhost:8081/LogicService.php" // PHP
    //];


    //var ServiceArr = [
    //    "http://158.69.93.176:8080/cornerstone.java/LogicService", // Server Java 1
    //    "http://158.69.93.177:8080/cornerstone.java/LogicService", // Server Java 2
    //    "http://158.69.93.178:8080/cornerstone.java/LogicService", // Server Java 3
    //    "http://158.69.93.179:8080/cornerstone.java/LogicService" // Server Java 4
    //];

    var ServiceArr = [
       "http://158.69.71.100:8080/cornerstone.java/LogicService", // Ubuntu - Tomcat - Java
       "http://158.69.71.113:8080/cornerstone.java/LogicService", // Ubuntu - Glassfish - Java
       "http://158.69.71.118/LogicService.php", // Ubuntu - Apache - PHP
       "http://158.69.71.12/LogicService.ashx", // Ubuntu - Apache - Mono
       "http://158.69.71.121/LogicService.ashx", // Windows - IIS - Asp.net
       "http://158.69.71.133/LogicService.php" // Windows - IIS - PHP
    ];

    $('#btn_Comunicacion1').kendoButton();

    $('#btn_Comunicacion1').click(function () {

        for (i = 0; i < ServiceArr.length; i++) {
            _ServiceURL = ServiceArr[i];
            // floating point communication test
            //var init = Date.now();

            Proxy_benchmark_Benchmark_Test1(function (r) {
                //var end = Date.now() - init;
                //alert(end);
                alert(r);
            });
        }
    });

    $('#btn_Comunicacion2').kendoButton();

    $('#btn_Comunicacion2').click(function () {
        for (i = 0; i < ServiceArr.length; i++) {
            _ServiceURL = ServiceArr[i];
            // String communication test
            Proxy_benchmark_Benchmark_Test2('Cornerstone', function (r) {
                alert(r);
            });
        }
    });

    $('#btn_Comunicacion3').kendoButton();

    $('#btn_Comunicacion3').click(function () {
        for (i = 0; i < ServiceArr.length; i++) {
            _ServiceURL = ServiceArr[i];
            // Date Calculations communication test
            Proxy_benchmark_Benchmark_Test3(function (r) {
                alert(r);
            });
        }
    });


    $('#btn_Simple1').kendoButton();

    $('#btn_Simple1').click(function () {
        for (i = 0; i < ServiceArr.length; i++) {
            _ServiceURL = ServiceArr[i];
            // floating point benchmark
            Proxy_benchmark_Benchmark_Benchmark1(function (r) {
                alert(r);
            });
        }
    });

    $('#btn_Simple2').kendoButton();

    $('#btn_Simple2').click(function () {
        for (i = 0; i < ServiceArr.length; i++) {
            _ServiceURL = ServiceArr[i];
            // String calculations benchmark
            Proxy_benchmark_Benchmark_Benchmark2(function (r) {
                alert(r);
            });
        }
    });

    $('#btn_Simple3').kendoButton();

    $('#btn_Simple3').click(function () {
        for (i = 0; i < ServiceArr.length; i++) {
            _ServiceURL = ServiceArr[i];
            // Date Calculations benchmark
            Proxy_benchmark_Benchmark_Benchmark3(function (r) {
                alert(r);
            });
        }
    });


    $('#btn_Carga1').kendoButton();

    $('#btn_Carga1').click(function () {


        // floating point benchmark
        for (j = 0; j < 1000; j++) {
            for (i = 0; i < ServiceArr.length; i++) {
                _ServiceURL = ServiceArr[i];
                Proxy_benchmark_Benchmark_Benchmark1(function (r) {
                    //alert(r);
                });
            }
        }
    });

    $('#btn_Carga2').kendoButton();

    $('#btn_Carga2').click(function () {

        // String calculations benchmark
        for (j = 0; j < 1000; j++) {
            for (i = 0; i < ServiceArr.length; i++) {
                _ServiceURL = ServiceArr[i];
                Proxy_benchmark_Benchmark_Benchmark2(function (r) {
                    //alert(r);
                });
            }
        }
    });

    $('#btn_Carga3').kendoButton();

    $('#btn_Carga3').click(function () {

        // Date Calculations benchmark
        for (j = 0; j < 200; j++) {
            for (i = 0; i < ServiceArr.length; i++) {
                _ServiceURL = ServiceArr[i];
                Proxy_benchmark_Benchmark_Benchmark3(function (r) {
                    //alert(r);
                });
            }
        }
    });

    var seqcounter = 0;
    var servicecounter = 0;

    $('#btn_Secuencial1').kendoButton();

    function Benchmark1(r) {
        if (seqcounter == 100) {
            servicecounter++;
            if (servicecounter >= ServiceArr.length) return;
            _ServiceURL = ServiceArr[servicecounter];
            seqcounter = 0;
        }
        Proxy_benchmark_Benchmark_Benchmark1(Benchmark1);
        seqcounter++;
    }

    $('#btn_Secuencial1').click(function () {
        _ServiceURL = ServiceArr[servicecounter];
        seqcounter = 0;
        servicecounter = 0;

        // floating point benchmark
        Benchmark1(0);
    });

    function Benchmark2(r) {
        if (seqcounter == 100) {
            servicecounter++;
            if (servicecounter >= ServiceArr.length) return;
            _ServiceURL = ServiceArr[servicecounter];
            seqcounter = 0;
        }
        Proxy_benchmark_Benchmark_Benchmark2(Benchmark2);
        seqcounter++;
    }

    $('#btn_Secuencial2').kendoButton();

    $('#btn_Secuencial2').click(function () {
        _ServiceURL = ServiceArr[servicecounter];
        seqcounter = 0;
        servicecounter = 0;

        // String calculations benchmark
        Benchmark2(0);

    });


    function Benchmark3(r) {
        if (seqcounter == 100) {
            servicecounter++;
            if (servicecounter >= ServiceArr.length) return;
            _ServiceURL = ServiceArr[servicecounter];
            seqcounter = 0;
        }
        Proxy_benchmark_Benchmark_Benchmark3(Benchmark3);
        seqcounter++;
    }
    $('#btn_Secuencial3').kendoButton();

    $('#btn_Secuencial3').click(function () {
        _ServiceURL = ServiceArr[servicecounter];
        seqcounter = 0;
        servicecounter = 0;

        // Date Calculations benchmark
        Benchmark3(0);
    });


    $('#btn_Multicall1').kendoButton();
    $('#btn_Multicall1').click(function () {

        for (i = 0; i < ServiceArr.length; i++) {

            _ServiceURL = ServiceArr[i];

            var arr = new Array();

            for (j = 0; j < 10; j++) {
                arr.push(Call_benchmark_Benchmark_Benchmark1());
            }

            ServiceMultiCall(arr, function (a) {
                //alert(a);
            });
        }
    });

    $('#btn_Multicall2').kendoButton();
    $('#btn_Multicall2').click(function () {
        for (i = 0; i < ServiceArr.length; i++) {

            _ServiceURL = ServiceArr[i];

            var arr = new Array();

            for (j = 0; j < 10; j++) {
                arr.push(Call_benchmark_Benchmark_Benchmark2());
            }

            ServiceMultiCall(arr, function (a) {
                //alert(a);
            });
        }
    });

    $('#btn_Multicall3').kendoButton();
    $('#btn_Multicall3').click(function () {
        for (i = 0; i < ServiceArr.length; i++) {

            _ServiceURL = ServiceArr[i];

            var arr = new Array();

            for (j = 0; j < 10; j++) {
                arr.push(Call_benchmark_Benchmark_Benchmark3());
            }

            ServiceMultiCall(arr, function (a) {
                //alert(a);
            });
        }
    });
});

