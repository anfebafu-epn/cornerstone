package cornerstone.integrator;

import cornerstone.integrator.configuration.DatabaseEngineEnum;
import cornerstone.integrator.configuration.DatabaseConnectionData;
import cornerstone.integrator.configuration.DeployEnvironmentEnum;
import cornerstone.integrator.LibReference;
import cornerstone.integrator.transport.MethodCall;
import cornerstone.integrator.interfaces.IDisposable;
import cornerstone.integrator.transport.MethodResult;
import cornerstone.integrator.transport.ResponseMessage;
import cornerstone.integrator.transport.RequestMessage;
import cornerstone.integrator.configuration.ConfigurationData;
import cornerstone.integrator.configuration.Configuration;
import haxe.Serializer;
import haxe.Unserializer;
import cornerstone.integrator.exceptions.ExceptionManager;
import cornerstone.integrator.exceptions.SecurityException;
import cornerstone.integrator.exceptions.LogicException;

/**
*   Clase de procesamiento de pedidos. Toma el pedido recibido por medio de POST y lo procesa ubicando el componente encargado del procesamiento
**/
@:keep
class Integrator {

    public static function Init() {
        if (Configuration.Data == null) {
            Configuration.Data = new ConfigurationData();
            Configuration.Data.LogFolder = "D:\\Workspace\\Cornerstone\\log\\";
            Configuration.Data.CurrentDeployEnvironment = DeployEnvironmentEnum.DEVELOPMENT;
            Configuration.Data.DatabaseConnections = new Array<DatabaseConnectionData>();
            var  dcd = new DatabaseConnectionData();
            dcd.DeploEnvironment = DeployEnvironmentEnum.DEVELOPMENT;
            //dcd.DatabaseEngine = DatabaseEngineEnum.POSTGRESQL;
            //dcd.ConnectionString = "Data Source=sql.smartwork.com.ec\\DEVELOPMENT,1433;Initial Catalog=MAINDERORM;User ID=sa;Password=Smartw0rk;";
            //dcd.ConnectionString = "Data Source=167.114.86.107\\SQL2014,49170;Initial Catalog=DEMO;User ID=sa;Password=Smartw0rk.;";
            //dcd.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=167.114.86.107)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=Ora12c)));User Id=Oscar;Password=Pero01;";
            //dcd.ConnectionString = "Server=167.114.86.107;Uid=sa;Pwd=Smartw0rk.;Database=demo;";
            //dcd.ConnectionString = "User ID=postgres;Password=Smartw0rk.;Host=167.114.86.107;Port=5432;Database=DEMO;";

            Configuration.Data.DatabaseConnections.push(dcd);
            
        }
    }

    public static function Process(PostData:String):String {
        var DecryptedString:String = "";
        var req:RequestMessage = null;
        var LogicModule:String = "";
        var LogicClass:String = "";
        var LogicMethod:String = "";
        var Name:String = "";

        try {
            if (Configuration.Data == null) {
                Init();
            }
//var PostBytes:Array<Int> = Base64.decode(PostData);
//var DecryptedBytes:Array<Int> = AES.decrypt(key, PostBytes, "CBC", iv);
//DecryptedString = UTF8.bytesToText(PKCS7.unpad(DecryptedBytes));
//var unserializer:Unserializer = new Unserializer(DecryptedString);

            var unserializer:Unserializer = new Unserializer(PostData);
            req = unserializer.unserialize();
            var res:ResponseMessage = new ResponseMessage();
            var Results = new Array<MethodResult>();

            for (mc in req.Calls) {
                LogicModule = mc.LogicModule;
                LogicClass = mc.LogicClass;
                LogicMethod = mc.LogicMethod;
                Name = mc.Name;

                var cl = Type.resolveClass(mc.LogicClass);
                if (cl == null) {
                    ExceptionManager.LogicException(1, "Clase no encontrada. Revise actualización de módulos");
                }
                var obj:Dynamic = Type.createInstance(cl, []);


                var mt = Reflect.field(obj, mc.LogicMethod);
                if (mt == null) {
                    ExceptionManager.LogicException(2, "Método no encontrado. Revise actualización de módulos");
                }
                var ret:Dynamic = Reflect.callMethod(obj, mt, mc.LogicParams);

                var mr = new MethodResult();
                mr.Name = mc.Name;
                mr.ReturnObject = ret;

                Results.push(mr);

                obj.Dispose();
            }

            LogicModule = "";
            LogicClass = "";
            LogicMethod = "";
            Name = "OK";

            res.RequestSeq = req.RequestSeq;
            res.Results = Results;
            var serializer = new Serializer();
            serializer.serialize(res);

            var ResponseString:String = serializer.toString();

            return ResponseString;

//            var ResponseBytes:Array<Int> = PKCS7.pad(UTF8.textToBytes(ResponseString), 16);
//            var EncryptedBytes:Array<Int> = AES.encrypt(key, ResponseBytes, "CBC", iv);
//            var PostReturn:String = Base64.encode(EncryptedBytes);
//            return PostReturn;
        }
        catch (ex:LogicException) {
            return ExceptionManager.HandleException(ex);
        }
        catch (ex:SecurityException) {
            return ExceptionManager.HandleException(ex);
        }
        catch (ex:Exception) {
            return ExceptionManager.HandleException(ex, DecryptedString, req, LogicModule, LogicClass, LogicMethod, Name);
        }
        catch (ex:Dynamic) {
            return ExceptionManager.HandleException(Exception.wrap(ex), DecryptedString, req, LogicModule, LogicClass, LogicMethod, Name);
        }

        return "";
    }

    public static function main() {
        Type.getClass(LibReference);
    }
}
