package cornerstone.benchmark;

import haxe.Log;
import Date;
import cornerstone.integrator.helpers.Int64_Helper;
import haxe.Int64;
import haxe.io.UInt8Array;
import cornerstone.integrator.exceptions.ExceptionManager;
import cornerstone.integrator.interfaces.IDisposable;
import sys.io.File;
import sys.FileSystem;

@:keepSub @:final @:rtti
class Benchmark implements IDisposable {
    public function new() {
    }

    public function Logic_Test1():Float {
        return Math.PI * Math.PI;
    }

    public function Logic_Test2(Par:String):String {
        return Par + " Welcome";
    }

    public function Logic_Test3():Date {
        var day:Float = 1000 * 60 * 60 * 24;
        var d = Date.now();
        var d2 = Date.fromTime(d.getTime() + day);
        return d2;
    }

    public function Logic_Benchmark1():Float {
        var DateStart = Date.now();

//        var a = [
//            for (a in 1...1000)
//                a
//        ];

        var x:Float = 0.5;
        var y:Float = 0.5;
        var t:Float = 0.49999975;
        var t2:Float = 0.5;

        for (i in 0...100) {
            for (j in 0...1000) {
                x = t * Math.atan(t2 * Math.sin(x) * Math.cos(x) / (Math.cos(x + y) + Math.cos(x - y) - 1.0));
                y = t * Math.atan(t2 * Math.sin(y) * Math.cos(y) / (Math.cos(x + y) + Math.cos(x - y) - 1.0));
                //x = t*atan(t2*sin(x)*cos(x)/(cos(x+y)+cos(x-y)-1.0));
                //y = t*atan(t2*sin(y)*cos(y)/(cos(x+y)+cos(x-y)-1.0));
            }
        }

        return Date.now().getTime() - DateStart.getTime();
    }

    public function Logic_Benchmark2():String {
        var Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var d = new Probs();
        var Buffer = "";

        for (r in 0...100) {
            for (i in 0...1000) {
                var pos:Int = Std.int(d.getNext() * Letters.length);
                Buffer += Letters.charAt(pos);
            }
            var reduce:Int = Std.int(d.getNext() * (Buffer.length / 2));
            Buffer = Buffer.substr(Buffer.length - reduce, reduce);
        }

        return Buffer;
    }

    public function Logic_Benchmark3():Date {
        var day:Float = 1000 * 60 * 60 * 24;
        var d = Date.now();
        var p = new Probs();

        for (r in 0...100) {
            for (i in 0...1000) {
                var diff1 = ((p.getNext() - 0.55) * 90.0 * day);
                d = Date.fromTime(d.getTime() + diff1);
//                Log.trace(d, null);
//                Log.trace(diff1, null);
            }
        }

        return d;
    }

    public function Dispose():Void {
    }
}
