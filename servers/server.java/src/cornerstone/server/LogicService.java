
package cornerstone.server;

import cornerstone.integrator.Integrator;
import cornerstone.integrator.exceptions.ExceptionManager;
import cornerstone.integrator.transport.RequestMessage;
import cornerstone.server.ExPos;
import haxe.lang.Exceptions;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

@WebServlet(
        name = "LogicService"
)
public class LogicService extends HttpServlet {
    public LogicService() {
    }

    protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
        String ResData = "";
        String PostData = "";

        try {
            Integrator.Init();
            BufferedReader out = new BufferedReader(new InputStreamReader(request.getInputStream(), "UTF-8"));
            PostData = out.readLine();
            if(PostData.equals("")) {
                return;
            }

            ResData = Integrator.Process(PostData);
        } catch (Exception ex) {
            Exceptions.setException(ex);
            ExPos pos = new ExPos("LogicService", "doPost", "LogicService.java", 0);
            ResData = ExceptionManager.HandleException(haxe.root.Exception.wrap(ex, pos), PostData, (RequestMessage)null, (String)null, (String)null, (String)null, (String)null);
        }

        response.setContentType("text/plain");
        response.addHeader("Access-Control-Allow-Origin", "*");  //"http://localhost:49709");
        PrintWriter out1 = response.getWriter();
        out1.print(ResData);
    }

    protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
        PrintWriter out = response.getWriter();
        out.print("VERB GET NOT ALLOWED");
    }
}
