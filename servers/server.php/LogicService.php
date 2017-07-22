<?php
/**
 * Created by IntelliJ IDEA.
 * User: Admin
 * Date: 03/09/2015
 * Time: 21:03
 */

error_reporting(E_ALL | E_STRICT);
ini_set('display_errors', 1);
ini_set('max_execution_time', 300); //300 seconds = 5 minutes

if (version_compare(PHP_VERSION, '5.1.0', '<')) {
    exit('Your current PHP version is: ' . PHP_VERSION . '. cornerstone needs version 5.1.0 or later');
};

require_once dirname(__FILE__) . '/lib/php/Boot.class.php';

function global_exception_handler(Exception $ex)
{
    cornerstone_integrator_exceptions_ExceptionManager::HandleException($ex, null, null, null,null, null, null);
}

set_exception_handler('global_exception_handler');

header("Content-Type: text/plain", true);
header("Access-Control-Allow-Origin: *", true);

$PostData = "";
$ResData = "";

try {
    cornerstone_integrator_Integrator::Init();
    $PostData = file_get_contents('php://input');
    $ResData = cornerstone_integrator_Integrator::Process($PostData);
}
catch (Exception $ex){
    cornerstone_integrator_exceptions_ExceptionManager::HandleException($ex, $PostData, null, null,null, null, null);
}

echo $ResData;
