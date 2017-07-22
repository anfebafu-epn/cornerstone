/// <reference path="js/JQuery/typescript/jquery.d.ts"/>
/// <reference path="js/KendoUI/typescript/kendo.all.d.ts"/>
/// <reference path="js/MouseTrap/typescript/mousetrap.d.ts"/>
/// <reference path="js/MouseTrap/typescript/mousetrap-global-bind.d.ts"/>
/// <reference path="js/cornerstone.loader.ts"/>
/// <reference path="js/cornerstone.integrator.ts"/>

head.ready(function () {

    // Control Initialization (WE)
    $('#menuPrincipal').kendoMenu();

    // Events from the Controls (WE)
    // ejemplo
    $('#Ingresar').click(function () {
        $('#Login').hide(null);
        $('#Environment').fadeIn(null);
    });

    // ejemplo
    $('#CerrarSesion').click(function () {
        $('#Environment').hide(null);
        $('#Login').fadeIn(null);
    });

    $('#Ingresar').kendoButton();
    $('#Accion').kendoButton();

    $('#Accion').click(function () {

        //Proxy_tenants_Run1("no entiendo nada", 2, function (r) {
        //    alert(r);
        //});

        //Proxy_tenants_Run2("hola2", 2, function (r) {
        //    alert(r);
        //});

        //ServiceMultiCall(
        //    [
        //        Message_tenants_Run1("ahora", 3, "run1"),
        //        Message_tenants_Run2("ahora", 3, "run2"),
        //    ],
        //    function (o, n) {
        //        alert(o);
        //    });

        //var arr = new Array();
        //for (var i = 0; i <= 1000; i++) {
        //    arr.push(Call_Tenants_Logic_Run2("ahora", 3, "run1"));
        //}
        //ServiceMultiCall(arr, function (result, name) {
        //    $('#Login').append(result + " ");
        //});

        //var oc = new mainder_tenants_obj_ObjectContainer();
        //oc.detail = new Array<mainder_tenants_obj_DetailData>();

        //var d1 = new mainder_tenants_obj_DetailData();
        //d1.ID = 5;
        //d1.Name = "Andrés";
        //oc.detail.push(d1);

        //Proxy_Tenants_Logic_Run5("adlskj", 1, 3, false, null, new Date(), mainder_tenants_obj_Tipos.TIPO1[1], 434, new haxe_ds_IntMap(), "323", new Array<mainder_tenants_obj_ObjectContainer>(), function () { });

        //Proxy_Tenants_Logic_Run5("hola", 1, 2, false, null, new Date(), mainder_tenants_obj_Tipos.TIPO2, 332323, null, "323232", new Array<mainder_tenants_obj_ObjectContainer>(), function (n) {

        //});

        //Proxy_Tenants_Logic_Run6(mainder_tenants_obj_Tipos.TIPO1, function (n) {
        //    alert(n);
        //});

        //var dd = new mainder_tenants_obj_DetailData();
        //dd.ID = -5;
        //dd.Name = "Andrés";

        //Proxy_tenants_Processor_Run2(dd, function (n) {
        //    alert(n.ID.toString() + " : " + n.Name);
        //});

        //Proxy_tenants_Tenants_Run4();

        //Proxy_tenants_Tenants_DB1(function (r) {
        //    alert(r);
        //});

        //var arr = new Array();
        //for (var i = 0; i <= 500; i++) {
        //    arr.push(Call_tenants_Tenants_DB1('n' + i));
        //}
        //ServiceMultiCall(arr, function (result, name) {
        //    $('#Login').append(name + " " + result + " <br>");
        //});



        //Proxy_tenants_Tenants_DB2(function (n) {
        //    $('#grid').kendoGrid({
        //        dataSource: { data: n },
        //        height: 200,
        //        groupable: true,
        //        sortable: true,
        //        columns: [
        //            {
        //            field: "ID",
        //            title: "ID",
        //            width: 120
        //            },
        //            {
        //            field: "Name",
        //            title: "Name",
        //            width: 240
        //            }
        //        ],
        //    });
        //});

        //Proxy_tenants_Tenants_DB3(function (n) {
        //    $.each(n.Rows, function (index, value) {
        //        alert(value.Items[1]);
        //    });
        //});


        //Proxy_tenants_Processor_CreateTableUsuario();
        Proxy_tenants_Processor_InsertarUsuarios();

        //if (JsInteractionObj)
        //    JsInteractionObj.mostrar('samy');
    });
});


