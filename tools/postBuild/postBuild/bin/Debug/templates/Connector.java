package cornerstone.orm.client;

import haxe.root.*;

@SuppressWarnings(value={"rawtypes", "unchecked"})
public class Connector extends haxe.lang.HxObject
{
	static
	{
		//line 8 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		cornerstone.orm.client.Connector.__rtti = "<class path=\"cornerstone.orm.client.Connector\" params=\"\">\n\t<Execute public=\"1\" set=\"method\" line=\"9\" static=\"1\"><f a=\"SQL:Parameters\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<unknown/>\n</f></Execute>\n\t<Queue public=\"1\" set=\"method\" line=\"13\" static=\"1\"><f a=\"SQL:Parameters:queue\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.QueueItem\"/></c>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.QueueItem\"/></c>\n</f></Queue>\n\t<ExecuteQueue public=\"1\" set=\"method\" line=\"25\" static=\"1\"><f a=\"queue\">\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.QueueItem\"/></c>\n\t<x path=\"Void\"/>\n</f></ExecuteQueue>\n\t<ReadValue public=\"1\" set=\"method\" line=\"31\" static=\"1\"><f a=\"SQL:Parameters\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<d/>\n</f></ReadValue>\n\t<ReadTable public=\"1\" set=\"method\" line=\"35\" static=\"1\"><f a=\"SQL:Parameters\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<c path=\"cornerstone.orm.client.DataTable\"/>\n</f></ReadTable>\n\t<Read public=\"1\" params=\"T\" set=\"method\" line=\"40\" static=\"1\"><f a=\"SQL:Parameters:cl\">\n\t<c path=\"String\"/>\n\t<c path=\"Array\"><c path=\"cornerstone.orm.client.Parameter\"/></c>\n\t<x path=\"Class\"><c path=\"Read.T\"/></x>\n\t<c path=\"Array\"><c path=\"Read.T\"/></c>\n</f></Read>\n\t<meta>\n\t\t<m n=\":directlyUsed\"/>\n\t\t<m n=\":keepSub\"/>\n\t\t<m n=\":rtti\"/>\n\t</meta>\n</class>";
	}
	
	public Connector(haxe.lang.EmptyObject empty)
	{
	}
	
	
	public Connector()
	{
		//line 8 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		cornerstone.orm.client.Connector.__hx_ctor_mainder_orm_client_Connector(this);
	}
	
	
	public static void __hx_ctor_mainder_orm_client_Connector(cornerstone.orm.client.Connector __temp_me74)
	{
	}
	
	
	public static java.lang.String __rtti;
	
	public static java.lang.Object Execute(java.lang.String SQL, haxe.root.Array<cornerstone.orm.client.Parameter> Parameters)
	{
		//line 10 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		return null;
	}
	
	
	public static haxe.root.Array<cornerstone.orm.client.QueueItem> Queue(java.lang.String SQL, haxe.root.Array<cornerstone.orm.client.Parameter> Parameters, haxe.root.Array<cornerstone.orm.client.QueueItem> queue)
	{
		//line 14 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		if (( queue == null )) 
		{
			//line 15 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
			queue = new haxe.root.Array<cornerstone.orm.client.QueueItem>();
		}
		
		//line 17 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		cornerstone.orm.client.QueueItem item = new cornerstone.orm.client.QueueItem();
		//line 18 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		item.set_SQL(SQL);
		//line 19 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		item.set_Parameters(Parameters);
		//line 20 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		queue.push(item);
		//line 22 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		return queue;
	}
	
	
	public static void ExecuteQueue(haxe.root.Array<cornerstone.orm.client.QueueItem> queue)
	{
		//line 26 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		int _g = 0;
		//line 26 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		while (( _g < queue.length ))
		{
			//line 26 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
			cornerstone.orm.client.QueueItem item = queue.__get(_g);
			//line 26 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
			 ++ _g;
			//line 27 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
			cornerstone.orm.client.Connector.Execute(item.get_SQL(), item.get_Parameters());
		}
		
	}
	
	
	public static java.lang.Object ReadValue(java.lang.String SQL, haxe.root.Array<cornerstone.orm.client.Parameter> Parameters)
	{
		//line 32 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		return null;
	}
	
	
	public static cornerstone.orm.client.DataTable ReadTable(java.lang.String SQL, haxe.root.Array<cornerstone.orm.client.Parameter> Parameters)
	{
		//line 36 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		cornerstone.orm.client.DataTable dt = new cornerstone.orm.client.DataTable();
		//line 37 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		return dt;
	}
	
	
	public static <T> haxe.root.Array<T> Read(java.lang.String SQL, haxe.root.Array<cornerstone.orm.client.Parameter> Parameters, java.lang.Class cl)
	{
		//line 41 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		haxe.root.Array<T> arr = new haxe.root.Array<T>();
		//line 42 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		return arr;
	}
	
	
	public static java.lang.Object __hx_createEmpty()
	{
		//line 8 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		return new cornerstone.orm.client.Connector(haxe.lang.EmptyObject.EMPTY);
	}
	
	
	public static java.lang.Object __hx_create(haxe.root.Array arr)
	{
		//line 8 "D:\\Workspace_Mainder\\v3.1\\modules\\cornerstone.orm\\src\\cornerstone\\orm\\client\\Connector.hx"
		return new cornerstone.orm.client.Connector();
	}
	
	
}


