package cornerstone.integrator.configuration;
@:keepSub @:rtti
class DatabaseConnectionData {
    public function new() {
    }

    @:isVar public var DatabaseEngine(get, set):DatabaseEngineEnum;
    @:isVar public var DeploEnvironment(get, set):DeployEnvironmentEnum;
    @:isVar public var ConnectionString(get, set):String;
    @:isVar public var DataSource(get, set):String;
    @:isVar public var ServiceName(get, set):String;
    @:isVar public var SID(get, set):String;
    @:isVar public var UserID(get, set):String;
    @:isVar public var Password(get, set):String;
    @:isVar public var Database(get, set):String;
    @:isVar public var OtherParameters(get, set):Array<String>;

    function get_DatabaseEngine():DatabaseEngineEnum {
        return DatabaseEngine;
    }

    function set_DatabaseEngine(value:DatabaseEngineEnum) {
        return this.DatabaseEngine = value;
    }

    function get_DeploEnvironment():DeployEnvironmentEnum {
        return DeploEnvironment;
    }

    function set_DeploEnvironment(value:DeployEnvironmentEnum) {
        return this.DeploEnvironment = value;
    }

    function get_ConnectionString():String {
        return ConnectionString;
    }

    function set_ConnectionString(value:String) {
        return this.ConnectionString = value;
    }

    function set_DataSource(value:String) {
        return this.DataSource = value;
    }

    function get_DataSource():String {
        return DataSource;
    }

    function set_ServiceName(value:String) {
        return this.ServiceName = value;
    }

    function get_ServiceName():String {
        return ServiceName;
    }

    function set_SID(value:String) {
        return this.SID = value;
    }

    function get_SID():String {
        return SID;
    }

    function set_UserID(value:String) {
        return this.UserID = value;
    }

    function get_UserID():String {
        return UserID;
    }

    function get_Password():String {
        return Password;
    }

    function set_Password(value:String) {
        return this.Password = value;
    }

    function get_Database():String {
        return Database;
    }

    function set_Database(value:String) {
        return this.Database = value;
    }

    function set_OtherParameters(value:Array<String>) {
        return this.OtherParameters = value;
    }

    function get_OtherParameters():Array<String> {
        return OtherParameters;
    }


}
