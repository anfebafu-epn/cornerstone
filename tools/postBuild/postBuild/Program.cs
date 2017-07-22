using Ionic.Zip;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace postBuild
{
    class Program
    {
        private static bool IsIntegrator = false;
        private static Dictionary<string, string> Params = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                string[] parts = arg.Split('=');
                Params.Add(parts[0], parts[1]);
            }

            string AssemblyName = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory()).Name;
            string ModuleName = Params["module"];
            if (ModuleName == "integrator") IsIntegrator = true;

            // Instalador de los módulos, la instalación del integrador es diferente
            if (!IsIntegrator)
            {
                #region NET
                // borro el directorio de link de CS (linked)
                try
                {
                    System.IO.Directory.Delete(System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\", true);
                }
                catch { }

                // copio el directorio compilado al directorio linked
                DirectoryCopy(System.IO.Directory.GetCurrentDirectory() + "\\out\\net\\",
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\", true);

                // renderización de WebServices
                string WSContent = GenerateWSHeader(ModuleName) + RenderWS(ModuleName) + GenerateWSFooter();

                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\src\\cornerstone\\" + ModuleName + "\\WS.cs",
                    WSContent);

                // Generar AssemblyInfo
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\src\\cornerstone\\" + ModuleName + "\\AssemblyInfo.cs", GenerarAssemblyInfo("cornerstone." + ModuleName, "cornerstone." + ModuleName, "", "cornerstone." + ModuleName, "", "Copyright " + DateTime.Now.Year.ToString(), "", Guid.NewGuid().ToString()));

                // genero el nuevo csproj en el directorio linked, arreglando las referencias para evitar duplicidad de clases
                string CSProjFileName = System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\" + AssemblyName + ".csproj";
                string CsProjContent = GenerarCsProj(AssemblyName, System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\src\\cornerstone\\" + ModuleName + "\\");
                System.IO.File.WriteAllText(CSProjFileName, CsProjContent);


                // compilo el nuevo csproj
                string projectFileName = CSProjFileName;
                ProjectCollection pc = new ProjectCollection();
                Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                GlobalProperty.Add("Configuration", "Release");
                GlobalProperty.Add("Platform", "AnyCPU");
                BuildRequestData BuidlRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Build" }, null);
                BuildResult buildResult = BuildManager.DefaultBuildManager.Build(new BuildParameters(pc), BuidlRequest);

                // traslado la dll final compilada al server.net para que pueda ejecutarse
                System.IO.File.Copy(
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\bin\\Release\\" + AssemblyName + ".dll",
                    System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.net\\server\\Bin\\" + AssemblyName + ".dll",
                    true
                    );

                string ASMX = "<%@ WebService Language=\"C#\" Class=\"cornerstone." + ModuleName + ".WS\" %>";
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.net\\server\\" + AssemblyName + ".asmx", ASMX);

                string SVC = "<%@ ServiceHost Service=\"cornerstone." + ModuleName + ".WS\" %>";
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.net\\server\\" + AssemblyName + ".svc", SVC);

                #endregion

                #region JAVA

                // SENTENCIAS PARA RECOMPILAR Y REEMPAQUETAR
                //dir *.java /s /b > FilesList.txt
                //javac -cp lib\cornerstone.integrator.jar;lib\cornerstone.orm.jar @cmd -d obj
                //jar cvf uuu.jar -C obj .

                // borro el directorio de link de JAVA (linked)
                try
                {
                    System.IO.Directory.Delete(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\", true);
                }
                catch { }

                // creo el directorio Linked
                if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\"))
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\");

                DirectoryCopy(System.IO.Directory.GetCurrentDirectory() + "\\out\\jar\\src\\",
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\src\\", true);


                // creación del archivo de compilación
                StringBuilder cmd = new StringBuilder();
                Createcmd(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\src\\", ModuleName, cmd);
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\cmd", cmd.ToString());

                // copio las librerías integrator y orm
                if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\lib\\"))
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\lib\\");

                System.IO.File.Copy(System.IO.Directory.GetCurrentDirectory() + "\\..\\cornerstone.integrator\\out\\jarlinked\\cornerstone.integrator.jar",
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\lib\\cornerstone.integrator.jar", true);

                // creo carpeta para recibir la compilación
                if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\obj\\"))
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\obj\\");

                string JDKPath = ConfigurationManager.AppSettings["JDKPath"];

                // ejecuto la compilación
                {

                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = JDKPath + "\\bin\\javac.exe";
                    psi.Arguments = " -cp lib/cornerstone.integrator.jar @cmd -d obj";
                    psi.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\";
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    Process p = Process.Start(psi);
                    p.WaitForExit();
                }
                // ensamblo el jar
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = JDKPath + "\\bin\\jar.exe";
                    psi.Arguments = " cf cornerstone." + ModuleName + ".jar -C obj .";
                    psi.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\";
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    Process p = Process.Start(psi);
                    p.WaitForExit();
                }

                // crea el directorio Lib en el server, si no existe
                if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.java\\web\\WEB-INF\\lib\\"))
                {
                    Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.java\\web\\WEB-INF\\lib\\");
                }

                // traslado el jar al server.java
                System.IO.File.Copy(
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\" + AssemblyName + ".jar",
                    System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.java\\web\\WEB-INF\\lib\\" + AssemblyName + ".jar",
                    true
                    );


                #endregion

                #region PHP

                if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.php\\lib\\cornerstone\\" + ModuleName + "\\"))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.php\\lib\\cornerstone\\" + ModuleName + "\\");
                }

                // copio el directorio compilado al directorio linked
                DirectoryCopy(System.IO.Directory.GetCurrentDirectory() + "\\out\\php\\lib\\cornerstone\\" + ModuleName + "\\",
                    System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.php\\lib\\cornerstone\\" + ModuleName + "\\", true);


                #endregion

                #region JS Proxy

                try
                {
                    // crear archivo proxy
                    string bin = System.IO.Directory.GetCurrentDirectory() + "\\out\\net\\bin\\";

                    DirectoryInfo oDirectoryInfo = new DirectoryInfo(bin);
                    Assembly asm = null;

                    //Instanciación del Assembly del proyecto
                    if (oDirectoryInfo.Exists)
                    {
                        //Foreach Assembly with dll as the extension
                        foreach (FileInfo oFileInfo in oDirectoryInfo.GetFiles("*.dll", SearchOption.AllDirectories))
                        {

                            Assembly tempAssembly = null;

                            //Before loading the assembly, check all current loaded assemblies in case talready loaded
                            //has already been loaded as a reference to another assembly
                            //Loading the assembly twice can cause major issues
                            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                //Check the assembly is not dynamically generated as we are not interested in these
                                if (loadedAssembly.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                                {
                                    //Get the loaded assembly filename
                                    string sLoadedFilename =
                                        loadedAssembly.CodeBase.Substring(loadedAssembly.CodeBase.LastIndexOf('/') + 1);

                                    //If the filenames match, set the assembly to the one that is already loaded
                                    if (sLoadedFilename.ToUpper() == oFileInfo.Name.ToUpper())
                                    {
                                        tempAssembly = loadedAssembly;
                                        break;
                                    }
                                }
                            }

                            //If the assembly is not aleady loaded, load it manually
                            if (tempAssembly == null)
                            {
                                tempAssembly = Assembly.LoadFile(oFileInfo.FullName);
                            }

                            asm = tempAssembly;
                        }

                    }



                    // se itera por los tipos y métodos, buscando las funciones de lógica "Logic_"
                    var LogicTypes = (from t in asm.GetTypes()
                                      from m in t.GetMethods()
                                      where t.IsSealed == true &&
                                        t.FullName.Contains("cornerstone." + ModuleName) &&
                                       m.Name.StartsWith("Logic_")
                                      select t).Distinct();

                    StringBuilder sbMethods = new StringBuilder();
                    List<string> ClassesToRender = new List<string>();


                    foreach (var LogicType in LogicTypes)
                    {
                        // se instancia la clase de Lógica encontrada
                        //var LogicInstance = System.Reflection.Assembly.GetAssembly(LogicType).CreateInstance(LogicType.FullName);

                        //// Leo el RTTI
                        //if (LogicInstance.GetType().GetField("__rtti") != null)
                        //{

                        var fieldn = LogicType.GetField("__rtti");
                        if (fieldn != null)
                        {
                            var xml = fieldn.GetValue(null).ToString();
                            // obtengo el XML
                            //var xml = LogicInstance.GetType().GetField("__rtti").GetValue(LogicInstance).ToString();

                            // lo parseo para leerlo con Linq
                            XDocument doc = XDocument.Parse(xml);

                            // obtengo los métodos declarados en el XML
                            var methods = from nclass in doc.Elements("class")
                                          from nmethod in nclass.Elements()
                                          where nmethod.Name != "meta" &&
                                                nmethod.Name != "implements" &&
                                                nmethod.Attribute("set").Value == "method" &&
                                                nmethod.Name.ToString().StartsWith("Logic_")
                                          select nmethod;

                            // obtención de la lista de clases
                            foreach (var method in methods)
                            {
                                //// barro los parámetros
                                foreach (var par in method.Element("f").Elements())
                                {
                                    string strType = par.Attribute("path").Value;
                                    if (IsCustomClass(par))
                                    {
                                        if (strType == "List" || strType == "Array")
                                        {
                                            foreach (var t in par.Elements().Where(e => e.Attributes().Count(a => a.Name == "path") > 0 && IsCustomClass(e)))
                                            {
                                                if (IsCustomClass(t))
                                                {
                                                    if (!ClassesToRender.Contains(t.Attribute("path").Value))
                                                    {
                                                        ClassesToRender.Add(t.Attribute("path").Value);
                                                        SubTypeCheck(t.Attribute("path").Value, asm, ClassesToRender);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!ClassesToRender.Contains(par.Attribute("path").Value))
                                            {
                                                ClassesToRender.Add(par.Attribute("path").Value);
                                                SubTypeCheck(par.Attribute("path").Value, asm, ClassesToRender);
                                            }
                                        }
                                    }
                                }
                            }

                            // generación de los métodos en TypeScript
                            foreach (var method in methods)
                            {
                                // obtengo la lista de nombres de parámetro
                                string[] nparams = method.Element("f").Attribute("a").Value.Split(':');
                                XElement[] xparams = method.Element("f").Elements().ToArray();

                                string ListOfParams = "";
                                string ListOfParamsWithTypes = "";
                                string ReturnType = "";


                                for (int i = 0; i < nparams.Length; i++)
                                {
                                    if (nparams[i] == "") continue;
                                    ListOfParams += nparams[i] + ", ";
                                    ListOfParamsWithTypes += nparams[i] + ":" + RenderParamforTS(xparams[i]) + ", ";
                                }


                                ReturnType = RenderParamforTS(xparams.Last());


                                sbMethods.Append("function Call_" + ModuleName + "_" + LogicType.Name + "_" + method.Name.ToString().Replace("Logic_", "") + "(" + ListOfParamsWithTypes + "name) {"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    var mcc = new cornerstone_integrator_transport_MethodCall();"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    mcc.set_LogicModule(\"" + ModuleName + "\");"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    mcc.set_LogicClass(\"" + LogicType.Namespace + "." + LogicType.Name + "\");"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    mcc.set_LogicMethod(\"" + method.Name.ToString() + "\");"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    mcc.set_LogicParams([" + (ListOfParams.Length > 2 ? ListOfParams.Substring(0, ListOfParams.Length - 2) : "") + "]);"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    mcc.set_Name(name);"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    return mcc; "); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("}"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("function Proxy_" + ModuleName + "_" + LogicType.Name + "_" + method.Name.ToString().Replace("Logic_", "") + "(" + ListOfParamsWithTypes + (ReturnType == "Void" ? "" : "callback : (n : " + ReturnType + ") => any") + ") {"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("    return ServiceCall(Call_" + ModuleName + "_" + LogicType.Name + "_" + method.Name.ToString().Replace("Logic_", "") + "(" + ListOfParams + "'N'), " + (ReturnType == "Void" ? "null" : "callback") + ");"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append("}"); sbMethods.Append(Environment.NewLine);
                                sbMethods.Append(Environment.NewLine);
                            }
                        }
                    }


                    StringBuilder sbClasses = new StringBuilder();
                    //ClassesToRender.Reverse();

                    foreach (var ClasstoRender in ClassesToRender)
                    {
                        TSRenderClasses(ClasstoRender, asm, sbClasses);
                    }

                    StringBuilder References = new StringBuilder();
                    References.AppendLine("/// <reference path=\"cornerstone.integrator.ts\"/>");

                    String ProxyContent = References.ToString() + Environment.NewLine + sbClasses.ToString() + Environment.NewLine + sbMethods.ToString();

                    System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.ts", ProxyContent, Encoding.UTF8);
                    // corrida automática del TypeScript para generar el JS
                    // se necesita instalado Microsoft SDKs. tsc.exe /Program Files/Microsoft SDKs/TypeScript/1.0/tsc.exe
                    // https://www.microsoft.com/en-us/download/confirmation.aspx?id=48593
                    try
                    {
                        TypeScriptCompiler.Compile(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.ts");
                    }
                    catch (InvalidTypeScriptFileException ex)
                    {
                        // there was a compiler error, show the compiler output
                        //Console.WriteLine(ex.Message);
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException ex)
                {
                    foreach (Exception i in ex.LoaderExceptions)
                        Console.WriteLine(i.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //// Minify
                //using (Task<String> task = JavaScripMinifier.MinifyJs(System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.js")))
                //{
                //    task.Wait();
                //    System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.min.js", task.Result);
                //}

                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.min.js",
                    JavaScripMinifier.MinifyJs3(System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.js")));


                #endregion

                #region Externs

                try
                {
                    string bin = System.IO.Directory.GetCurrentDirectory() + "\\out\\net\\bin\\";

                    DirectoryInfo oDirectoryInfo = new DirectoryInfo(bin);
                    Assembly asm = null;

                    //Instanciación del Assembly del proyecto
                    if (oDirectoryInfo.Exists)
                    {
                        //Foreach Assembly with dll as the extension
                        //foreach (FileInfo oFileInfo in oDirectoryInfo.GetFiles("Main.dll", SearchOption.AllDirectories))
                        //{

                        FileInfo oFileInfo = new FileInfo(oDirectoryInfo.FullName + "Main.dll");

                        Assembly tempAssembly = null;

                        //Before loading the assembly, check all current loaded assemblies in case talready loaded
                        //has already been loaded as a reference to another assembly
                        //Loading the assembly twice can cause major issues
                        foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            //Check the assembly is not dynamically generated as we are not interested in these
                            if (loadedAssembly.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                            {
                                //Get the loaded assembly filename
                                string sLoadedFilename =
                                    loadedAssembly.CodeBase.Substring(loadedAssembly.CodeBase.LastIndexOf('/') + 1);

                                //If the filenames match, set the assembly to the one that is already loaded
                                if (sLoadedFilename.ToUpper() == oFileInfo.Name.ToUpper())
                                {
                                    tempAssembly = loadedAssembly;
                                    break;
                                }
                            }
                        }

                        //If the assembly is not aleady loaded, load it manually
                        if (tempAssembly == null)
                        {
                            tempAssembly = Assembly.LoadFile(oFileInfo.FullName);
                        }

                        asm = tempAssembly;
                        //}

                    }

                    // Itero por todos los tipos del módulo
                    foreach (var LogicType in asm.GetTypes().Where(t => t.FullName.Contains("cornerstone." + ModuleName)))
                    {
                        if (asm.GetType(LogicType.FullName).IsSubclassOf(asm.GetType("haxe.lang.HxObject")))
                        {
                            // variable para consolidar el código interno de la clase
                            StringBuilder sbExtern = new StringBuilder();
                            string HXImplements = "";


                            // se instancia la clase de Lógica encontrada
                            //var LogicInstance = System.Reflection.Assembly.GetAssembly(LogicType).CreateInstance(LogicType.FullName);


                            // Leo el RTTI
                            //if (LogicInstance.GetType().GetField("__rtti") != null)
                            //{
                            var fieldn = LogicType.GetField("__rtti");
                            if (fieldn != null)
                            {
                                var xml = fieldn.GetValue(null).ToString();

                                // obtengo el XML
                                //var xml = LogicInstance.GetType().GetField("__rtti").GetValue(LogicInstance).ToString();

                                // lo parseo para leerlo con Linq
                                XDocument doc = XDocument.Parse(xml);

                                // creo la carpeta correcta
                                string InnerPath = "";
                                string[] Parts = LogicType.Namespace
                                    .Replace("cornerstone." + ModuleName, "")
                                    .Split('.');

                                foreach (var part in Parts)
                                    InnerPath += part + "\\";

                                if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\out\\rtti\\" + InnerPath))
                                    Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\out\\rtti\\" + InnerPath);

                                // escribo los rtti en las rutas correctas en formato xml
                                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\out\\rtti\\" + InnerPath + LogicType.Name + ".xml", xml);

                                // variable de clase
                                //var classinfo = from nclass in doc.Elements("class")

                                // obtengo los métodos declarados en el XML (dividir)
                                var fields = from nclass in doc.Elements("class")
                                             from nmethod in nclass.Elements()
                                                 //where nmethod.Name != "meta" &&
                                                 //      nmethod.Name != "implements"
                                                 //      //nmethod.Attribute("set").Value == "method"
                                             select nmethod;

                                // generación de los métodos 
                                foreach (var field in fields)
                                {
                                    if (field.Name == "implements") // campo de herencia
                                    {
                                        // en externs es preferible que no haya implements
                                        //HXImplements = "implements " + field.Attribute("path").Value;
                                    }
                                    else if (field.Name == "meta") // campo de metadata
                                    {
                                        // no es necesario leer la metadata de la clase
                                        continue;
                                    }
                                    // detección de método
                                    else if (field.Attributes().Count(a => a.Name == "set") > 0 && field.Attribute("set").Value == "method")
                                    {
                                        // obtengo la lista de nombres de parámetro
                                        string[] nparams = field.Element("f").Attribute("a").Value.Split(':');
                                        XElement[] xparams = field.Element("f").Elements().ToArray();

                                        string ListOfParams = "";
                                        string ListOfParamsWithTypes = "";
                                        string ReturnType = "";
                                        bool IsStatic = false;
                                        bool IsPublic = false;

                                        for (int i = 0; i < nparams.Length; i++)
                                        {
                                            if (nparams[i] == "") continue;
                                            ListOfParams += nparams[i] + ", ";
                                            // RenderParamForExtern
                                            ListOfParamsWithTypes += nparams[i] + ":" + RenderParamforExtern(xparams[i]) + ", ";
                                        }
                                        if (ListOfParams.EndsWith(", "))
                                            ListOfParams = ListOfParams.Substring(0, ListOfParams.Length - 2);
                                        if (ListOfParamsWithTypes.EndsWith(", "))
                                            ListOfParamsWithTypes = ListOfParamsWithTypes.Substring(0, ListOfParamsWithTypes.Length - 2);

                                        ReturnType = RenderParamforExtern(xparams.Last());
                                        if (ReturnType == "") ReturnType = "Void";

                                        if (field.Attributes().Count(a => a.Name == "static") > 0 && field.Attribute("static").Value == "1")
                                            IsStatic = true;

                                        if (field.Attributes().Count(a => a.Name == "public") > 0 && field.Attribute("public").Value == "1")
                                            IsPublic = true;

                                        sbExtern.AppendLine("    " + (IsPublic ? "public " : " ") + (IsStatic ? "static " : " ") + "function " + field.Name + "(" + ListOfParamsWithTypes + ") : " + ReturnType + ";");
                                    }
                                    else // sino es campo / propiedad
                                    {
                                        // es propiedad (tiene get o set)
                                        if (
                                            (field.Attributes().Count(a => a.Name == "get") > 0) ||
                                            (field.Attributes().Count(a => a.Name == "set") > 0)
                                            )
                                        {
                                            bool IsStatic = false;
                                            bool IsPublic = false;
                                            bool IsGet = false;
                                            bool IsSet = false;
                                            string FieldType = "";
                                            string PropertyBlock = "";

                                            if (field.Attributes().Count(a => a.Name == "static") > 0 && field.Attribute("static").Value == "1")
                                                IsStatic = true;

                                            if (field.Attributes().Count(a => a.Name == "public") > 0 && field.Attribute("public").Value == "1")
                                                IsPublic = true;

                                            if (field.Attributes().Count(a => a.Name == "get") > 0)
                                                IsGet = true;

                                            if (field.Attributes().Count(a => a.Name == "set") > 0)
                                                IsSet = true;

                                            var TypeNode = field.Elements().FirstOrDefault();
                                            if (TypeNode == null) continue;
                                            if (TypeNode.Attributes().Count(a => a.Name == "path") > 0)
                                                FieldType = RenderParamforExtern(TypeNode);

                                            // renderizar propiedades
                                            if (IsSet == true && IsGet == false && field.Attribute("set").Value == "null")
                                            {
                                                PropertyBlock = "(default, null)";
                                            }
                                            else if (IsSet == false && IsGet == true && field.Attribute("get").Value == "null")
                                            {
                                                PropertyBlock = "(null, default)";
                                            }
                                            else if (IsSet == true && IsGet == true && field.Attribute("get").Value == "accessor" && field.Attribute("set").Value == "accessor")
                                            {
                                                PropertyBlock = "(get, set)";
                                            }
                                            else if (IsSet == true && IsGet == true && field.Attribute("get").Value == "accessor" && field.Attribute("set").Value == "null")
                                            {
                                                PropertyBlock = "(get, never)";
                                            }


                                            //@:isVar public var Name(get, set):String;
                                            sbExtern.AppendLine("    @:isVar " + (IsPublic ? "public " : " ") + (IsStatic ? "static " : " ") + "var " + field.Name + PropertyBlock + ":" + FieldType + ";");
                                        }
                                        // es campo sibmple
                                        else
                                        {
                                            bool IsStatic = false;
                                            bool IsPublic = false;
                                            string FieldType = "";

                                            if (field.Attributes().Count(a => a.Name == "static") > 0 && field.Attribute("static").Value == "1")
                                                IsStatic = true;

                                            if (field.Attributes().Count(a => a.Name == "public") > 0 && field.Attribute("public").Value == "1")
                                                IsPublic = true;

                                            var TypeNode = field.Elements().FirstOrDefault();
                                            if (TypeNode == null) continue;
                                            if (TypeNode.Attributes().Count(a => a.Name == "path") > 0)
                                                FieldType = RenderParamforExtern(TypeNode);

                                            sbExtern.AppendLine("    " + (IsPublic ? "public " : " ") + (IsStatic ? "static " : " ") + "var " + field.Name + ":" + FieldType + ";");
                                        }
                                    }
                                    sbExtern.Append(Environment.NewLine);
                                }
                            }

                            // escritura del archivo en la ruta correcta, si es que tiene algo que escribir
                            if (sbExtern.Length > 2)
                            {
                                // creo la carpeta correcta
                                string InnerPath = "";
                                string[] Parts = LogicType.Namespace.Replace("cornerstone." + ModuleName, "").Split('.');

                                foreach (var part in Parts)
                                    InnerPath += part + "\\";

                                if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\out\\externs\\cornerstone\\" + ModuleName + "\\" + InnerPath))
                                    Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\out\\externs\\cornerstone\\" + ModuleName + "\\" + InnerPath);

                                // cabecera del archivo HX
                                StringBuilder HXheader = new StringBuilder();
                                HXheader.AppendLine("package " + LogicType.Namespace + ";");
                                HXheader.AppendLine();
                                HXheader.AppendLine("extern class " + LogicType.Name + " " + HXImplements + " {");

                                // pie del archivo HX
                                StringBuilder HXfooter = new StringBuilder();
                                HXfooter.AppendLine("}");

                                // escribo el archivo final
                                System.IO.File.WriteAllText(
                                    System.IO.Directory.GetCurrentDirectory() + "\\out\\externs\\cornerstone\\" + ModuleName + "\\" + InnerPath + LogicType.Name + ".hx",
                                    HXheader.ToString() +
                                    sbExtern.ToString() +
                                    HXfooter.ToString()
                                    );
                            }
                        }
                        // identificación de interfaces
                        else if (asm.GetType(LogicType.FullName).IsSubclassOf(asm.GetType("haxe.lang.IHxObject")))
                        {
                            // no renderizo las interfaces porque no tienen rtti, podría haber problemas con los tipos de dato
                            // los módulos integrados no trabajarían con interfaces, sino con objetos directos
                        }
                        // identificación de enumeraciones
                        else if (asm.GetType(LogicType.FullName).IsSubclassOf(asm.GetType("haxe.lang.Enum")))
                        {
                            var fieldn = LogicType.GetField("__hx_constructs");
                            if (fieldn != null)
                            {
                                string[] options = (string[])fieldn.GetValue(null);
                                StringBuilder sbExtern = new StringBuilder();

                                // detallo las opciones de la enumeración
                                foreach (string option in options)
                                {
                                    sbExtern.AppendLine("    " + option + ";");
                                }

                                // escritura del archivo en la ruta correcta, si es que tiene algo que escribir
                                if (sbExtern.Length > 2)
                                {
                                    // creo la carpeta correcta
                                    string InnerPath = "";
                                    string[] Parts = LogicType.Namespace.Replace("cornerstone." + ModuleName, "").Split('.');

                                    foreach (var part in Parts)
                                        InnerPath += part + "\\";

                                    if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\out\\externs\\cornerstone\\" + ModuleName + "\\" + InnerPath))
                                        Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\out\\externs\\cornerstone\\" + ModuleName + "\\" + InnerPath);

                                    // cabecera del archivo HX
                                    StringBuilder HXheader = new StringBuilder();
                                    HXheader.AppendLine("package " + LogicType.Namespace + ";");
                                    HXheader.AppendLine();
                                    HXheader.AppendLine("extern enum " + LogicType.Name + " {");

                                    // pie del archivo HX
                                    StringBuilder HXfooter = new StringBuilder();
                                    HXfooter.AppendLine("}");

                                    // escribo el archivo final
                                    System.IO.File.WriteAllText(
                                        System.IO.Directory.GetCurrentDirectory() + "\\out\\externs\\cornerstone\\" + ModuleName + "\\" + InnerPath + LogicType.Name + ".hx",
                                        HXheader.ToString() +
                                        sbExtern.ToString() +
                                        HXfooter.ToString()
                                        );
                                }
                            }
                        }
                    }


                    //    StringBuilder sbClasses = new StringBuilder();
                    //    //ClassesToRender.Reverse();

                    //    foreach (var ClasstoRender in ClassesToRender)
                    //    {
                    //        TSRenderClasses(ClasstoRender, asm, sbClasses);
                    //    }

                    //    StringBuilder References = new StringBuilder();
                    //    References.AppendLine("/// <reference path=\"cornerstone.integrator.ts\"/>");

                    //    String ProxyContent = References.ToString() + Environment.NewLine + sbClasses.ToString() + Environment.NewLine + sbMethods.ToString();

                    //    System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.ts", ProxyContent, Encoding.UTF8);
                    //    // corrida automática del TypeScript para generar el JS
                    //    // se necesita instalado Microsoft SDKs. tsc.exe /Program Files/Microsoft SDKs/TypeScript/1.0/tsc.exe
                    //    try
                    //    {
                    //        TypeScriptCompiler.Compile(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone." + ModuleName + ".proxy.ts");
                    //    }
                    //    catch (InvalidTypeScriptFileException ex)
                    //    {
                    //        // there was a compiler error, show the compiler output
                    //        //Console.WriteLine(ex.Message);
                    //    }
                }
                catch (System.Reflection.ReflectionTypeLoadException ex)
                {
                    foreach (Exception i in ex.LoaderExceptions)
                        Console.WriteLine(i.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                #endregion
            }
            else
            {
                #region NET

                // borro el directorio de link de CS (linked)
                try
                {
                    System.IO.Directory.Delete(System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\", true);
                }
                catch { }

                DirectoryCopy(System.IO.Directory.GetCurrentDirectory() + "\\out\\net\\",
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\", true);

                // Generar AssemblyInfo
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\src\\cornerstone\\" + ModuleName + "\\AssemblyInfo.cs", GenerarAssemblyInfo("cornerstone." + ModuleName, "cornerstone." + ModuleName, "", "cornerstone." + ModuleName, "", "Copyright " + DateTime.Now.Year.ToString(), "", Guid.NewGuid().ToString()));

                string CSProjFileName = System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\" + AssemblyName + ".csproj";
                string CsProjContent = GenerarCsProj(AssemblyName, System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\src\\");


                System.IO.File.WriteAllText(CSProjFileName, CsProjContent);

                // build csproj
                string projectFileName = CSProjFileName;
                ProjectCollection pc = new ProjectCollection();
                Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                GlobalProperty.Add("Configuration", "Release");
                GlobalProperty.Add("Platform", "AnyCPU");
                BuildRequestData BuidlRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Build" }, null);
                BuildResult buildResult = BuildManager.DefaultBuildManager.Build(new BuildParameters(pc), BuidlRequest);

                // traslado la dll de .net al server
                System.IO.File.Copy(
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\bin\\Release\\" + AssemblyName + ".dll",
                    System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.net\\server\\Bin\\" + AssemblyName + ".dll",
                    true
                    );

                //// traslado la dll de .net al server
                //System.IO.File.Copy(
                //    System.IO.Directory.GetCurrentDirectory() + "\\out\\net\\bin\\Integrator.dll",
                //    System.IO.Directory.GetCurrentDirectory() + "\\out\\net\\bin\\" + AssemblyName + ".dll",
                //    true
                //    );

                #endregion

                #region JAVA

                // borro el directorio de link de CS (linked)
                try
                {
                    System.IO.Directory.Delete(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\", true);
                }
                catch { }

                // creo el directorio Linked
                if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\"))
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\");

                // solamente renombro el jar
                System.IO.File.Copy(
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\jar\\Integrator.jar",
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\" + AssemblyName + ".jar",
                    true
                    );

                // no limpio ni recompilo porque el Integrator entra completo

                // traslado el jar al server.java
                System.IO.File.Copy(
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\" + AssemblyName + ".jar",
                    System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.java\\web\\WEB-INF\\lib\\" + AssemblyName + ".jar",
                    true
                    );


                #endregion

                #region PHP

                if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.php\\lib\\"))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.php\\lib\\");
                }

                // copio el directorio compilado al directorio linked
                DirectoryCopy(System.IO.Directory.GetCurrentDirectory() + "\\out\\php\\lib\\",
                    System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\servers\\server.php\\lib\\", true);

                #endregion

                #region JS

                // traslado del js al cliente
                System.IO.File.Copy(
                    System.IO.Directory.GetCurrentDirectory() + "\\out\\js\\cornerstone.integrator.js",
                    System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.ts",
                    true
                    );

                // procesamiento del archivo, para que el js generado sea global, no solamente específico para la consola
                string JSContent = System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.ts", System.Text.Encoding.UTF8);

                // versión original
                JSContent = JSContent.Replace("(function (console) { \"use strict\";", "");
                JSContent = JSContent.Replace("cornerstone_integrator_Integrator.main();", "");
                JSContent = JSContent.Replace("})(typeof console != \"undefined\" ? console : {log:function(){}});", "");

                // nueva versión
                if (JSContent.Contains("function ($global)"))
                {
                    JSContent = JSContent.Replace("(function ($global) { \"use strict\";", "");
                    JSContent = JSContent.Replace("})(typeof window != \"undefined\" ? window : typeof global != \"undefined\" ? global : typeof self != \"undefined\" ? self : this);", "");
                    JSContent = "var $global = typeof window != \"undefined\" ? window : typeof global != \"undefined\" ? global : typeof self != \"undefined\" ? self : this;" + Environment.NewLine + JSContent;
                }
                JSContent += GenerateCommonCode();
                JSContent = System.Text.RegularExpressions.Regex.Replace(JSContent, @"\r\n|\n\r|\n|\r", Environment.NewLine);

                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.ts", JSContent, System.Text.Encoding.UTF8);

                // corrida automática del TypeScript para generar el JS
                try
                {
                    TypeScriptCompiler.Compile(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.ts");
                }
                catch (InvalidTypeScriptFileException ex)
                {
                    // there was a compiler error, show the compiler output
                    //Console.WriteLine(ex.Message);
                }

                // Minify
                //using (Task<String> task = JavaScripMinifier.MinifyJs(System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.js")))
                //{
                //    task.Wait();
                //    System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.min.js", task.Result);
                //}
                
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.min.js",
                    JavaScripMinifier.MinifyJs3(System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.integrator.js")));

                // Minify Loader
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.loader.min.js",
                    JavaScripMinifier.MinifyJs3(System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\client.html\\js\\cornerstone.loader.js")));

                #endregion
            }
            // mover los archivos a las ubicaciones de ejecución

            Console.WriteLine("Linked");
            //Console.ReadLine();
        }

        private static void Createcmd(string folder, string ModuleName, StringBuilder cmd)
        {
            foreach (string File in System.IO.Directory.GetFiles(folder))
            {
                if (File.Contains("cornerstone\\" + ModuleName))
                    cmd.AppendLine(File.Replace(System.IO.Directory.GetCurrentDirectory() + "\\out\\jarlinked\\", "").Replace("\\", "/"));
            }

            foreach (string dir in System.IO.Directory.GetDirectories(folder))
            {
                Createcmd(dir, ModuleName, cmd);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static string GenerarCsProj(string AssemblyName, string srcFolder)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>"); sb.Append(Environment.NewLine);
            sb.Append("<Project ToolsVersion=\"4.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"); sb.Append(Environment.NewLine);
            sb.Append("  <PropertyGroup>"); sb.Append(Environment.NewLine);
            sb.Append("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>"); sb.Append(Environment.NewLine);
            sb.Append("    <Platform Condition=\" '$(Platform)' == '' \">x86</Platform>"); sb.Append(Environment.NewLine);
            sb.Append("    <OutputType>Library</OutputType>"); sb.Append(Environment.NewLine);
            sb.Append("    <AppDesignerFolder>Properties</AppDesignerFolder>"); sb.Append(Environment.NewLine);
            sb.Append("    <RootNamespace>"); sb.Append(Environment.NewLine);
            sb.Append("    </RootNamespace>"); sb.Append(Environment.NewLine);
            sb.Append("    <AssemblyName>" + AssemblyName + "</AssemblyName>"); sb.Append(Environment.NewLine);
            sb.Append("    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>"); sb.Append(Environment.NewLine);
            sb.Append("    <TargetFrameworkProfile>"); sb.Append(Environment.NewLine);
            sb.Append("    </TargetFrameworkProfile>"); sb.Append(Environment.NewLine);
            sb.Append("    <FileAlignment>512</FileAlignment>"); sb.Append(Environment.NewLine);
            sb.Append("    <ProjectGuid>{" + Guid.NewGuid().ToString() + "}</ProjectGuid>"); sb.Append(Environment.NewLine);
            sb.Append("  </PropertyGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|x86' \">"); sb.Append(Environment.NewLine);
            sb.Append("    <PlatformTarget>x86</PlatformTarget>"); sb.Append(Environment.NewLine);
            sb.Append("    <DebugSymbols>true</DebugSymbols>"); sb.Append(Environment.NewLine);
            sb.Append("    <DebugType>full</DebugType>"); sb.Append(Environment.NewLine);
            sb.Append("    <Optimize>false</Optimize>"); sb.Append(Environment.NewLine);
            sb.Append("    <OutputPath>bin\\Debug\\</OutputPath>"); sb.Append(Environment.NewLine);
            sb.Append("    <DefineConstants>DEBUG;TRACE</DefineConstants>"); sb.Append(Environment.NewLine);
            sb.Append("    <ErrorReport>prompt</ErrorReport>"); sb.Append(Environment.NewLine);
            sb.Append("    <WarningLevel>4</WarningLevel>"); sb.Append(Environment.NewLine);
            sb.Append("  </PropertyGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|x86' \">"); sb.Append(Environment.NewLine);
            sb.Append("    <PlatformTarget>x86</PlatformTarget>"); sb.Append(Environment.NewLine);
            sb.Append("    <DebugType>pdbonly</DebugType>"); sb.Append(Environment.NewLine);
            sb.Append("    <Optimize>true</Optimize>"); sb.Append(Environment.NewLine);
            sb.Append("    <OutputPath>bin\\Release\\</OutputPath>"); sb.Append(Environment.NewLine);
            sb.Append("    <DefineConstants>TRACE</DefineConstants>"); sb.Append(Environment.NewLine);
            sb.Append("    <ErrorReport>prompt</ErrorReport>"); sb.Append(Environment.NewLine);
            sb.Append("    <WarningLevel>4</WarningLevel>"); sb.Append(Environment.NewLine);
            sb.Append("  </PropertyGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)' == 'Debug|x64'\">"); sb.Append(Environment.NewLine);
            sb.Append("    <DebugSymbols>true</DebugSymbols>"); sb.Append(Environment.NewLine);
            sb.Append("    <OutputPath>bin\\x64\\Debug\\</OutputPath>"); sb.Append(Environment.NewLine);
            sb.Append("    <DefineConstants>DEBUG;TRACE</DefineConstants>"); sb.Append(Environment.NewLine);
            sb.Append("    <DebugType>full</DebugType>"); sb.Append(Environment.NewLine);
            sb.Append("    <PlatformTarget>AnyCPU</PlatformTarget>"); sb.Append(Environment.NewLine);
            sb.Append("    <ErrorReport>prompt</ErrorReport>"); sb.Append(Environment.NewLine);
            sb.Append("  </PropertyGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)' == 'Release|x64'\">"); sb.Append(Environment.NewLine);
            sb.Append("    <OutputPath>bin\\x64\\Release\\</OutputPath>"); sb.Append(Environment.NewLine);
            sb.Append("    <DefineConstants>TRACE</DefineConstants>"); sb.Append(Environment.NewLine);
            sb.Append("    <Optimize>true</Optimize>"); sb.Append(Environment.NewLine);
            sb.Append("    <DebugType>pdbonly</DebugType>"); sb.Append(Environment.NewLine);
            sb.Append("    <PlatformTarget>x64</PlatformTarget>"); sb.Append(Environment.NewLine);
            sb.Append("    <ErrorReport>prompt</ErrorReport>"); sb.Append(Environment.NewLine);
            sb.Append("  </PropertyGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'\">"); sb.Append(Environment.NewLine);
            sb.Append("    <PlatformTarget>AnyCPU</PlatformTarget>"); sb.Append(Environment.NewLine);
            sb.Append("    <OutputPath>bin\\Debug\\</OutputPath>"); sb.Append(Environment.NewLine);
            sb.Append("    <ErrorReport>prompt</ErrorReport>"); sb.Append(Environment.NewLine);
            sb.Append("  </PropertyGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)' == 'Release|AnyCPU'\">"); sb.Append(Environment.NewLine);
            sb.Append("    <PlatformTarget>AnyCPU</PlatformTarget>"); sb.Append(Environment.NewLine);
            sb.Append("    <OutputPath>bin\\Release\\</OutputPath>"); sb.Append(Environment.NewLine);
            sb.Append("    <Optimize>true</Optimize>"); sb.Append(Environment.NewLine);
            sb.Append("    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>"); sb.Append(Environment.NewLine);
            sb.Append("    <ErrorReport>prompt</ErrorReport>"); sb.Append(Environment.NewLine);
            sb.Append("    <DebugType>pdbonly</DebugType>"); sb.Append(Environment.NewLine);
            sb.Append("  </PropertyGroup>"); sb.Append(Environment.NewLine);

            sb.Append("  <ItemGroup>"); sb.Append(Environment.NewLine);
            if (!IsIntegrator)
            {
                sb.Append("    <Reference Include=\"Integrator\"><HintPath>..\\..\\..\\cornerstone.integrator\\out\\netlinked\\bin\\Release\\cornerstone.integrator.dll</HintPath></Reference>"); sb.Append(Environment.NewLine);

            }


            sb.Append("    <Reference Include=\"System\" />"); sb.Append(Environment.NewLine);
            sb.Append("    <Reference Include=\"System.ServiceModel\" />"); sb.Append(Environment.NewLine);
            sb.Append("    <Reference Include=\"System.Web\" />"); sb.Append(Environment.NewLine);
            sb.Append("    <Reference Include=\"System.Web.Extensions\" />"); sb.Append(Environment.NewLine);
            sb.Append("    <Reference Include=\"System.Web.Services\" />"); sb.Append(Environment.NewLine);

            sb.Append("    <Reference Include=\"System.Data\" />"); sb.Append(Environment.NewLine);
            sb.Append("    <Reference Include=\"System.XML\" />"); sb.Append(Environment.NewLine);

            sb.Append("  </ItemGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  <ItemGroup>"); sb.Append(Environment.NewLine);

            ProcessCoreFiles(srcFolder, sb);

            sb.Append("  </ItemGroup>"); sb.Append(Environment.NewLine);
            sb.Append("  "); sb.Append(Environment.NewLine);
            sb.Append("  <ItemGroup />"); sb.Append(Environment.NewLine);
            sb.Append("  <Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />"); sb.Append(Environment.NewLine);
            sb.Append("  <!-- To modify your build process, add your task inside one of the targets below and uncomment it."); sb.Append(Environment.NewLine);
            sb.Append("       Other similar extension points exist, see Microsoft.Common.targets."); sb.Append(Environment.NewLine);
            sb.Append("  <Target Name=\"BeforeBuild\">"); sb.Append(Environment.NewLine);
            sb.Append("  </Target>"); sb.Append(Environment.NewLine);
            sb.Append("  <Target Name=\"AfterBuild\">"); sb.Append(Environment.NewLine);
            sb.Append("  </Target>"); sb.Append(Environment.NewLine);
            sb.Append("  -->"); sb.Append(Environment.NewLine);
            sb.Append("</Project>"); sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static string GenerarAssemblyInfo(string Name, string Description, string Configuration, string Company, string Product, string Copyright, string Trademark, string COMID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System.Reflection;"); sb.Append(Environment.NewLine);
            sb.Append("using System.Runtime.CompilerServices;"); sb.Append(Environment.NewLine);
            sb.Append("using System.Runtime.InteropServices;"); sb.Append(Environment.NewLine);
            sb.Append("// General Information about an assembly is controlled through the following "); sb.Append(Environment.NewLine);
            sb.Append("// set of attributes. Change these attribute values to modify the information"); sb.Append(Environment.NewLine);
            sb.Append("// associated with an assembly."); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyTitle(\"" + Name + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyDescription(\"" + Description + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyConfiguration(\"" + Configuration + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyCompany(\"" + Company + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyProduct(\"" + Product + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyCopyright(\"" + Copyright + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyTrademark(\"" + Trademark + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyCulture(\"\")]"); sb.Append(Environment.NewLine);
            sb.Append("// Setting ComVisible to false makes the types in this assembly not visible "); sb.Append(Environment.NewLine);
            sb.Append("// to COM components.  If you need to access a type in this assembly from "); sb.Append(Environment.NewLine);
            sb.Append("// COM, set the ComVisible attribute to true on that type."); sb.Append(Environment.NewLine);
            sb.Append("[assembly: ComVisible(false)]"); sb.Append(Environment.NewLine);
            sb.Append("// The following GUID is for the ID of the typelib if this project is exposed to COM"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: Guid(\"" + COMID + "\")]"); sb.Append(Environment.NewLine);
            sb.Append("// Version information for an assembly consists of the following four values:"); sb.Append(Environment.NewLine);
            sb.Append("//"); sb.Append(Environment.NewLine);
            sb.Append("//      Major Version"); sb.Append(Environment.NewLine);
            sb.Append("//      Minor Version "); sb.Append(Environment.NewLine);
            sb.Append("//      Build Number"); sb.Append(Environment.NewLine);
            sb.Append("//      Revision"); sb.Append(Environment.NewLine);
            sb.Append("//"); sb.Append(Environment.NewLine);
            sb.Append("// You can specify all the values or you can default the Revision and Build Numbers "); sb.Append(Environment.NewLine);
            sb.Append("// by using the '*' as shown below:"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyVersion(\"1.0.0.0\")]"); sb.Append(Environment.NewLine);
            sb.Append("[assembly: AssemblyFileVersion(\"1.0.0.0\")]"); sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static string GenerateCommonCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function ServiceCall(Call : cornerstone_integrator_transport_MethodCall, callback) {"); sb.Append(Environment.NewLine);
            sb.Append("    var req = new cornerstone_integrator_transport_RequestMessage();"); sb.Append(Environment.NewLine);
            sb.Append("    req.set_RequestSeq(100);"); sb.Append(Environment.NewLine);
            sb.Append("    req.set_SessionID(\"\");"); sb.Append(Environment.NewLine);
            sb.Append("    req.set_Calls([]);"); sb.Append(Environment.NewLine);
            sb.Append("    req.get_Calls().push(Call);"); sb.Append(Environment.NewLine);
            sb.Append("    var serializer = new haxe_Serializer();"); sb.Append(Environment.NewLine);
            sb.Append("    serializer.serialize(req);"); sb.Append(Environment.NewLine);
            sb.Append("    var PostSend = serializer.toString();"); sb.Append(Environment.NewLine);
            sb.Append("    // hago la llamada al servicio y obtengo el encriptado de retorno"); sb.Append(Environment.NewLine);
            sb.Append("    $.ajax({"); sb.Append(Environment.NewLine);
            sb.Append("        url: ServiceURL(),"); sb.Append(Environment.NewLine);
            sb.Append("        type: 'POST',"); sb.Append(Environment.NewLine);
            sb.Append("        dataType: 'text',"); sb.Append(Environment.NewLine);
            sb.Append("        data: PostSend,"); sb.Append(Environment.NewLine);
            sb.Append("        contentType: 'text/plain; charset=UTF-8',"); sb.Append(Environment.NewLine);
            sb.Append("        mimeType: 'text/plain',"); sb.Append(Environment.NewLine);
            sb.Append("        crossDomain: true,"); sb.Append(Environment.NewLine);
            sb.Append("        cache: false,"); sb.Append(Environment.NewLine);
            sb.Append("    })"); sb.Append(Environment.NewLine);
            sb.Append("        .done(function (PostReturn) {"); sb.Append(Environment.NewLine);
            sb.Append("            if (!PostReturn || PostReturn == \"\") {"); sb.Append(Environment.NewLine);
            sb.Append("                throw ('No se ha recibido respuesta del servidor.');"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            var unserializer = new haxe_Unserializer(PostReturn);"); sb.Append(Environment.NewLine);
            sb.Append("            var res = new cornerstone_integrator_transport_ResponseMessage();"); sb.Append(Environment.NewLine);
            sb.Append("            res = unserializer.unserialize();"); sb.Append(Environment.NewLine);
            sb.Append("            // Si es una excepción de infraestructura"); sb.Append(Environment.NewLine);
            sb.Append("            if (res.RequestSeq == -3) {"); sb.Append(Environment.NewLine);
            sb.Append("                throw ('Ha ocurrido un error en el procesamiento de su pedido. Por favor tome contacto con el administrador del sistema y proporciónele el siguiente código de error: ' + res.AttentionCode);"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            // Si es una excepción de logica"); sb.Append(Environment.NewLine);
            sb.Append("            if (res.RequestSeq == -1) {"); sb.Append(Environment.NewLine);
            sb.Append("                throw (res.Message + ' (#: ' + res.AttentionCode + ')');"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            // Si es una excepción de seguridad"); sb.Append(Environment.NewLine);
            sb.Append("            if (res.RequestSeq == -2) {"); sb.Append(Environment.NewLine);
            sb.Append("                throw (res.Message + ' (#: ' + res.AttentionCode + ')');"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            // devuelvo el objeto de respuesta"); sb.Append(Environment.NewLine);
            sb.Append("            if (typeof callback === \"function\") {"); sb.Append(Environment.NewLine);
            sb.Append("                if (res.Results.length == 1) {"); sb.Append(Environment.NewLine);
            sb.Append("                    callback(res.Results[0].ReturnObject);"); sb.Append(Environment.NewLine);
            sb.Append("                }"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("        })"); sb.Append(Environment.NewLine);
            sb.Append("        .fail(function (jqXHR, textStatus, errorThrown) {"); sb.Append(Environment.NewLine);
            sb.Append("            throw ('No se ha recibido respuesta del servidor, #: ' + jqXHR.status);"); sb.Append(Environment.NewLine);
            sb.Append("            //throw (errorThrown);"); sb.Append(Environment.NewLine);
            sb.Append("            // registro y búsqueda de servidor alternativo"); sb.Append(Environment.NewLine);
            sb.Append("        })"); sb.Append(Environment.NewLine);
            sb.Append("        .always(function () {"); sb.Append(Environment.NewLine);
            sb.Append("        });"); sb.Append(Environment.NewLine);
            sb.Append("}"); sb.Append(Environment.NewLine);
            sb.Append("function ServiceMultiCall(Calls: Array<cornerstone_integrator_transport_MethodCall>, callback) {"); sb.Append(Environment.NewLine);
            sb.Append("    var req = new cornerstone_integrator_transport_RequestMessage();"); sb.Append(Environment.NewLine);
            sb.Append("    req.set_RequestSeq(100);"); sb.Append(Environment.NewLine);
            sb.Append("    req.set_SessionID(\"\");"); sb.Append(Environment.NewLine);
            sb.Append("    req.set_Calls(Calls);"); sb.Append(Environment.NewLine);
            sb.Append("    var serializer = new haxe_Serializer();"); sb.Append(Environment.NewLine);
            sb.Append("    serializer.serialize(req);"); sb.Append(Environment.NewLine);
            sb.Append("    var PostSend = serializer.toString();"); sb.Append(Environment.NewLine);
            sb.Append("    // hago la llamada al servicio y obtengo el encriptado de retorno"); sb.Append(Environment.NewLine);
            sb.Append("    $.ajax({"); sb.Append(Environment.NewLine);
            sb.Append("        url: ServiceURL(),"); sb.Append(Environment.NewLine);
            sb.Append("        type: 'POST',"); sb.Append(Environment.NewLine);
            sb.Append("        dataType: 'text',"); sb.Append(Environment.NewLine);
            sb.Append("        data: PostSend,"); sb.Append(Environment.NewLine);
            sb.Append("        contentType: 'text/plain; charset=UTF-8',"); sb.Append(Environment.NewLine);
            sb.Append("        mimeType: 'text/plain',"); sb.Append(Environment.NewLine);
            sb.Append("        crossDomain: true,"); sb.Append(Environment.NewLine);
            sb.Append("        cache: false,"); sb.Append(Environment.NewLine);
            sb.Append("    })"); sb.Append(Environment.NewLine);
            sb.Append("        .done(function (PostReturn) {"); sb.Append(Environment.NewLine);
            sb.Append("            if (!PostReturn || PostReturn == \"\") {"); sb.Append(Environment.NewLine);
            sb.Append("                throw ('No se ha recibido respuesta del servidor.');"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            var unserializer = new haxe_Unserializer(PostReturn);"); sb.Append(Environment.NewLine);
            sb.Append("            var res = new cornerstone_integrator_transport_ResponseMessage();"); sb.Append(Environment.NewLine);
            sb.Append("            res = unserializer.unserialize();"); sb.Append(Environment.NewLine);
            sb.Append("            // Si es una excepción de infraestructura"); sb.Append(Environment.NewLine);
            sb.Append("            if (res.RequestSeq == -3) {"); sb.Append(Environment.NewLine);
            sb.Append("                throw ('Ha ocurrido un error en el procesamiento de su pedido. Por favor tome contacto con el administrador del sistema y proporciónele el siguiente código de error: ' + res.AttentionCode);"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            // Si es una excepción de logica"); sb.Append(Environment.NewLine);
            sb.Append("            if (res.RequestSeq == -1) {"); sb.Append(Environment.NewLine);
            sb.Append("                throw (res.Message + ' (#: ' + res.AttentionCode + ')');"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            // Si es una excepción de seguridad"); sb.Append(Environment.NewLine);
            sb.Append("            if (res.RequestSeq == -2) {"); sb.Append(Environment.NewLine);
            sb.Append("                throw (res.Message + ' (#: ' + res.AttentionCode + ')');"); sb.Append(Environment.NewLine);
            sb.Append("                return;"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("            // devuelvo el objeto de respuesta"); sb.Append(Environment.NewLine);
            sb.Append("            if (typeof callback === \"function\") {"); sb.Append(Environment.NewLine);
            sb.Append("                for (var i = 0; i <= res.Results.length - 1; i++) {"); sb.Append(Environment.NewLine);
            sb.Append("                    callback(res.Results[i].ReturnObject, res.Results[i].Name);"); sb.Append(Environment.NewLine);
            sb.Append("                }"); sb.Append(Environment.NewLine);
            sb.Append("            }"); sb.Append(Environment.NewLine);
            sb.Append("        })"); sb.Append(Environment.NewLine);
            sb.Append("        .fail(function (jqXHR, textStatus, errorThrown) {"); sb.Append(Environment.NewLine);
            sb.Append("            throw ('No se ha recibido respuesta del servidor, #: ' + jqXHR.status);"); sb.Append(Environment.NewLine);
            sb.Append("            //throw (errorThrown);"); sb.Append(Environment.NewLine);
            sb.Append("            // registro y búsqueda de servidor alternativo"); sb.Append(Environment.NewLine);
            sb.Append("        })"); sb.Append(Environment.NewLine);
            sb.Append("        .always(function () {"); sb.Append(Environment.NewLine);
            sb.Append("        });"); sb.Append(Environment.NewLine);
            sb.Append("}"); sb.Append(Environment.NewLine);
            sb.Append("function SaveToDisk(blobURL, fileName) {"); sb.Append(Environment.NewLine);
            sb.Append("    var reader = new FileReader();"); sb.Append(Environment.NewLine);
            sb.Append("    reader.readAsDataURL(blobURL);"); sb.Append(Environment.NewLine);
            sb.Append("    reader.onload = function (event) {"); sb.Append(Environment.NewLine);
            sb.Append("        var save = document.createElement('a');"); sb.Append(Environment.NewLine);
            sb.Append("        save.href = event.target.result;"); sb.Append(Environment.NewLine);
            sb.Append("        save.target = '_blank';"); sb.Append(Environment.NewLine);
            sb.Append("        save.download = fileName || 'unknown file';"); sb.Append(Environment.NewLine);
            sb.Append("        var event = document.createEvent('Event');"); sb.Append(Environment.NewLine);
            sb.Append("        event.initEvent('click', true, true);"); sb.Append(Environment.NewLine);
            sb.Append("        save.dispatchEvent(event);"); sb.Append(Environment.NewLine);
            sb.Append("        (window.URL || window.webkitURL).revokeObjectURL(save.href);"); sb.Append(Environment.NewLine);
            sb.Append("    };"); sb.Append(Environment.NewLine);
            sb.Append("}"); sb.Append(Environment.NewLine);
            sb.Append("function readMultipleFiles(evt) {"); sb.Append(Environment.NewLine);
            sb.Append("    //Retrieve all the files from the FileList object"); sb.Append(Environment.NewLine);
            sb.Append("    var files = evt.target.files;"); sb.Append(Environment.NewLine);
            sb.Append("    if (files) {"); sb.Append(Environment.NewLine);
            sb.Append("        for (var i = 0, f; f = files[i]; i++) {"); sb.Append(Environment.NewLine);
            sb.Append("            var r = new FileReader();"); sb.Append(Environment.NewLine);
            sb.Append("            r.onload = (function (f) {"); sb.Append(Environment.NewLine);
            sb.Append("                return function (e) {"); sb.Append(Environment.NewLine);
            sb.Append("                    var contents = e.target.result;"); sb.Append(Environment.NewLine);
            sb.Append("                    alert(\"Got the file.n\""); sb.Append(Environment.NewLine);
            sb.Append("                        + \"name: \" + f.name + \"n\""); sb.Append(Environment.NewLine);
            sb.Append("                        + \"type: \" + f.type + \"n\""); sb.Append(Environment.NewLine);
            sb.Append("                        + \"size: \" + f.size + \" bytesn\""); sb.Append(Environment.NewLine);
            sb.Append("                        + \"starts with: \" + contents.substr(1, contents.indexOf(\"n\"))"); sb.Append(Environment.NewLine);
            sb.Append("                    );"); sb.Append(Environment.NewLine);
            sb.Append("                };"); sb.Append(Environment.NewLine);
            sb.Append("            })(f);"); sb.Append(Environment.NewLine);
            sb.Append("            r.readAsText(f);"); sb.Append(Environment.NewLine);
            sb.Append("        }"); sb.Append(Environment.NewLine);
            sb.Append("    } else {"); sb.Append(Environment.NewLine);
            sb.Append("        alert(\"Failed to load files\");"); sb.Append(Environment.NewLine);
            sb.Append("    }"); sb.Append(Environment.NewLine);
            sb.Append("}"); sb.Append(Environment.NewLine);
            sb.Append("// <input type=\"file\" id=\"fileinput\" multiple />"); sb.Append(Environment.NewLine);
            sb.Append("//  document.getElementById('fileinput').addEventListener('change', readMultipleFiles, false);"); sb.Append(Environment.NewLine);
            sb.Append("window.onerror = function (msg, url, line, col, error) {"); sb.Append(Environment.NewLine);
            sb.Append("    alert(error);"); sb.Append(Environment.NewLine);
            sb.Append("    var suppressErrorAlert = true;"); sb.Append(Environment.NewLine);
            sb.Append("    return suppressErrorAlert;"); sb.Append(Environment.NewLine);
            sb.Append("}"); sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        private static void ProcessCoreFiles(string srcFolder, StringBuilder sb)
        {
            foreach (string file in System.IO.Directory.GetFiles(srcFolder))
            {
                sb.Append("    <Compile Include=\"" + file.Replace(System.IO.Directory.GetCurrentDirectory() + "\\out\\netlinked\\", "") + "\"/>"); sb.Append(Environment.NewLine);
            }

            foreach (string dir in System.IO.Directory.GetDirectories(srcFolder))
            {
                ProcessCoreFiles(dir, sb);
            }
        }

        private static void SubTypeCheck(string strType, System.Reflection.Assembly asm, List<string> ClassesToRender)
        {
            if (asm.GetType(strType).IsSubclassOf(asm.GetType("haxe.lang.HxObject")))
            {
                object obj = asm.CreateInstance(strType);

                if (obj.GetType().GetField("__rtti") != null)
                {
                    // obtengo el XML
                    var xml = obj.GetType().GetField("__rtti").GetValue(obj).ToString();

                    // lo parseo para leerlo con Linq
                    XDocument doc = XDocument.Parse(xml);

                    // obtengo los métodos declarados en el XML
                    var fields = from nclass in doc.Elements("class")
                                 from nfields in nclass.Elements()
                                 where nfields.Attributes().Count(a => a.Name == "get") > 0 &&
                                    nfields.Attribute("get").Value == "accessor"
                                 select nfields;

                    // obtención de la lista de propiedades (deben estar declaradas como propiedades)
                    foreach (var field in fields)
                    {
                        var subTypes = from tt in field.Descendants()
                                       where tt.Attributes().Count(a => a.Name == "path") > 0
                                       select tt;

                        // barro todos los nodos que tiene un path (tipo de dato)
                        foreach (var subType in subTypes)
                        {
                            if (IsCustomClass(subType) && subType.Attributes().Count(a => a.Name == "path") > 0)
                            {
                                if (subType.Attribute("path").Value == "List" || subType.Attribute("path").Value == "Array")
                                {
                                    foreach (var t in subType.Elements().Where(e => e.Attributes().Count(a => a.Name == "path") > 0 && IsCustomClass(e)))
                                    {
                                        if (IsCustomClass(t))
                                        {
                                            if (!ClassesToRender.Contains(t.Attribute("path").Value))
                                            {
                                                ClassesToRender.Add(t.Attribute("path").Value);
                                                SubTypeCheck(t.Attribute("path").Value, asm, ClassesToRender);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!ClassesToRender.Contains(subType.Attribute("path").Value))
                                    {
                                        ClassesToRender.Add(subType.Attribute("path").Value);
                                        SubTypeCheck(subType.Attribute("path").Value, asm, ClassesToRender);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            else if (asm.GetType(strType).IsSubclassOf(asm.GetType("haxe.lang.Enum")))
            {
                if (!ClassesToRender.Contains(strType))
                    ClassesToRender.Add(strType);
            }
        }

        private static void TSRenderClasses(string ClasstoRender, System.Reflection.Assembly asm, StringBuilder sbClasses)
        {
            if (asm.GetType(ClasstoRender).IsSubclassOf(asm.GetType("haxe.lang.HxObject")))
            {
                object obj = asm.CreateInstance(ClasstoRender);

                if (obj.GetType().GetField("__rtti") != null)
                {
                    // obtengo el XML
                    var xml = obj.GetType().GetField("__rtti").GetValue(obj).ToString();

                    // lo parseo para leerlo con Linq
                    XDocument doc = XDocument.Parse(xml);

                    // obtengo los métodos declarados en el XML
                    var fields = from nclass in doc.Elements("class")
                                 from nfields in nclass.Elements()
                                 where
                                    nfields.Attributes().Count(a => a.Name == "get") > 0 &&
                                    nfields.Attribute("get").Value == "accessor" // propiedad
                                 select nfields;

                    sbClasses.AppendLine("class " + obj.GetType().FullName.Replace(".", "_") + " {");


                    // obtención de la lista de propiedades (deben estar declaradas como propiedades en haxe)
                    foreach (var field in fields)
                    {
                        var inType = from n in field.Elements()
                                     where n.Attributes().Count(a => a.Name == "path") > 0
                                     select n;
                        sbClasses.AppendLine("    " + field.Name + ":" + RenderParamforTS(inType.First()) + ";");
                    }
                    sbClasses.AppendLine("    static __name__: Array<any>;");

                    sbClasses.AppendLine("}");
                    sbClasses.Append("$hxClasses[\"" + obj.GetType().Namespace + "." + obj.GetType().Name + "\"] = " + obj.GetType().Namespace.Replace(".", "_") + "_" + obj.GetType().Name + ";"); sbClasses.Append(Environment.NewLine);
                    sbClasses.Append("" + obj.GetType().Namespace.Replace(".", "_") + "_" + obj.GetType().Name + ".__name__ = [\"" + obj.GetType().Namespace.Split('.').Aggregate((a, b) => a + "\",\"" + b) + "\", \"" + obj.GetType().Name + "\"];"); sbClasses.Append(Environment.NewLine);
                    sbClasses.Append("" + obj.GetType().Namespace.Replace(".", "_") + "_" + obj.GetType().Name + ".prototype = {"); sbClasses.Append(Environment.NewLine);
                    foreach (var field in fields)
                    {
                        sbClasses.AppendLine("    " + field.Name + ": null,");
                    }
                    sbClasses.Append("	__class__: " + obj.GetType().Namespace.Replace(".", "_") + "_" + obj.GetType().Name + ""); sbClasses.Append(Environment.NewLine);

                    sbClasses.Append("};"); sbClasses.Append(Environment.NewLine);
                }
            }
            else if (asm.GetType(ClasstoRender).IsSubclassOf(asm.GetType("haxe.lang.Enum")))
            {
                var Options = from field in asm.GetType(ClasstoRender).GetFields()
                              where field.IsStatic == true && field.Name != "__hx_constructs"
                              select field;


                sbClasses.Append("class " + asm.GetType(ClasstoRender).Namespace.Replace(".", "_") + "_" + asm.GetType(ClasstoRender).Name + " {"); sbClasses.Append(Environment.NewLine);
                sbClasses.Append("    static __ename__: Array<any> = [\"" + asm.GetType(ClasstoRender).Namespace + "." + asm.GetType(ClasstoRender).Name + "\"];"); sbClasses.Append(Environment.NewLine);
                sbClasses.Append("    static __constructs__: Array<string> = [\"" + (Options.Select(a => a.Name).Aggregate((a, b) => a + "\",\"" + b)) + "\"];"); sbClasses.Append(Environment.NewLine);
                foreach (var option in Options)
                {
                    sbClasses.Append("    static " + option.Name + ": any;"); sbClasses.Append(Environment.NewLine);
                }
                sbClasses.Append("}"); sbClasses.Append(Environment.NewLine);
                sbClasses.Append("$hxClasses[\"" + asm.GetType(ClasstoRender).Namespace + "." + asm.GetType(ClasstoRender).Name + "\"] = " + asm.GetType(ClasstoRender).Namespace.Replace(".", "_") + "_" + asm.GetType(ClasstoRender).Name + ";"); sbClasses.Append(Environment.NewLine);

                int enumcount = 0;
                foreach (var option in Options)
                {
                    sbClasses.Append("" + asm.GetType(ClasstoRender).Namespace.Replace(".", "_") + "_" + asm.GetType(ClasstoRender).Name + "." + option.Name + " = [\"" + option.Name + "\", " + enumcount.ToString() + "];"); sbClasses.Append(Environment.NewLine);
                    sbClasses.Append("" + asm.GetType(ClasstoRender).Namespace.Replace(".", "_") + "_" + asm.GetType(ClasstoRender).Name + "." + option.Name + ".toString = $estr;"); sbClasses.Append(Environment.NewLine);
                    sbClasses.Append("" + asm.GetType(ClasstoRender).Namespace.Replace(".", "_") + "_" + asm.GetType(ClasstoRender).Name + "." + option.Name + ".__enum__ = " + asm.GetType(ClasstoRender).Namespace.Replace(".", "_") + "_" + asm.GetType(ClasstoRender).Name + ";"); sbClasses.Append(Environment.NewLine);
                    enumcount++;
                }
                sbClasses.Append(Environment.NewLine);
            }

        }

        private static bool IsCustomClass(XElement xElement)
        {
            if (xElement.Attributes().Count(a => a.Name == "path") == 0) return false;

            string Path = xElement.Attribute("path").Value;

            switch (Path)
            {
                case "Void":
                    return false;
                    break;
                case "String":
                    return false;
                    break;
                case "Int":
                    return false;
                    break;
                case "Float":
                    return false;
                    break;
                case "Bool":
                    return false;
                    break;
                case "haxe.io.UInt8Array":
                    return false;
                case "haxe.io.Bytes":
                    return false;
                    break;
                case "Date":
                    return false;
                    break;
                case "haxe.Int64":
                    return false;
                    break;
                case "Map":
                    return false;
                    break;
                case "List":
                    return IsCustomClass(xElement.Elements().First());
                    break;
                case "Array":
                    return IsCustomClass(xElement.Elements().First());
                    break;
                default:
                    return true;
                    break;
            }
        }

        private static string RenderParamforTS(XElement xElement)
        {
            if (xElement.Attributes().Count(a => a.Name == "path") == 0) return "any";

            string Path = xElement.Attribute("path").Value;

            switch (Path)
            {
                case "String":
                    return "string";
                    break;
                case "Int":
                    return "number";
                    break;
                case "Float":
                    return "number";
                    break;
                case "Bool":
                    return "boolean";
                    break;
                case "haxe.io.UInt8Array":
                case "haxe.io.Bytes":
                    return "Uint8Array";
                    break;
                case "Date":
                    return "Date";
                    break;
                case "haxe.Int64":
                    return "haxe__$Int64__$_$_$Int64";
                    break;
                case "Map":
                    if (xElement.Elements().First().Attribute("path").Value == "Int")
                        return "haxe_ds_IntMap";
                    else
                        return "haxe_ds_StringMap";
                    break;
                case "List":
                    return "Array<" + xElement.Elements().First().Attribute("path").Value.Replace(".", "_") + ">";
                    break;
                case "Array":
                    if (xElement.Elements().First().Name == "d")
                        return "Array<any>";
                    else
                        return "Array<" + xElement.Elements().First().Attribute("path").Value.Replace(".", "_") + ">";
                    break;
                default:
                    return Path.Replace(".", "_");
                    break;
            }
        }

        private static string RenderParamforExtern(XElement xElement)
        {
            if (xElement.Attributes().Count(a => a.Name == "path") == 0) return "Dynamic";
            string Path = xElement.Attribute("path").Value;

            switch (Path)
            {
                case "String":
                    return Path;
                    break;
                case "Int":
                    return Path;
                    break;
                case "Float":
                    return Path;
                    break;
                case "Bool":
                    return Path;
                    break;
                case "haxe.io.UInt8Array":
                    return Path;
                    break;
                case "haxe.io.Bytes":
                    return Path;
                    break;
                case "Date":
                    return Path;
                    break;
                case "haxe.Int64":
                    return Path;
                    break;
                case "Map":
                    string Ts = "";
                    foreach (var element in xElement.Elements())
                    {
                        Ts += RenderParamforExtern(element) + ",";
                    }
                    if (Ts.EndsWith(","))
                        Ts = Ts.Substring(0, Ts.Length - 1);
                    return "Map<" + Ts + ">";
                    break;
                case "List":
                    return "List<" + RenderParamforExtern(xElement.Elements().First()) + ">";
                    break;
                case "Array":
                    return "Array<" + RenderParamforExtern(xElement.Elements().First()) + ">";
                    break;
                default:
                    return Path;
                    break;
            }
        }

        public static string RenderWS(string ModuleName)
        {
            #region WebServices
            StringBuilder sbMethods = new StringBuilder();
            sbMethods.Append(Environment.NewLine);

            try
            {
                // crear archivo proxy
                string bin = System.IO.Directory.GetCurrentDirectory() + "\\out\\net\\bin\\";

                DirectoryInfo oDirectoryInfo = new DirectoryInfo(bin);
                Assembly asm = null;

                //Instanciación del Assembly del proyecto
                if (oDirectoryInfo.Exists)
                {
                    //Foreach Assembly with dll as the extension
                    foreach (FileInfo oFileInfo in oDirectoryInfo.GetFiles("*.dll", SearchOption.AllDirectories))
                    {

                        Assembly tempAssembly = null;

                        //Before loading the assembly, check all current loaded assemblies in case talready loaded
                        //has already been loaded as a reference to another assembly
                        //Loading the assembly twice can cause major issues
                        foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            //Check the assembly is not dynamically generated as we are not interested in these
                            if (loadedAssembly.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                            {
                                //Get the loaded assembly filename
                                string sLoadedFilename =
                                    loadedAssembly.CodeBase.Substring(loadedAssembly.CodeBase.LastIndexOf('/') + 1);

                                //If the filenames match, set the assembly to the one that is already loaded
                                if (sLoadedFilename.ToUpper() == oFileInfo.Name.ToUpper())
                                {
                                    tempAssembly = loadedAssembly;
                                    break;
                                }
                            }
                        }

                        //If the assembly is not aleady loaded, load it manually
                        if (tempAssembly == null)
                        {
                            tempAssembly = Assembly.LoadFile(oFileInfo.FullName);
                        }

                        asm = tempAssembly;
                    }

                }

                if (asm == null)
                    return "";

                // se itera por los tipos y métodos, buscando las funciones de lógica "Logic_"
                var LogicTypes = (from t in asm.GetTypes()
                                  from m in t.GetMethods()
                                  where t.IsSealed == true &&
                                    t.FullName.Contains("cornerstone." + ModuleName) &&
                                   m.Name.StartsWith("Logic_")
                                  select t).Distinct();

                if (LogicTypes.Count() == 0)
                    return "";

                List<string> ClassesToRender = new List<string>();


                foreach (var LogicType in LogicTypes)
                {
                    // se instancia la clase de Lógica encontrada
                    //var LogicInstance = System.Reflection.Assembly.GetAssembly(LogicType).CreateInstance(LogicType.FullName);

                    //// Leo el RTTI
                    //if (LogicInstance.GetType().GetField("__rtti") != null)
                    //{

                    var fieldn = LogicType.GetField("__rtti");
                    if (fieldn != null)
                    {
                        var xml = fieldn.GetValue(null).ToString();
                        // obtengo el XML
                        //var xml = LogicInstance.GetType().GetField("__rtti").GetValue(LogicInstance).ToString();

                        // lo parseo para leerlo con Linq
                        XDocument doc = XDocument.Parse(xml);

                        // obtengo los métodos declarados en el XML
                        var methods = from nclass in doc.Elements("class")
                                      from nmethod in nclass.Elements()
                                      where nmethod.Name != "meta" &&
                                            nmethod.Name != "implements" &&
                                            nmethod.Attribute("set").Value == "method" &&
                                            nmethod.Name.ToString().StartsWith("Logic_") &&
                                            nmethod.Elements().Count(e => e.Name == "meta") > 0 &&
                                            nmethod.Element("meta").Elements().Count(e => e.Name == "m") > 0 &&
                                            nmethod.Element("meta").Element("m").Attributes().Count(a => a.Name == "n") > 0 &&
                                            (nmethod.Element("meta").Element("m").Attribute("n").Value.ToLower() == "integration" ||
                                            nmethod.Element("meta").Element("m").Attribute("n").Value.ToLower() == "webservice")
                                      select nmethod;

                        // generación de los métodos en TypeScript
                        foreach (var method in methods)
                        {
                            // obtengo la lista de nombres de parámetro
                            string[] nparams = method.Element("f").Attribute("a").Value.Split(':');
                            XElement[] xparams = method.Element("f").Elements().ToArray();

                            string ListOfParams = "";
                            string ListOfParamsWithTypes = "";
                            string ReturnType = "";


                            for (int i = 0; i < nparams.Length; i++)
                            {
                                if (nparams[i] == "") continue;
                                ListOfParams += nparams[i] + ", ";
                                ListOfParamsWithTypes += RenderParamforCS(xparams[i]) + " " + nparams[i] + ", ";
                            }
                            if (ListOfParams.EndsWith(", "))
                            {
                                ListOfParams = ListOfParams.Substring(0, ListOfParams.Length - 2);
                            }
                            if (ListOfParamsWithTypes.EndsWith(", "))
                            {
                                ListOfParamsWithTypes = ListOfParamsWithTypes.Substring(0, ListOfParamsWithTypes.Length - 2);
                            }

                            ReturnType = RenderParamforCS(xparams.Last());

                            sbMethods.Append("    [WebMethod]"); sbMethods.Append(Environment.NewLine);
                            sbMethods.Append("    [OperationContract]"); sbMethods.Append(Environment.NewLine);
                            sbMethods.Append("    public " + ReturnType + " " + LogicType.Name + "_" + method.Name.ToString().Replace("Logic_", "") + "(" + ListOfParamsWithTypes + ") {"); sbMethods.Append(Environment.NewLine);
                            sbMethods.Append("        " + LogicType.Name + " obj = new " + LogicType.Name + "();"); sbMethods.Append(Environment.NewLine);
                            sbMethods.Append("        " + (ReturnType == "void" ? "" : "return") + " obj." + method.Name + "(" + ListOfParams + ");"); sbMethods.Append(Environment.NewLine);
                            sbMethods.Append("    }"); sbMethods.Append(Environment.NewLine);
                            sbMethods.Append(Environment.NewLine);
                            sbMethods.Append(Environment.NewLine);
                        }
                    }
                }
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                foreach (Exception i in ex.LoaderExceptions)
                    Console.WriteLine(i.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return sbMethods.ToString();

            #endregion
        }

        public static string GenerateWSHeader(string ModuleName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;"); sb.Append(Environment.NewLine);
            sb.Append("using System.Collections.Generic;"); sb.Append(Environment.NewLine);
            sb.Append("using System.Linq;"); sb.Append(Environment.NewLine);
            sb.Append("using System.Web;"); sb.Append(Environment.NewLine);
            sb.Append("using System.Web.Services;"); sb.Append(Environment.NewLine);
            sb.Append("using System.ServiceModel;"); sb.Append(Environment.NewLine);
            sb.Append("using System.Web.Script.Services;"); sb.Append(Environment.NewLine);
            sb.Append("namespace cornerstone." + ModuleName); sb.Append(Environment.NewLine);
            sb.Append("{"); sb.Append(Environment.NewLine);
            sb.Append("    /// <summary>"); sb.Append(Environment.NewLine);
            sb.Append("    /// Summary description for cornerstone_" + ModuleName); sb.Append(Environment.NewLine);
            sb.Append("    /// </summary>"); sb.Append(Environment.NewLine);
            sb.Append("    [WebService(Namespace = \"http://cornerstone." + ModuleName + "/\")]"); sb.Append(Environment.NewLine);
            sb.Append("    [ServiceContract(Namespace = \"http://cornerstone." + ModuleName + "/\")]"); sb.Append(Environment.NewLine);
            sb.Append("    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]"); sb.Append(Environment.NewLine);
            sb.Append("    [System.ComponentModel.ToolboxItem(false)]"); sb.Append(Environment.NewLine);
            sb.Append("    [ScriptService]"); sb.Append(Environment.NewLine);
            sb.Append("    public class WS : System.Web.Services.WebService"); sb.Append(Environment.NewLine);
            sb.Append("    {"); sb.Append(Environment.NewLine);
            sb.Append("       [WebMethod]"); sb.Append(Environment.NewLine);
            sb.Append("       [OperationContract]"); sb.Append(Environment.NewLine);
            sb.Append("        public string Ping(string data)"); sb.Append(Environment.NewLine);
            sb.Append("        {"); sb.Append(Environment.NewLine);
            sb.Append("            return \"Ping \" + data;"); sb.Append(Environment.NewLine);
            sb.Append("        }"); sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static string GenerateWSFooter()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("    }"); sb.Append(Environment.NewLine);
            sb.Append("}"); sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        private static string RenderParamforCS(XElement xElement)
        {
            if (xElement.Attributes().Count(a => a.Name == "path") == 0) return "any";

            string Path = xElement.Attribute("path").Value;

            switch (Path)
            {
                case "String":
                    return "string";
                    break;
                case "Int":
                    return "int";
                    break;
                case "Float":
                    return "float";
                    break;
                case "Bool":
                    return "bool";
                    break;
                case "haxe.io.UInt8Array":
                case "haxe.io.Bytes":
                    return "Bytes";
                    break;
                case "Date":
                    return "DateTime";
                    break;
                case "Void":
                    return "void";
                    break;
                case "haxe.Int64":
                    return "long";
                    break;
                case "Map":
                    if (xElement.Elements().First().Attribute("path").Value == "Int")
                        return "IntMap";
                    else
                        return "StringMap";
                    break;
                case "List":
                    return "List<" + xElement.Elements().First().Attribute("path").Value + ">";
                    break;
                case "Array":
                    if (xElement.Elements().First().Name == "d")
                        return "List<object>";
                    else
                        return "List<" + xElement.Elements().First().Attribute("path").Value + ">";
                    break;
                default:
                    return Path;
                    break;
            }
        }

    }
}
