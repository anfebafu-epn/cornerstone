/**
*  GLOBAL VARIABLES
**/
var CurrentUser = null;
var GlobalConfig = null;
var CurrentLanguage = null;

//var _ServiceURL = "http://localhost:53149/LogicService.ashx"; //.net
//var _ServiceURL = "http://localhost:8080/LogicService"; // tomcat
//var _ServiceURL = "http://localhost:8080/cornerstone.java/LogicService"; //jboss
var _ServiceURL = "http://localhost:8081/LogicService.php"; // PHP

var ServiceURL = function () {
    return _ServiceURL;
};

function Loading_Show() {
    Loading_MsgClear();
    $('#loading-container').show(null);
}

function Loading_Hide() {
    $('#loading-container').hide(null);
}

function Loading_Msg(message) {
    $('#loading-msg').append('<br>' + message);
}

function Loading_MsgClear() {
    $('#loading-msg').html('');
}

var currentJSPath = function () {
    var scripts = document.querySelectorAll('script[src]');
    var currentScript = scripts[scripts.length - 1].src;
    var currentScriptChunks = currentScript.split('/');
    var currentScriptFile = currentScriptChunks[currentScriptChunks.length - 1];
    return currentScript.replace(currentScriptFile, '').replace('HeadJS/', '');
};

var currentRootPath = function () {
    return currentJSPath().replace('js/', '');
};

var currentCSSPath = function () {
    return currentJSPath().replace('js/', 'css/');
};

var currentImagesPath = function () {
    return currentJSPath().replace('js/', 'images/');
};

////////////////////////////////////
// INITIALIZATION
////////////////////////////////////
// First of all, load Jquery with HeadJS
head.load([currentJSPath() + "JQuery/jquery-1.11.3.min.js"], function () {
    // Clear Loading Messages, in order to start with a new history
    Loading_MsgClear();

    // Notify user that it is starting to download framework components
    Loading_Msg('Downloading Framework...');

    var frameworkItems = [
        currentJSPath() + "KendoUI/kendo.all.min.js",
        currentJSPath() + "MouseTrap/mousetrap.min.js",
        currentJSPath() + "MouseTrap/mousetrap-global-bind.min.js",
        currentJSPath() + "cornerstone.integrator.js",
        currentJSPath() + "cornerstone.benchmark.proxy.js",
        currentCSSPath() + "KendoUI/kendo.common.min.css",
        currentCSSPath() + "KendoUI/kendo.metro.min.css",
        currentCSSPath() + "KendoUI/kendo.dataviz.min.css",
        currentCSSPath() + "KendoUI/kendo.dataviz.default.min.css",
        currentCSSPath() + "cornerstone.global.css"
    ];

    // When completed, Load Frameworks
    head.load(frameworkItems, function () {
        // Notify user that it is starting to download current Page files
        Loading_Msg('Downloading Page Files...');

        // Read Configuration File
        $.getJSON(currentRootPath() + "web.txt", function (data) {
            // Save config in memory
            GlobalConfig = data;

            // Document Title
            document.title = GlobalConfig.AppTitle;

            // When Completed, Load current page js and css
            head.load(location.pathname + ".js", location.pathname + ".css");

            // Global Keyboard Events
            Mousetrap.bindGlobal("ctrl+s", function () {
                alert('teclado controlado');
            });

            // Hide Loading screen, the user can start using UI
            Loading_Hide();

            // Muestro el login
            $('#Login').fadeIn();
        });
    });
});
