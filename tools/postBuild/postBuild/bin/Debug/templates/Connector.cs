
#pragma warning disable 109, 114, 219, 429, 168, 162
using System.Data.Common;
using haxe.io;
namespace cornerstone.orm.client
{
    public class Connector : global::haxe.lang.HxObject
    {

        static Connector()
        {
            global::cornerstone.orm.client.Connector.__rtti = "<class path=\"cornerstone.orm.client.Connector\" params=\"\">\n\t<Execute public=\"1\" set=\"method\" line=\"9\" static=\"1\"><f a=\"SQL:Parameters\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<unknown/>\n</f></Execute>\n\t<ReadValue public=\"1\" set=\"method\" line=\"15\" static=\"1\"><f a=\"SQL:Parameters\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<d/>\n</f></ReadValue>\n\t<ReadTable public=\"1\" set=\"method\" line=\"21\" static=\"1\"><f a=\"SQL:Parameters\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<c path=\"cornerstone.orm.client.DataTable\"/>\n</f></ReadTable>\n\t<Read public=\"1\" params=\"T\" set=\"method\" line=\"56\" static=\"1\"><f a=\"SQL:Parameters:cl\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<x path=\"Class\"><c path=\"Read.T\"/></x>\n\t<c path=\"Array\"><c path=\"Read.T\"/></c>\n</f></Read>\n\t<meta>\n\t\t<m n=\":directlyUsed\"/>\n\t\t<m n=\":keepSub\"/>\n\t\t<m n=\":rtti\"/>\n\t</meta>\n</class>";
        }

        public Connector(global::haxe.lang.EmptyObject empty)
        {
        }

        public Connector()
        {
            global::cornerstone.orm.client.Connector.__hx_ctor_mainder_orm_client_Connector(this);
        }

        public static void __hx_ctor_mainder_orm_client_Connector(global::cornerstone.orm.client.Connector __temp_me94)
        {
        }

        public static string __rtti;

        private static object Run<T>(string SQL, global::Array<object> Parameters, OperationEnum operation, global::System.Type cl)
        {
            object Result = null;
            //try
            //{
            unchecked
            {
                global::cornerstone.integrator.configuration.DatabaseConnectionData ConnectionData = global::cornerstone.integrator.configuration.Configuration.Data.get_CurrentDatabaseConnection();
                string ConnectionString = ConnectionData.get_ConnectionString();

                string provider = ReadProvider(ConnectionData);

                // Create the DbProviderFactory and DbConnection.
                DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
                DbConnection connection = factory.CreateConnection();
                connection.ConnectionString = ConnectionString;

                using (connection)
                {
                    // Create the DbCommand.
                    DbCommand command = factory.CreateCommand();
                    command.CommandText = SQL;
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    //Define Parameters
                    if (Parameters != null)
                    {
                        object par;
                        int _g1 = 0;

                        while ((_g1 < Parameters.length))
                        {
                            Parameter p = (Parameter)Parameters[_g1];
                            DbParameter param = factory.CreateParameter();
                            param.ParameterName = p.ParameterName;
                            param.DbType = ConvertToDBType(p);
                            param.Direction = ConvertToDBDirection(p);
                            param.Value = ConvertToDBValue(p);
                            command.Parameters.Add(param);
                            _g1++;
                        }
                    }

                    connection.Open();

                    switch (operation)
                    {
                        case OperationEnum.NonQuery:
                            Result = command.ExecuteNonQuery();
                            break;

                        case OperationEnum.Scalar:
                            Result = command.ExecuteScalar();
                            break;

                        case OperationEnum.Datatable:

                            DbDataAdapter adapter = factory.CreateDataAdapter();
                            adapter.SelectCommand = command;
                            System.Data.DataTable dt = new System.Data.DataTable();
                            adapter.Fill(dt);
                            Result = ConvertToHaxeDataTable(dt);

                            break;

                        case OperationEnum.ObjectArray:

                            DbDataReader reader = command.ExecuteReader();
                            Array<T> array = new Array<T>();
                            global::Array<object> _g2 = Fields(cl);

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    T item = global::Type.createInstance<T>(cl, new global::Array<object>(new object[] { }));

                                    // mapeo al objeto
                                    int _g1 = 0;

                                    while ((_g1 < _g2.length))
                                    {
                                        string field = global::haxe.lang.Runtime.toString(_g2[_g1]);
                                        ++_g1;
                                        global::Reflect.setField(item, field, reader.GetValue(reader.GetOrdinal(field)));
                                    }

                                    array.push(item);
                                }
                            }

                            Result = array;

                            break;

                        case OperationEnum.SingleObject:

                            DbDataReader reader1 = command.ExecuteReader();

                            global::Array<object> _g3 = Fields(cl);

                            if (reader1.HasRows)
                            {
                                while (reader1.Read())
                                {
                                    T item = global::Type.createInstance<T>(cl, new global::Array<object>(new object[] { }));

                                    // mapeo al objeto
                                    int _g1 = 0;

                                    while ((_g1 < _g3.length))
                                    {
                                        string field = global::haxe.lang.Runtime.toString(_g3[_g1]);
                                        ++_g1;
                                        global::Reflect.setField(item, field, reader1.GetValue(reader1.GetOrdinal(field)));
                                    }

                                    return item;

                                }
                            }

                            return null;

                    }

