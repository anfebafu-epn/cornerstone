package cornerstone.integrator.configuration;
import cornerstone.integrator.exceptions.ExceptionManager;
@:keepSub @:rtti
class ConfigurationData {
    public function new() {
    }

    @:isVar public var LogFolder(get, set):String;
    @:isVar public var CurrentDeployEnvironment(get, set):DeployEnvironmentEnum;
    @:isVar public var DatabaseConnections(get, set):Array<DatabaseConnectionData>;

    function get_LogFolder():String {
        return LogFolder;
    }

    function set_LogFolder(value:String) {
        return this.LogFolder = value;
    }

    function get_CurrentDeployEnvironment():DeployEnvironmentEnum {
        return CurrentDeployEnvironment;
    }

    function set_CurrentDeployEnvironment(value:DeployEnvironmentEnum) {
        return this.CurrentDeployEnvironment = value;
    }

    function set_DatabaseConnections(value:Array<DatabaseConnectionData>) {
        return this.DatabaseConnections = value;
    }

    function get_DatabaseConnections():Array<DatabaseConnectionData> {
        return DatabaseConnections;
    }

    public function get_CurrentDatabaseConnection():DatabaseConnectionData {
        var res = DatabaseConnections.filter(function(d) { return d.DeploEnvironment == CurrentDeployEnvironment; });
        if (res.length == 0)
        {
            ExceptionManager.LogicException(100, "No hay configuración de conexión a la base de datos en el ambiente deploy actual");
        }
        return res[0];
    }
}