                    connection.Close();
                }
            }
            //}
            //catch (System.Exception ex)
            //{
            //    global::haxe.lang.Exceptions.exception = ex;
            //    object pos = new { className = "LogicService", methodName = "Execute", fileName = "Connector.cs", lineNumber = 0 };
            //    cornerstone.integrator.exceptions.ExceptionManager.HandleException(global::Exception.wrap(ex, pos), SQL , null, null, null, null, null);
            //}
            return Result;
        }

        private static object Run(global::Array<object> queue)
        {
            object Result = null;
            //try
            //{
            unchecked
            {
                global::cornerstone.integrator.configuration.DatabaseConnectionData ConnectionData = global::cornerstone.integrator.configuration.Configuration.Data.get_CurrentDatabaseConnection();
                string ConnectionString = ConnectionData.get_ConnectionString();

                string provider = ReadProvider(ConnectionData);


                // Create the DbProviderFactory and DbConnection.
                DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
                DbConnection connection = factory.CreateConnection();
                connection.ConnectionString = ConnectionString;

                DbCommand[] commands = new DbCommand[queue.length];

                int _g = 0;
                while ((_g < queue.length))
                {
                    global::cornerstone.orm.client.QueueItem item = ((global::cornerstone.orm.client.QueueItem)(queue[_g]));
                    ++_g;
                    //global::cornerstone.orm.client.Connector.Execute(item.get_SQL(), item.get_Parameters());

                    // Create the DbCommand.
                    DbCommand command = factory.CreateCommand();
                    command.CommandText = item.get_SQL();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    //Define Parameters
                    if (item.get_Parameters() != null)
                    {
                        foreach (object par in item.get_Parameters().__a)
                        {
                            Parameter p = (Parameter)par;
                            DbParameter param = factory.CreateParameter();
                            param.ParameterName = p.ParameterName;
                            param.DbType = ConvertToDBType(p);
                            param.Direction = ConvertToDBDirection(p);
                            param.Value = ConvertToDBValue(p);
                            command.Parameters.Add(param);
                        }
                    }
                    commands[_g] = command;
                }


                using (connection)
                {
                    connection.Open();
                    foreach (DbCommand command in commands)
                        command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            //}
            //catch (System.Exception ex)
            //{
            //    global::haxe.lang.Exceptions.exception = ex;
            //    object pos = new { className = "LogicService", methodName = "Execute", fileName = "Connector.cs", lineNumber = 0 };
            //    cornerstone.integrator.exceptions.ExceptionManager.HandleException(global::Exception.wrap(ex, pos), SQL , null, null, null, null, null);
            //}
            return Result;
        }

        public static Array<object> Fields(global::System.Type c)
        {
            Array<object> array = new Array<object>(new object[0]);
            var fields = c.GetFields();

            int num = 0;
            int length = fields.Length;
            while (num < length)
            {
                int num1 = num;
                num = num1 + 1;
                string name = fields[num1].Name;
                if (name.StartsWith("__hx_") || name == "__rtti")
                {
                    continue;
                }
                array.push(name);
            }
            return array;
        }

        public static object Execute(string SQL, global::Array<object> Parameters)
        {
            return Run<object>(SQL, Parameters, OperationEnum.NonQuery, typeof(string));
        }

        public static global::Array<object> Queue(string SQL, global::Array<object> Parameters, global::Array<object> queue)
        {
            if ((queue == null))
            {
                queue = new global::Array<object>();
            }

            global::cornerstone.orm.client.QueueItem item = new global::cornerstone.orm.client.QueueItem();
            item.set_SQL(SQL);
            item.set_Parameters(Parameters);
            queue.push(item);
            return queue;
        }

        public static void ExecuteQueue(global::Array<object> queue)
        {
            Run(queue);
        }

        public static object ReadValue(string SQL, global::Array<object> Parameters)
        {
            return Run<object>(SQL, Parameters, OperationEnum.Scalar, typeof(string));
        }

        public static global::cornerstone.orm.client.DataTable ReadTable(string SQL, global::Array<object> Parameters)
        {
            return (global::cornerstone.orm.client.DataTable)Run<object>(SQL, Parameters, OperationEnum.Datatable, typeof(string));
        }

        public static global::Array<T> Read<T>(string SQL, global::Array<object> Parameters, global::System.Type cl, global::cornerstone.orm.ORM orm)
        {
            global::Array<T> arr = (global::Array<T>)Run<T>(SQL, Parameters, OperationEnum.ObjectArray, cl);
            foreach (T obj in arr.__a)
            {
                if (obj is global::cornerstone.orm.client.ModelObject)
                {
                    global::cornerstone.orm.client.ModelObject mo = (global::cornerstone.orm.client.ModelObject)(object)obj;
                    
                    mo.ObjectEditType = common.enums.ObjectEditTypeEnum.NOCHANGE;
                    mo.ObjectSource = common.enums.ObjectSourceTypeEnum.DB;
                    if (orm != null)
                    {
                        orm.Objects.push(obj);
                        mo.TrackChanges = true;
                    }
                    else
                    {
                        mo.TrackChanges = false;
                    }
                }
            }
            return arr;
        }

        public static T ReadOne<T>(string SQL, global::Array<object> Parameters, global::System.Type cl, global::cornerstone.orm.ORM orm)
        {
            T obj = (T)Run<T>(SQL, Parameters, OperationEnum.SingleObject, cl);
            if (obj is global::cornerstone.orm.client.ModelObject)
            {
                global::cornerstone.orm.client.ModelObject mo = (global::cornerstone.orm.client.ModelObject)(object)obj;
                mo.ObjectEditType = common.enums.ObjectEditTypeEnum.NOCHANGE;
                mo.ObjectSource = common.enums.ObjectSourceTypeEnum.DB;
                if (orm != null)
                {
                    orm.Objects.push(obj);
                    mo.TrackChanges = true;
                }
                else
                {
                    mo.TrackChanges = false;
                }
            }
            return obj;
        }

        private static string ReadProvider(global::cornerstone.integrator.configuration.DatabaseConnectionData ConnectionData)
        {
            string provider = "System.Data.SqlClient";
            {
                global::cornerstone.integrator.configuration.DatabaseEngineEnum _g1 = ConnectionData.get_DatabaseEngine();
                switch (_g1.index)
                {
                    case 4: // GENERIC, ODBC
                        {
                            provider = "System.Data.Odbc";
                            break;
                        }


                    case 2: // MYSQL
                        {
                            provider = "MySql.Data.MySqlClient";
                            break;
                        }


                    case 1: // ORACLE
                        {
                            provider = "Oracle.ManagedDataAccess.Client";
                            break;
                        }


                    case 3: //POSTGRES
                        {
                            provider = "Devart.Data.PostgreSql";
                            break;
                        }


                    case 0: // SQL SERVER
                        {
                            provider = "System.Data.SqlClient";
                            break;
                        }

                }

            }
            return provider;
        }

        private static System.Data.DbType ConvertToDBType(Parameter p)
        {
            {
                global::cornerstone.orm.common.enums.DBTypeEnum _g1 = p.DataType;
                switch (_g1.index)
                {
                    case 0: // int
                        return System.Data.DbType.Int32;
                    case 1: // long
                        return System.Data.DbType.Int64;
                    case 2: // varchar
                        return System.Data.DbType.String;
                    case 3: // float
                        return System.Data.DbType.Double;
                    case 4: // decimal
                        return System.Data.DbType.Decimal;
                    case 5: // datetime
                        return System.Data.DbType.DateTime;
                    case 6: // bool
                        return System.Data.DbType.Boolean;
                    case 7: // blob
                        return System.Data.DbType.Binary;
                    case 8: // uuid
                        return System.Data.DbType.Guid;
                    default:
                        return System.Data.DbType.String;
                }
            }
        }

        private static System.Data.ParameterDirection ConvertToDBDirection(Parameter p)
        {
            {
                global::cornerstone.orm.common.enums.ParameterDirectionEnum _g1 = p.ParameterDirection;
                if (_g1 == null)
                    return System.Data.ParameterDirection.Input;

                switch (_g1.index)
                {
                    case 0: //INPUT
                        return System.Data.ParameterDirection.Input;
                    case 1: // OUTPUT
                        return System.Data.ParameterDirection.Output;
                    case 2: //INPUTOUTPUT
                        return System.Data.ParameterDirection.InputOutput;
                    case 3: //RETURN VALUE
                        return System.Data.ParameterDirection.ReturnValue;
                    default:
                        return System.Data.ParameterDirection.Input;
                }
            }
        }

        private static object ConvertToDBValue(Parameter p)
        {
            if (p.Value is Date)
                return ((Date)p.Value).date;

            if (p.Value is Bytes)
                return ((Bytes)p.Value).b;

            // GUID
            int dbindex = global::cornerstone.integrator.configuration.Configuration.Data.get_CurrentDatabaseConnection().DatabaseEngine.index;
            if (dbindex == 0 || dbindex == 1) // oracle o SQL Server reciben el UUID en binario, las otras bases lo almacenan en string
            {
                if (p.DataType.index == 8 && p.Value is string)
                    return new System.Guid(p.Value.ToString());
            }

            return p.Value;
        }
        
        private static object ConvertToHaxeDataTable(System.Data.DataTable dt)
        {
            DataTable hdt = new DataTable();

            foreach (System.Data.DataColumn dc in dt.Columns)
            {
                DataColumn col = new DataColumn();
                col.ColumnIndex = dc.Ordinal;
                col.ColumnName = dc.ColumnName;
                col.ColumnType = ConvertToHaxeType(dc.DataType);
                hdt.Columns.push(col);
            }

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                global::Array<object> arr = new Array<object>(dr.ItemArray);
                DataRow row = new DataRow();
                row.set_Items(arr);
                hdt.Rows.push(row);
            }

            return hdt;
        }

        private static common.enums.DBTypeEnum ConvertToHaxeType(System.Type type)
        {
            if (type == typeof(int)) return common.enums.DBTypeEnum.INTEGER;
            if (type == typeof(long)) return common.enums.DBTypeEnum.BIGINTEGER;
            if (type == typeof(string)) return common.enums.DBTypeEnum.VARCHAR;
            if (type == typeof(double)) return common.enums.DBTypeEnum.FLOAT;
            if (type == typeof(decimal)) return common.enums.DBTypeEnum.DECIMAL;
            if (type == typeof(System.DateTime)) return common.enums.DBTypeEnum.DATETIME;
            if (type == typeof(bool)) return common.enums.DBTypeEnum.BOOLEAN;
            if (type == typeof(byte[])) return common.enums.DBTypeEnum.BINARY;
            if (type == typeof(System.Guid)) return common.enums.DBTypeEnum.UUID;

            return common.enums.DBTypeEnum.VARCHAR;
        }

        public static new object __hx_createEmpty()
        {
            return new global::cornerstone.orm.client.Connector(global::haxe.lang.EmptyObject.EMPTY);
        }
        
        public static new object __hx_create(global::Array arr)
        {
            return new global::cornerstone.orm.client.Connector();
        }
    }

    public enum OperationEnum
    {
        NonQuery,
        Scalar,
        Datatable,
        ObjectArray,
        SingleObject
    }
}


